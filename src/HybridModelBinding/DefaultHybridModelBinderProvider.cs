using Microsoft.AspNetCore.Mvc.Formatters;
using System.Collections.Generic;

#if NET451
using Microsoft.AspNetCore.Mvc.Internal;
#else
using Microsoft.AspNetCore.Mvc.Infrastructure;
#endif


namespace HybridModelBinding
{
    public class DefaultHybridModelBinderProvider : HybridModelBinderProvider
    {
        public DefaultHybridModelBinderProvider(
            IList<IInputFormatter> formatters,
            IHttpRequestStreamReaderFactory readerFactory)
            : base(
                 new HybridBindingSource(),
                 new DefaultHybridModelBinder(formatters, readerFactory))
        { }
    }
}
