using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;
using System.Collections.Generic;

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
