using System.Collections.Generic;

namespace HybridModelBinding
{
    public class HybridModelBinderOptions
    {
        public IEnumerable<string> FallbackBindingOrder { get; set; }
        public bool Passthrough { get; set; }
    }
}