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
            if (bindingSource == null)
            {
                throw new ArgumentNullException(nameof(bindingSource));
            }

            if (modelBinder == null)
            {
                throw new ArgumentNullException(nameof(modelBinder));
            }

            this.bindingSource = bindingSource;
            this.modelBinder = modelBinder;
        }

        private BindingSource bindingSource { get; }
        private IModelBinder modelBinder { get; }

        public virtual IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.BindingInfo?.BindingSource != null &&
                context.BindingInfo.BindingSource.CanAcceptDataFrom(bindingSource))
            {
                return modelBinder;
            }
            else
            {
                return null;
            }
        }
    }
}
