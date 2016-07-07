using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;
using System.Collections.Generic;

namespace HybridModelBinding
{
    public class DefaultHybridModelBinderProvider : HybridModelBinderProvider
    {
        public DefaultHybridModelBinderProvider(
            IList<IInputFormatter> formatters,
            IHttpRequestStreamReaderFactory readerFactory)
            :base(
                 new HybridBindingSource(),
                 new DefaultHybridModelBinder(formatters, readerFactory))
        { }
    }
}
