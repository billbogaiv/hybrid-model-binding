using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HybridModelBinding
{
    public sealed class HybridBindingSource : BindingSource
    {
        public HybridBindingSource()
            : base("Hybrid", "Hybrid", true, true)
        { }

        public override bool CanAcceptDataFrom(BindingSource bindingSource)
        {
            return bindingSource.Id == "Hybrid";
        }
    }
}
