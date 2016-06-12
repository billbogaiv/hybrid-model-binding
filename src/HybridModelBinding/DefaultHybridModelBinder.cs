using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace HybridModelBinding
{
    public class DefaultHybridModelBinder : HybridModelBinder
    {
        public DefaultHybridModelBinder(IHttpRequestStreamReaderFactory readerFactory)
        {
            ModelBinders.Add(new BodyModelBinder(readerFactory));

            ValueProviderFactories.Add(new FormValueProviderFactory());
            ValueProviderFactories.Add(new RouteValueProviderFactory());
            ValueProviderFactories.Add(new QueryStringValueProviderFactory());
        }
    }
}
