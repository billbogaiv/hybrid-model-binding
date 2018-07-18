using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace HybridModelBinding
{
    public class DefaultUnsafeHybridModelBinderProvider : HybridModelBinderProvider
    {
        public DefaultUnsafeHybridModelBinderProvider(
            IList<IInputFormatter> formatters,
            IHttpRequestStreamReaderFactory readerFactory)
            : base(
                 new HybridBindingSource(),
                 new DefaultUnsafeHybridModelBinder(formatters, readerFactory))
        { }
    }
}
