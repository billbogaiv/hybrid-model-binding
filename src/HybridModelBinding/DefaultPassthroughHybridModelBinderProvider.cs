using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace HybridModelBinding
{
    public class DefaultPassthroughHybridModelBinderProvider : HybridModelBinderProvider
    {
        public DefaultPassthroughHybridModelBinderProvider(
            IList<IInputFormatter> formatters,
            IHttpRequestStreamReaderFactory readerFactory)
            : base(
                 new HybridBindingSource(),
                 new DefaultPassthroughHybridModelBinder(formatters, readerFactory))
        { }
    }
}
