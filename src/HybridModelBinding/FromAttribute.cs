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

        protected FromAttribute(HybridModelBinder.BindStrategy strategy, params string[] valueProviders)
            : this(valueProviders)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            Strategy = strategy;
        }

        public HybridModelBinder.BindStrategy Strategy { get; private set; }
        public string[] ValueProviders { get; private set; }
    }
}
