using Microsoft.AspNetCore.Mvc;
using BC = BCrypt.Net;
using X.PagedList;
using project_ca_nhan.Models;
using project_ca_nhan.Areas.Admin.Attributes;
using System.Security.Cryptography;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
namespace project_ca_nhan.Controllers
{
    public class AccountController : Controller
    {
        public MyDbcontext db = new MyDbcontext();
  

        public IActionResult Register()
        {
            string namex = TempData["namex"]!=null ? TempData["namex"].ToString() : "";
            string emailx = TempData["emailx"] != null ? TempData["emailx"].ToString() : "";
            string alert = TempData["alert"] != null ? TempData["alert"].ToString() : "";
            ViewBag.namex = namex;
            ViewBag.emailx = emailx;
            ViewBag.action = "/Account/RegisterPost";
            ViewBag.alert = alert;
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult RegisterPost(IFormCollection fc)
        {
            string _Name = fc["Name"].ToString();
            string _Email = fc["Email"].ToString();
            string _Phone = fc["Phone"].ToString();
            string _Address = fc["Address"].ToString();
            string _Password = fc["Password"].ToString();
           
            var check_email = db.Customers.FirstOrDefault(s => s.Email == _Email);
            if (check_email == null)
            {


                TempData["Name"] = _Name;
                TempData["Email"] = _Email;
                TempData["Phone"] = _Phone;
                TempData["Address"] = _Address;
                TempData["Password"] = _Password;


               
                return Redirect("/Account/SendMail");

            }
            return Redirect("/Account/Register?notify=false");


        }
        public IActionResult SendMail()
        {
            string _Name = TempData["Name"] != null ? TempData["Name"].ToString() : "";
            string _Email = TempData["Email"] != null ? TempData["Email"].ToString() : "";
            string _Address = TempData["Address"] != null ? TempData["Address"].ToString() : "";
            string _Phone = TempData["Phone"] != null ? TempData["Phone"].ToString() : "";
            string _Password = TempData["Password"] != null ? TempData["Password"].ToString() : "";

            ViewBag.action = "/Account/SendMailPost";
            Random ran = new Random();
            int random = ran.Next(100000, 1000000);

            var FromAddress = new MailAddress("hoangthaisonqs@gmail.com", "Shop Mail");
            var ToAddress = new MailAddress(_Email, "Khach Hang");
            const string formpassword = "mobm riko ijaw spac";
            string body = $@"
        <html>
        <body>
            <h1 style='color: blue;'>Xin chào bạn {_Name}</h1>
            <p>Tôi có gửi cho bạn email này để xác nhận Email của bạn:</p>
            <p>Bạn hãy lấy 6 chữ số dưới đây để hoàn tất thủ tục đăng kí:</p>
            <p><strong>{random}</strong></p>
        </body>
        </html>";
            const string subject = "Đăng Nhập Shop Mail";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(FromAddress.Address, formpassword),
                Timeout = 20000,
            };

            using (var message = new MailMessage(FromAddress, ToAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            })
            {
                smtp.Send(message);
            }

            TempData["Random"] = random;
            TempData.Keep();

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]

        public IActionResult SendMailPost(IFormCollection fc)
        {
            int maxacnhan = Convert.ToInt32(fc["maxacnhan"]);
            int savedRandom = TempData["Random"] != null ? Convert.ToInt32(TempData["Random"]) : 0;

            if (maxacnhan == savedRandom)
            {
                ItemCustomer x = new ItemCustomer();
                x.Email = TempData["Email"] != null ? TempData["Email"].ToString() : "";
                x.Address = TempData["Address"] != null ? TempData["Address"].ToString() : "";
                x.Phone = TempData["Phone"] != null ? TempData["Phone"].ToString() : "";
                x.Name = TempData["Name"] != null ? TempData["Name"].ToString() : "";
                x.Password = TempData["Password"] != null ? BC.BCrypt.HashPassword(TempData["Password"].ToString()) : "";

                db.Customers.Add(x);
                db.SaveChanges();
                return Redirect("/Account/Login");
            }

            ViewBag.Error = "Mã xác nhận không đúng. Vui lòng thử lại.";
            return View("SendMail");
        }

        public IActionResult Login()
        {
            ViewBag.action = "/Account/LoginPost";
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult LoginPost(IFormCollection fc)
        {
            string _Email = fc["Email"].ToString();
            string _Password = fc["Password"].ToString();
            ItemCustomer check_email = db.Customers.FirstOrDefault(s => s.Email == _Email);
            if(check_email != null)
            {
                // kiem tra den phan mat khau
                if (BC.BCrypt.Verify(_Password, check_email.Password))
                {
                    HttpContext.Session.SetString("customer_user_id",check_email.Id.ToString());
                    HttpContext.Session.SetString("customer_user_email", check_email.Email.ToString());
                    return Redirect("/Home/Index");
                }
                else
                {
                    return Redirect("/Account/Login?notify=invalid-email");
                }
            }
            
            else
                return Redirect("/Account/Login?notify=invalid-email");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("customer_user_id");
            HttpContext.Session.Remove("customer_user_email");
            return Redirect("/Home");
        }
        public IActionResult EditProfile()
        {
            ViewBag.action = "/Account/EditProfilePost";
            return View("EditProfile");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult EditProfilePost(IFormCollection fc)
        {
            string _Name = fc["Name"].ToString();
            string _Email = fc["Email"].ToString();
            string _Phone = fc["Phone"].ToString();
            string _Address = fc["Address"].ToString();
            string _Password = fc["Password"].ToString();

            ItemCustomer x = db.Customers.FirstOrDefault(s => s.Email == HttpContext.Session.GetString("customer_user_email"));

            x.Name = _Name;
            x.Address = _Address;
            x.Phone = _Phone;
            if (!String.IsNullOrEmpty(_Password))
            {
                x.Password = BC.BCrypt.HashPassword(_Password);
            }
           
            
            
            db.SaveChanges();
            return Redirect("/Account/EditProfile");
        }
        public IActionResult Order()
        {
            int k = Convert.ToInt32(HttpContext.Session.GetString("customer_user_id"));
            // kiem tra xem no da dang nhap hay chua 
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("customer_user_id")))
            {
                List<ItemOrder> x = db.Orders.Where(s => s.CustomerId == k).ToList();
                string trangthai = !String.IsNullOrEmpty("trangthai") ? Request.Query["trangthai"] : "";
                switch (trangthai)
                {
                    case "dathang":
                        x = db.Orders.Where(s => s.Status == 0).OrderByDescending(s=>s.Id).ToList();
                        break;
                    case "nhanhang":
                        x = db.Orders.Where(s => s.Status == 2).OrderByDescending(s => s.Id).ToList();
                        break;
                    case "danggiao":
                        x = db.Orders.Where(s => s.Status == 1).OrderByDescending(s => s.Id).ToList();
                        break;
                    default:
                        x = db.Orders.ToList();
                        break;
                }
                ViewBag.donhangId = trangthai;
                return View("Order",x);
            }
            else
            {
                return Redirect("/Account/Login");
            }
        }
 
        public IActionResult Orders()
        {
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("customer_user_id")))
                return Redirect("/Account/Login");
            List<ItemOrder> list_record = db.Orders.OrderByDescending(item => item.Id).ToList();
            return View("Orders", list_record);
        }
        public IActionResult OrderDetail(int? id)
        {
            int _OrderId = id ?? 0;
            ViewBag.OrderId = _OrderId;
            List<ItemOrderDetail> _ListRecord = db.OrdersDetail.Where(tbl => tbl.OrderId == _OrderId).ToList();
            return View("OrderDetail", _ListRecord);
        }
        public IActionResult CancelOrder(int id)
        {
            ItemOrder record = db.Orders.FirstOrDefault(item => item.Id == id);
            record.Status = 2; //2 là trạng thái hủy đơn hàng
            db.SaveChanges();
            return Redirect("/Account/Orders");
        }
    }
}
