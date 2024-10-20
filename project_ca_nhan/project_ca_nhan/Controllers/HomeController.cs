using Microsoft.AspNetCore.Mvc;

namespace project_ca_nhan.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //return Redirect("/Admin");
            return View();
        }
    }
}
