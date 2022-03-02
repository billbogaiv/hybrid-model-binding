using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace HybridModelBinding
{
    public abstract class HybridModelBinderProvider : IModelBinderProvider
    {
        public HybridModelBinderProvider(
            BindingSource bindingSource,
            IModelBinder modelBinder)
        {
            _bindingSource = bindingSource ?? throw new ArgumentNullException(nameof(bindingSource));
            _modelBinder = modelBinder ?? throw new ArgumentNullException(nameof(modelBinder));
        }

        private readonly BindingSource _bindingSource; 
        private readonly IModelBinder _modelBinder; 

        public virtual IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.BindingInfo?.BindingSource != null &&
                context.BindingInfo.BindingSource.CanAcceptDataFrom(_bindingSource))
            {
                return _modelBinder;
            }

            return null;
        }
    }
}
