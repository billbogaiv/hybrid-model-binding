using System;

namespace HybridModelBinding
{
    [Obsolete("Use `" + nameof(HybridBindPropertyAttribute) + "` instead.")]
    [AttributeUsage(AttributeTargets.Property)]
    public class FromAttribute : Attribute
    {
        /// <summary>
        /// Overall ordering with other usages of `HybridPropertyAttribute` is given priority.
        /// </summary>
        public FromAttribute(params string[] valueProviders)
        {
            ValueProviders = valueProviders;
        }

        protected FromAttribute(HybridModelBinder.BindStrategy strategy, params string[] valueProviders)
            : this(valueProviders)
        {
            Strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

        public HybridModelBinder.BindStrategy Strategy { get; }
        public string[] ValueProviders { get; }
    }
}
