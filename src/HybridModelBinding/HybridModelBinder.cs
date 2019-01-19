using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections;
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
            object baseModel = null;
            object hydratedModel = null;
            var boundProperties = new List<KeyValuePair<string, string>>();
            var modelBinderId = string.Empty;
            var valueProviders = new List<KeyValuePair<string, IValueProvider>>();

            foreach (var kvp in modelBinders)
            {
                await kvp.Value.BindModelAsync(bindingContext);

                hydratedModel = bindingContext.Result.Model;

                if (hydratedModel != null)
                {
                    modelBinderId = kvp.Key;

                    break;
                }
            }

            if (hydratedModel == null)
            {
                try
                {
                    // Not sure why this throws an exception. For now, we can ignore as it does not seem to affect the outcome.
                    bindingContext.ModelState.Clear();
                }
                catch (Exception) { }

                // First, let us try and use DI to get the model.
                hydratedModel = bindingContext.HttpContext.RequestServices.GetService(bindingContext.ModelType);

                if (hydratedModel == null)
                {
                    try
                    {
                        /**
                         * Using DI did not work, so let us get crude. This might also fail since the model
                         * may not have a parameterless constructor.
                         */
                        hydratedModel = Activator.CreateInstance(bindingContext.ModelType);
                    }
                    catch (Exception)
                    {
                        bindingContext.Result = ModelBindingResult.Failed();

                        return;
                    }
                }

                bindingContext.Result = ModelBindingResult.Success(hydratedModel);
            }

            baseModel = hydratedModel;

            var modelProperties = hydratedModel.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in modelProperties)
            {
                var propertyValue = property.GetValue(hydratedModel, null);

                if (propertyValue?.Equals(property.GetValue(baseModel, null)) == false)
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

                if (property.CanWrite && !valueProviderIds.Any(x => matchingPropertyNameBoundProperties.Contains(x)))
                {
                    property.SetValue(hydratedModel, property.GetValue(baseModel, null), null);
                }

                var boundValueProviderIds = new List<string>(boundProperties.Where(x => x.Value == property.Name).Select(x => x.Key));

                foreach (var valueProviderId in valueProviderIds)
                {
                    if (activeBindStrategy(boundValueProviderIds.ToArray(), valueProviderIds))
                    {
                        var valueProvider = valueProviders.FirstOrDefault(x => x.Key == valueProviderId).Value;

                        if (valueProvider != null)
                        {
                            var matchingUriParams = valueProvider.GetValue(property.Name)
                                .Where(x => !string.IsNullOrEmpty(x))
                                .ToList();

                            if (matchingUriParams.Any())
                            {
                                if (matchingUriParams.Count() == 1)
                                {
                                    var matchingUriParam = matchingUriParams.First();
                                    var descriptor = TypeDescriptor.GetConverter(property.PropertyType);

                                    if (descriptor.CanConvertFrom(matchingUriParam.GetType()))
                                    {
                                        property.SetValue(hydratedModel, descriptor.ConvertFrom(matchingUriParam), null);

                                        boundValueProviderIds.Add(valueProviderId);
                                        boundProperties.Add(new KeyValuePair<string, string>(valueProviderId, property.Name));
                                    }
                                }
                                else
                                {
                                    var descriptor = property.PropertyType.GenericTypeArguments.Any()
                                        ? TypeDescriptor.GetConverter(property.PropertyType.GenericTypeArguments.First())
                                        : TypeDescriptor.GetConverter(property.PropertyType);

                                    var propertyListType = typeof(List<>).MakeGenericType(property.PropertyType.GenericTypeArguments.First());
                                    var propertyListInstance = (IList)Activator.CreateInstance(propertyListType);

                                    foreach (var matchingUriParam in matchingUriParams)
                                    {
                                        if (descriptor.CanConvertFrom(matchingUriParam.GetType()))
                                        {
                                            try
                                            {
                                                propertyListInstance.Add(descriptor.ConvertFrom(matchingUriParam));
                                            }
                                            catch (Exception) { }
                                        }
                                    }

                                    if (propertyListInstance.Count == matchingUriParams.Count)
                                    {
                                        property.SetValue(hydratedModel, propertyListInstance, null);
                                    }
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
    }
}
