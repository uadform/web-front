using Microsoft.AspNetCore.Mvc;

namespace web_front.Controllers
{
    public class LoginController : Controller
    {
        //[Route("Login")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
