using HybridModelBinding;

namespace AspNetCoreWebApplication.Models
{
    public class IndexModel
    {
        [From(Source.Body, Source.QueryString, Source.Route)]
        public int? Age { get; set; }

        [From(Source.Body, Source.Form, Source.QueryString, Source.Route)]
        public string Name { get; set; }
    }
}
