using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HybridModelBinding
{
    public abstract class HybridModelBinder : IModelBinder
    {
        public HybridModelBinder(bool isGreedy = false)
        {
            this.isGreedy = isGreedy;
        }

        private readonly bool isGreedy;
        private readonly IList<KeyValuePair<Type, IValueProviderFactory>> mappedValueProviderFactories = new List<KeyValuePair<Type, IValueProviderFactory>>();
        private readonly IList<KeyValuePair<Type, IValueProvider>> mappedValueProviders = new List<KeyValuePair<Type, IValueProvider>>();
        private readonly IList<IModelBinder> modelBinders = new List<IModelBinder>();

        public HybridModelBinder AddValueProviderFactory(IValueProviderFactory factory)
        {
            if (!isGreedy)
            {
                throw new MethodAccessException(
                    $"This method cannot be called when the binder is not greedy. Use `{nameof(AddMappedValueProviderFactory)}`.");
            }
            else
            {
                mappedValueProviderFactories.Add(
                    new KeyValuePair<Type, IValueProviderFactory>(null, factory));

                return this;
            }
        }

        public HybridModelBinder AddMappedValueProviderFactory<TAttribute>(IValueProviderFactory factory)
            where TAttribute : Attribute
        {
            var existingKeys = mappedValueProviderFactories.Select(x => x.Key);

            if (existingKeys.Contains(typeof(TAttribute)))
            {
                throw new ArgumentException(
                    "Provided attribute already exists in collection.",
                    nameof(TAttribute));
            }
            else
            {
                mappedValueProviderFactories.Add(
                    new KeyValuePair<Type, IValueProviderFactory>(typeof(TAttribute), factory));

                return this;
            }
        }

        public HybridModelBinder AddModelBinder(IModelBinder modelBinder)
        {
            modelBinders.Add(modelBinder);

            return this;
        }

        public virtual async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            object model = null;

            foreach (var modelBinder in modelBinders)
            {
                await modelBinder.BindModelAsync(bindingContext);

                model = bindingContext.Result.Model;

                if (model != null)
                {
                    break;
                }
            }

            if (model == null)
            {
                try
                {
                    // Not sure why this throws an exception. For now, we can ignore as it does not seem to affect the outcome.
                    bindingContext.ModelState.Clear();
                }
                catch (Exception) { }

                // First, let us try and use DI to get the model.
                model = bindingContext.HttpContext.RequestServices.GetService(bindingContext.ModelType);

                if (model == null)
                {
                    try
                    {
                        /**
                         * Using DI did not work, so let us get crude. This might also fail since the model
                         * may not have a parameterless constructor.
                         */
                        model = Activator.CreateInstance(bindingContext.ModelType);
                    }
                    catch (Exception)
                    {
                        bindingContext.Result = ModelBindingResult.Failed();

                        return;
                    }
                }

                bindingContext.Result = ModelBindingResult.Success(model);
            }

            var valueProviderFactoryContext = new ValueProviderFactoryContext(bindingContext.ActionContext);

            foreach (var kvp in mappedValueProviderFactories)
            {
                await kvp.Value.CreateValueProviderAsync(valueProviderFactoryContext);

                var valueProvider = valueProviderFactoryContext.ValueProviders.LastOrDefault();

                if (valueProvider != null)
                {
                    mappedValueProviders.Add(new KeyValuePair<Type, IValueProvider>(kvp.Key, valueProvider));
                }
            }

            if (isGreedy)
            {
                foreach (var property in model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    foreach (var valueProvider in valueProviderFactoryContext.ValueProviders)
                    {
                        var matchingUriParam = valueProvider.GetValue(property.Name).FirstValue;

                        if (!string.IsNullOrEmpty(matchingUriParam))
                        {
                            var descriptor = TypeDescriptor.GetConverter(property.PropertyType);

                            if (descriptor.CanConvertFrom(matchingUriParam.GetType()))
                            {
                                property.SetValue(model, descriptor.ConvertFrom(matchingUriParam), null);
                            }
                        }
                    }
                }
            }
            else
            {
                var mappedValueProviderTypes = mappedValueProviders.Select(x => x.Key);

                foreach (var property in model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    foreach (var attribute in property.GetCustomAttributes())
                    {
                        if (mappedValueProviderTypes.Contains(attribute.GetType()))
                        {
                            var valueProvider = mappedValueProviders.FirstOrDefault(x => x.Key == attribute.GetType()).Value;

                            if (valueProvider != null)
                            {
                                var matchingUriParam = valueProvider.GetValue(property.Name).FirstValue;

                                if (!string.IsNullOrEmpty(matchingUriParam))
                                {
                                    var descriptor = TypeDescriptor.GetConverter(property.PropertyType);

                                    if (descriptor.CanConvertFrom(matchingUriParam.GetType()))
                                    {
                                        property.SetValue(model, descriptor.ConvertFrom(matchingUriParam), null);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
