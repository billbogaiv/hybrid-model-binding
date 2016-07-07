using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Collections.Generic;

namespace HybridModelBinding
{
    public class DefaultHybridModelBinder : HybridModelBinder
    {
        public DefaultHybridModelBinder(
            IList<IInputFormatter> formatters,
            IHttpRequestStreamReaderFactory readerFactory)
        {
            AddModelBinder(new BodyModelBinder(formatters, readerFactory))
                .AddMappedValueProviderFactory<FromFormAttribute>(new FormValueProviderFactory())
                .AddMappedValueProviderFactory<FromRouteAttribute>(new RouteValueProviderFactory())
                .AddMappedValueProviderFactory<FromQueryAttribute>(new QueryStringValueProviderFactory());
        }
    }
}
