using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

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
