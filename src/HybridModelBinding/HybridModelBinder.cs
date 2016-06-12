using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace HybridModelBinding
{
    public abstract class HybridModelBinder : IModelBinder
    {
        protected IList<IModelBinder> ModelBinders { get; } = new List<IModelBinder>();
        protected IList<IValueProviderFactory> ValueProviderFactories { get; } = new List<IValueProviderFactory>();

        public virtual async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            object model = null;

            foreach (var modelBinder in ModelBinders)
            {
                await modelBinder.BindModelAsync(bindingContext);

                model = bindingContext.Result.Value.Model;

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
                model = bindingContext.OperationBindingContext.HttpContext.RequestServices.GetService(bindingContext.ModelType);

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
                        bindingContext.Result = ModelBindingResult.Failed(bindingContext.ModelName);

                        return;
                    }
                }

                bindingContext.Result = ModelBindingResult.Success(bindingContext.ModelName, model);
            }

            var valueProviderFactoryContext = new ValueProviderFactoryContext(bindingContext.OperationBindingContext.ActionContext);

            foreach (var valueProviderFactory in ValueProviderFactories)
            {
                await valueProviderFactory.CreateValueProviderAsync(valueProviderFactoryContext);
            }

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
    }
}
