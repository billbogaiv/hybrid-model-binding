using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HybridModelBinding.ModelBinding
{
    public class ClaimValueProviderFactory : IValueProviderFactory
    {
        private readonly BindingSource Claim = new BindingSource(
            "Claim",
            "BindingSource_Claim",
            false,
            true);

        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var claimsPrincipal = context.ActionContext.HttpContext.User;
            if (claimsPrincipal != null)
            {
                var valueProvider = new ClaimValueProvider(
                    Claim,
                    claimsPrincipal);

                context.ValueProviders.Add(valueProvider);
            }

            return Task.CompletedTask;
        }
    }

    public class ClaimValueProvider : BindingSourceValueProvider
    {
        private readonly ClaimsPrincipal _claimsPrincipal;

        public ClaimValueProvider(BindingSource bindingSource, ClaimsPrincipal claimsPrincipal) : base(bindingSource)
        {
            _claimsPrincipal = claimsPrincipal;
        }

        public override bool ContainsPrefix(string prefix) => _claimsPrincipal.HasClaim(claim => claim.Type == prefix);

        public override ValueProviderResult GetValue(string key)
        {
            var claim = _claimsPrincipal.FindFirst(key);
            var claimValue = claim?.Value;

            return claimValue != null ? new ValueProviderResult(claimValue) : ValueProviderResult.None;
        }
    }
}