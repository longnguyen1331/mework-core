using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers
{
    [Route("contact/")]
    public class ContactController : Controller
	{
        private readonly ILogger<ContactController> _logger;

        public ContactController(ILogger<ContactController> logger)
        {
            _logger = logger;
        }
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}