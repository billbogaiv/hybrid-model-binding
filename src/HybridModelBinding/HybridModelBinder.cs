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
        public HybridModelBinder(BindStrategy bindStrategy)
        {
            this.bindStrategy = bindStrategy;
        }

        private readonly BindStrategy bindStrategy;
        private readonly IList<KeyValuePair<string, IValueProviderFactory>> valueProviderFactories = new List<KeyValuePair<string, IValueProviderFactory>>();
        private readonly IList<KeyValuePair<string, IModelBinder>> modelBinders = new List<KeyValuePair<string, IModelBinder>>();

        public delegate bool BindStrategy(
            string[] previouslyBoundValueProviderIds,
            string[] allValueProviderIds);

        public HybridModelBinder AddModelBinder(
            string id,
            IModelBinder modelBinder)
        {
            modelBinders.Add(new KeyValuePair<string, IModelBinder>(id, modelBinder));

            return this;
        }

        public HybridModelBinder AddValueProviderFactory(
            string id,
            IValueProviderFactory factory)
        {
            var existingKeys = valueProviderFactories.Select(x => x.Key);

            if (existingKeys.Contains(id))
            {
                throw new ArgumentException(
                    $"Provided `id` of `{id}` already exists in collection.",
                    nameof(id));
            }
            else
            {
                valueProviderFactories.Add(
                    new KeyValuePair<string, IValueProviderFactory>(id, factory));

                return this;
            }
        }

        public virtual async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            object model = null;
            var boundProperties = new List<KeyValuePair<string, string>>();
            var modelBinderId = string.Empty;
            var valueProviders = new List<KeyValuePair<string, IValueProvider>>();

            foreach (var kvp in modelBinders)
            {
                await kvp.Value.BindModelAsync(bindingContext);

                model = bindingContext.Result.Model;

                if (model != null)
                {
                    modelBinderId = kvp.Key;

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

            var modelProperties = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in modelProperties)
            {
                var propertyValue = property.GetValue(model, null);

                if (propertyValue?.Equals(GetDefaultPropertyValue(property.PropertyType)) == false)
                {
                    boundProperties.Add(new KeyValuePair<string, string>(modelBinderId, property.Name));
                }
            }

            var valueProviderFactoryContext = new ValueProviderFactoryContext(bindingContext.ActionContext);

            foreach (var kvp in valueProviderFactories)
            {
                await kvp.Value.CreateValueProviderAsync(valueProviderFactoryContext);

                var valueProvider = valueProviderFactoryContext.ValueProviders.LastOrDefault();

                if (valueProvider != null)
                {
                    valueProviders.Add(new KeyValuePair<string, IValueProvider>(kvp.Key, valueProvider));
                }
            }

            foreach (var property in modelProperties)
            {
                var fromAttribute = property.GetCustomAttribute<FromAttribute>();
                var activeBindStrategy = fromAttribute?.Strategy == null
                    ? bindStrategy
                    : fromAttribute?.Strategy;
                var valueProviderIds = fromAttribute?.ValueProviders;

                if (valueProviderIds == null || valueProviderIds.Count() == 0)
                {
                    valueProviderIds = new[] { modelBinderId }.Concat(valueProviders.Select(x => x.Key)).ToArray();
                }

                var nonMatchingBoundProperties = boundProperties
                    .Where(x => x.Value == property.Name && !valueProviderIds.Contains(x.Key)).ToList();

                foreach (var nonMatchingBoundProperty in nonMatchingBoundProperties)
                {
                    boundProperties.Remove(nonMatchingBoundProperty);
                }

                var matchingPropertyNameBoundProperties = boundProperties
                    .Where(x => x.Value == property.Name)
                    .Select(x => x.Key);

                if (!valueProviderIds.Any(x => matchingPropertyNameBoundProperties.Contains(x)))
                {
                    property.SetValue(model, GetDefaultPropertyValue(property.PropertyType), null);
                }

                var boundValueProviderIds = new List<string>(boundProperties.Where(x => x.Value == property.Name).Select(x => x.Key));

                foreach (var valueProviderId in valueProviderIds)
                {
                    if (activeBindStrategy(boundValueProviderIds.ToArray(), valueProviderIds))
                    {
                        var valueProvider = valueProviders.FirstOrDefault(x => x.Key == valueProviderId).Value;

                        if (valueProvider != null)
                        {
                            var matchingUriParam = valueProvider.GetValue(property.Name).FirstValue;

                            if (!string.IsNullOrEmpty(matchingUriParam))
                            {
                                var descriptor = TypeDescriptor.GetConverter(property.PropertyType);

                                if (descriptor.CanConvertFrom(matchingUriParam.GetType()))
                                {
                                    property.SetValue(model, descriptor.ConvertFrom(matchingUriParam), null);

                                    boundValueProviderIds.Add(valueProviderId);
                                    boundProperties.Add(new KeyValuePair<string, string>(valueProviderId, property.Name));
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private object GetDefaultPropertyValue(Type propertyType)
        {
            return propertyType.GetTypeInfo().IsPrimitive
                ? Activator.CreateInstance(propertyType)
                : null;
        }
    }
}
