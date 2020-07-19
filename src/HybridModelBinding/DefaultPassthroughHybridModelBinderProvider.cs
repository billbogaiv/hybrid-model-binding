using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Generic;

namespace HybridModelBinding
{
    public class DefaultPassthroughHybridModelBinderProvider : HybridModelBinderProvider
    {
        public DefaultPassthroughHybridModelBinderProvider(
            IList<IInputFormatter> formatters,
            IHttpRequestStreamReaderFactory readerFactory,
            IEnumerable<string> fallbackBindingOrder)
            : base(
                 new HybridBindingSource(),
                 new DefaultPassthroughHybridModelBinder(formatters, readerFactory, fallbackBindingOrder))
        { }
    }
}
