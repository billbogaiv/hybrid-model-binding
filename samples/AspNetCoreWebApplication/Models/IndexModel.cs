using HybridModelBinding;

namespace AspNetCoreWebApplication.Models
{
    public class IndexModel
    {
        [From(Source.Header, Source.Body, Source.QueryString, Source.Route)]
        public int? Age { get; set; }

        [HybridBindProperty(Source.Header, "X-Name")]
        [HybridBindProperty(new[] { Source.Body, Source.Form, Source.QueryString, Source.Route })]
        public string Name { get; set; }
    }
}
