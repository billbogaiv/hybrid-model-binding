using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace HybridModelBinding
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class FromHybridAttribute : Attribute, IBindingSourceMetadata
    {
        public BindingSource BindingSource => new HybridBindingSource();
    }
}
