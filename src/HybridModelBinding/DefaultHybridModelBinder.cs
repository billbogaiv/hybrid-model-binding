using HybridModelBinding.ModelBinding;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Collections.Generic;
using static HybridModelBinding.Source;

namespace HybridModelBinding
{
    public class DefaultHybridModelBinder : HybridModelBinder
    {
        public DefaultHybridModelBinder(
            IList<IInputFormatter> formatters,
            IHttpRequestStreamReaderFactory readerFactory,
            IEnumerable<string> fallbackBindingOrder)
            : base(Strategy.FirstInWins, fallbackBindingOrder)
        {
            base
                .AddModelBinder(Body, new BodyModelBinder(formatters, readerFactory))
                .AddValueProviderFactory(Form, new FormValueProviderFactory())
                .AddValueProviderFactory(Route, new RouteValueProviderFactory())
                .AddValueProviderFactory(QueryString, new QueryStringValueProviderFactory())
                .AddValueProviderFactory(Header, new HeaderValueProviderFactory())
                .AddValueProviderFactory(Claim, new ClaimValueProviderFactory());
        }
    }
}
