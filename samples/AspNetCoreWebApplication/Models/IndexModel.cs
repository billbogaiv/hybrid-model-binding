using HybridModelBinding;
using System.Collections.Generic;

namespace AspNetCoreWebApplication.Models
{
    public class IndexModel : IHybridBoundModel
    {
        [From(Source.Header, Source.Body, Source.QueryString, Source.Route)]
        public int? Age { get; set; }

        [HybridBindProperty(Source.Header, "X-Name")]
        [HybridBindProperty(new[] { Source.Body, Source.Form, Source.QueryString, Source.Route })]
        public string Name { get; set; }

        public IDictionary<string, string> HybridBoundProperties { get; } = new Dictionary<string, string>();
    }
}
