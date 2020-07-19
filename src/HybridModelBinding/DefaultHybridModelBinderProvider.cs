using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Generic;

namespace HybridModelBinding
{
    public class DefaultHybridModelBinderProvider : HybridModelBinderProvider
    {
        public DefaultHybridModelBinderProvider(
            IList<IInputFormatter> formatters,
            IHttpRequestStreamReaderFactory readerFactory,
            IEnumerable<string> fallbackBindingOrder)
            : base(
                 new HybridBindingSource(),
                 new DefaultHybridModelBinder(formatters, readerFactory, fallbackBindingOrder))
        { }
    }
}
