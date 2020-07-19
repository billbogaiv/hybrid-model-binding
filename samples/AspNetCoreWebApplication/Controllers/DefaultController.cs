using AspNetCoreWebApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreWebApplication.Controllers
{
    public class DefaultController : Controller
    {
        [HttpGet]
        [Route("{age?}/{name?}")]
        public IActionResult Index(IndexModel model)
        {
            return View(model);
        }
    }
}
