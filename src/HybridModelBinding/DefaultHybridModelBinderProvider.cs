using Microsoft.AspNetCore.Mvc.Internal;

namespace HybridModelBinding
{
    public class DefaultHybridModelBinderProvider : HybridModelBinderProvider
    {
        public DefaultHybridModelBinderProvider(IHttpRequestStreamReaderFactory readerFactory)
            :base(
                 new HybridBindingSource(),
                 new DefaultHybridModelBinder(readerFactory))
        { }
    }
}
