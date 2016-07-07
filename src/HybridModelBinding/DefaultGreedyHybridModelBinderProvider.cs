using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;
using System.Collections.Generic;

namespace HybridModelBinding
{
    public class DefaultGreedyHybridModelBinderProvider : HybridModelBinderProvider
    {
        public DefaultGreedyHybridModelBinderProvider(
            IList<IInputFormatter> formatters,
            IHttpRequestStreamReaderFactory readerFactory)
            :base(
                 new HybridBindingSource(),
                 new DefaultGreedyHybridModelBinder(formatters, readerFactory))
        { }
    }
}
