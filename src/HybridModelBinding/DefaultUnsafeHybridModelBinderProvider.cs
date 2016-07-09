using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;
using System.Collections.Generic;

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
