using Microsoft.AspNetCore.Mvc;
using project_ca_nhan.Areas.Admin;
using project_ca_nhan.Areas.Admin.Attributes;
using project_ca_nhan.Models;
namespace project_ca_nhan.Areas.Admin.Controllers
{
    [CheckLogin]
    [Area("Admin")]
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            string err3 = !String.IsNullOrEmpty(TempData["ViewBag.err3"].ToString()) != null ? TempData["ViewBag.err3"].ToString() : "";
            ViewBag.err3 = err3.ToString();
            string err2 = !String.IsNullOrEmpty(TempData["ViewBag.err3"].ToString()) != null ? TempData["ViewBag.err3"].ToString() : "";
            ViewBag.err2 = err2.ToString();
            string err1 = !String.IsNullOrEmpty(TempData["ViewBag.err3"].ToString()) != null ? TempData["ViewBag.err3"].ToString() : "";
            ViewBag.err1 = err1.ToString();
            string err5 = !String.IsNullOrEmpty(TempData["ViewBag.err5"].ToString()) != null ? TempData["ViewBag.err5"].ToString() : "";
            ViewBag.err5 = err5.ToString();
            return View();
        }
    }
}
