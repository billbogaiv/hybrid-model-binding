using System;

namespace HybridModelBinding
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class HybridBindClassAttribute : Attribute
    {
        public HybridBindClassAttribute(string[] defaultBindingOrder)
        {
            DefaultBindingOrder = defaultBindingOrder ?? throw new ArgumentNullException(nameof(defaultBindingOrder));
        }

        public string[] DefaultBindingOrder { get; private set; }
    }
}
