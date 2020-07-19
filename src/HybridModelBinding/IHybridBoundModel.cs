using System.Collections.Generic;

namespace HybridModelBinding
{
    public interface IHybridBoundModel
    {
        IDictionary<string, string> HybridBoundProperties { get; }

        internal (bool propertyIsBound, string source) IsBound(string name) => HybridBoundProperties.ContainsKey(name)
            ? (true, HybridBoundProperties[name])
            : (false, string.Empty);
    }
}
