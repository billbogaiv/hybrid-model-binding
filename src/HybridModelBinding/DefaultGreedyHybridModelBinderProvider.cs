using Microsoft.AspNetCore.Mvc.Internal;

namespace HybridModelBinding
{
    public class DefaultGreedyHybridModelBinderProvider : HybridModelBinderProvider
    {
        public DefaultGreedyHybridModelBinderProvider(IHttpRequestStreamReaderFactory readerFactory)
            :base(
                 new HybridBindingSource(),
                 new DefaultGreedyHybridModelBinder(readerFactory))
        { }
    }
}
