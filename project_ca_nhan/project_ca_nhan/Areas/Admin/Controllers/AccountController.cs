using Microsoft.AspNetCore.Mvc;
using BC = BCrypt.Net;
using project_ca_nhan.Models;
namespace project_ca_nhan.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        public MyDbcontext db = new MyDbcontext();
        public IActionResult Login()
        {
            ViewBag.action = "/Admin/Account/LoginPost";
            return View();
        }
        [HttpPost]
        public IActionResult LoginPost(IFormCollection fc)
        {
            string _email = fc["email"].ToString().Trim();
            string _password = fc["password"].ToString().Trim();
            ItemUsers item = db.Users.Where(s=>s.Email == _email).FirstOrDefault();
            if(item != null)
            {
                if (BC.BCrypt.Verify(_password,item.Password))
                {
                    HttpContext.Session.SetString("Admin_user_email",item.Email.ToString());
                    HttpContext.Session.SetString("Admin_user_password",item.Password.ToString());
                    return RedirectToAction("Index","Home");
                }
            }
            return Redirect("/Admin/Account/Login?notify=failse");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Admin_user_email");
            HttpContext.Session.Remove("Admin_user_password");
            return Redirect("/Admin/Account/Login");
        }
    }
}
