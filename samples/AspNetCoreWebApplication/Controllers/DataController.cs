using System.Text.Json.Serialization;
using HybridModelBinding;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AspNetCoreWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        [HttpPost]
        public ActionResult<string> Index(DataModel model)
        {
            return $"{model.Name} - {model.IsAdmin}";
        }
    }

    public class DataModel
    {
        [HybridBindProperty(Source.Body)]
        public string Name { get; set; }

        [HybridBindProperty(Source.Body)]
        [JsonPropertyName("is_admin"), JsonProperty("is_admin")]
        public bool IsAdmin { get; set; }

        [HybridBindProperty(Source.Header, "X-UserId")]
        public int UserId { get; set; }
    }
}