using Microsoft.AspNetCore.Mvc;
using project_ca_nhan.Models;
using Newtonsoft.Json;
namespace project_ca_nhan.Controllers
{
    public class ContactController : Controller
    {
        public MyDbcontext db= new MyDbcontext();
        public IActionResult Contact()
        {
            ViewBag.error = TempData["error"];
            ViewBag.action = "/Contact/ContactPost";
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ContactPost()
        {
            string mess = !String.IsNullOrEmpty(Request.Form["Message"]) ? Request.Form["Message"] : "";
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("customer_user_id")))
            {
                int customerid = Convert.ToInt32(HttpContext.Session.GetString("customer_user_id"));

                ItemRespond x = new ItemRespond();
                x.CustomerId = customerid;
                x.Date = DateTime.Now;
                x.Message = mess;
                db.Add(x);
                db.SaveChanges();
                return View("RespondSuccess", x);
            }
            else
            {

                string Name = !String.IsNullOrEmpty(Request.Form["Name"]) ? Request.Form["Name"] : "";
                string Email = !String.IsNullOrEmpty(Request.Form["Email"]) ? Request.Form["Email"] : "";
                ItemCustomer k = db.Customers.Where(s=>s.Email ==  Email).FirstOrDefault();
                if(k!= null){
                    string l = "Email "+ HttpContext.Session.GetString("customer_user_email")+ " Đã bị Trùng Vui Lòng Nhập Lại";
                    TempData["error"] = l;
                    return Redirect("/Contact/Contact");
                }
                else
                {
                    ViewBag.alert = "alert('Do bạn chưa đăng nhập nên không thể bình luận.');";
                    TempData["namex"] = Name;
                    TempData["emailx"] = Email;
                    TempData["alert"] = ViewBag.alert;
                    return Redirect("/Account/Register");
                }
               
            }
        }
       
    }
}
