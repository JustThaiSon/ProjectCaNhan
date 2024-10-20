using Microsoft.AspNetCore.Mvc;
using project_ca_nhan.Areas.Admin.Attributes;
namespace project_ca_nhan.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckLogin]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
