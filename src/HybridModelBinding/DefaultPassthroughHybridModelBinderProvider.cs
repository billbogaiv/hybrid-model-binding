using Microsoft.AspNetCore.Mvc.Formatters;
using System.Collections.Generic;

#if NET451
using Microsoft.AspNetCore.Mvc.Internal;
#else
using Microsoft.AspNetCore.Mvc.Infrastructure;
#endif

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
