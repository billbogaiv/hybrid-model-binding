using System;

namespace HybridModelBinding
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FromAttribute : Attribute
    {
        public FromAttribute(params string[] valueProviders)
        {
            ValueProviders = valueProviders;
        }

        public Unsafe IsUnsafe { get; set; } = Unsafe.Undefined;
        public string[] ValueProviders { get; private set; }
    }
}
