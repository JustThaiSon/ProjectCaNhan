using Microsoft.AspNetCore.Mvc;
using BC = BCrypt.Net;
// doi tuong phan trnag
using X.PagedList;
// de lay cac file trong thu muc models
using project_ca_nhan.Models;
using project_ca_nhan.Areas.Admin.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage;
using System.Net.Mail;
using System.CodeDom;
using System;
using System.Net;
using System.Reflection;
using System.Net.Sockets;
using Newtonsoft.Json;
namespace project_ca_nhan.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckLogin]
    public class UsersController : Controller
    {
        // khoi tao doi tuong de thao tac csdl
        public MyDbcontext db = new MyDbcontext();
        public IActionResult Index()
        {
            // di chuyen den ham read
            return RedirectToAction("Read","Users");
        }
        public IActionResult Read(int ? page)
        {
            // thiet lap so ban ghi trong 1 trang
            int page_size = 4;
            // thiet lap trang hien tai
            int page_number = page ?? 1;
            // su dung linq de lay ban ghi
            var list_record = db.Users.OrderByDescending(item => item.Status).ToList();
            return View("Read",list_record.ToPagedList(page_number,page_size));
        }
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            ViewBag.action = "/Admin/Users/UpdatePost/" + _id;
            var record = db.Users.FirstOrDefault(item => item.Id == id);
            return View("CreateUpdate", record);
        }
        [HttpPost]

        public IActionResult UpdatePost(int id, IFormCollection fc)
        {
            string _Name = fc["Name"].ToString().Trim();
            string _Email = fc["Email"].ToString().Trim();
            string _Password = fc["Password"].ToString().Trim();
            var record = db.Users.FirstOrDefault(item => item.Id == id);
            if (record != null)
            {
                record.Name = _Name;
                record.Email = _Email;
            }
            
            if (!String.IsNullOrEmpty(_Password))
            {
                _Password = BC.BCrypt.HashPassword(_Password);
                record.Password = _Password;
            }
            // cap nhat lai csdl
            db.SaveChanges();
            return Redirect("/Admin/Users");
        }

        public IActionResult Create()
        {
            // tim kim xem session o day la static nao
            ItemUsers x = db.Users.Where(s => s.Email == HttpContext.Session.GetString("Admin_user_email")).FirstOrDefault();
            if(x!= null)
            {
                if(x.Status == 0)
                {
                    return Redirect("/Admin/Error");
                }
                if(x.Status == 1 || x.Status == 2)
                {
                    ViewBag.action = "/Admin/Users/CreatePost";
                    return View("CreateUpdate");
                }
            }
            ViewBag.err2 = "Công Tác Viên Không Được Thực hiện Chức Năng Này, vui lòng liên hệ đến Admin";
            TempData["err2"] = ViewBag.err2;
                return Redirect("/Admin/Error");
        }
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string _Name = fc["Name"].ToString().Trim();
            string _Email = fc["Email"].ToString().Trim();
            string _Password = fc["Password"].ToString().Trim();
            _Password = BC.BCrypt.HashPassword(_Password);
            int _static = !String.IsNullOrEmpty(Request.Form["static"]) ? Convert.ToInt32(Request.Form["static"]) : -1;
            if(_static != -1)
            {
                string x = "";
                ViewBag.Static = _static;
                if(ViewBag.Static == 0)
                {
                    x = "Cộng Tác Viên";
                }
                if(ViewBag.Static == 1)
                {
                    x = "Admin Phụ";
                }
                if (ViewBag.Static == 2)
                {
                    x = "QUản Trị Trang Web (Admin Chính)";
                }

                // gui tin nhan de xac nhan email nay la chinh xac
                Random r = new Random();
                int random = r.Next(100000, 1000000);
                var FromAddress = new MailAddress("vutung88889@gmail.com", "Lời Nhắn Từ Admin Cửa Hàng FlatShop");

                var ToAddress = new MailAddress(_Email, _Name);
                const string FromPassword = "mobm riko ijaw spac";
                const string Subject = "Xin Chúc Mừng Bạn Đã Tham Gia Điều Hành FlatShop";
                string Body = $@"
                        <html>
                            <body>
                            <h1 style='color: blue;'>Xin chào bạn {_Name}</h1>
                            <p>Tôi có gửi cho bạn email này để thông báo việc bạn đã tham gia hệ thống của hàng của chúng tôi với vai trò là {x}, tôi hi vọng bạn sẽ đóng góp , xây dựng và phát triển trang web flatshop này cùng chúng tôi</p>
                            
                            <p>Xin Vui Lòng Nhập 6 số Dưới Đây Để Hoàn Tất Chương Trình Đăng Kí Của Bạn</p>
                            <p><strong> {random}</strong></p>
<p>Xin Vui Lòng Liên Hệ Lại Với Admin Chính Của Hệ Thống , nếu có bất kì thắc mắc nào</p>
                            <p><strong>Contact:  Email: vutung88889@gmail.com Phone: 0941208572</strong></p>
                            </body>

                        </html>
                ";
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(FromAddress.Address, FromPassword),
                    Timeout = 10000,
                };
                using(var message = new MailMessage(FromAddress, ToAddress)
                {
                    Subject = Subject,
                    Body = Body,
                    IsBodyHtml = true,
                })
                {
                    smtp.Send(message);
                }
                //// khoi tao mot doi tuong moi
                var doituong = new ItemUsers();
                doituong.Name = _Name;
                doituong.Email = _Email;
                doituong.Password = _Password;
                doituong.Status = _static;

                // khoi tao bien session user
                var json = JsonConvert.SerializeObject(doituong);
                HttpContext.Session.SetString("user", json);
                // khoi tao bien seesion cho random 
                var p = JsonConvert.SerializeObject(random);
                HttpContext.Session.SetString("random", p);
                return Redirect("/Admin/Users/SendMail");
                //// them ban ghi vao doituong
                //db.Users.Add(doituong);
                //db.SaveChanges();
                //return Redirect("/Admin/Users");
                
            }
            ViewBag.err3 = "Không Tìm Thấy Static với biến Request.Query";
            TempData["ViewBag.err3"] = ViewBag.err3;
            return Redirect("/Admin/Error");
            
            
        }

        public IActionResult SendMail()
        {
            string so = TempData["so"] != null ? TempData["so"].ToString() : "";
            ViewBag.err = so;
            ViewBag.action = "/Admin/Users/SendMailPost";
            return View();
        }
        [HttpPost,ValidateAntiForgeryToken]
        public IActionResult SendMailPost()
        {
            string user12 = HttpContext.Session.GetString("user");
            string random = HttpContext.Session.GetString("random");
            if (!String.IsNullOrEmpty(user12) && !String.IsNullOrEmpty(random))
            {
                ItemUsers chuyen = JsonConvert.DeserializeObject<ItemUsers>(user12);
                int ran = JsonConvert.DeserializeObject<int>(random);
                int number = !String.IsNullOrEmpty(Request.Form["Number"]) ? Convert.ToInt32(Request.Form["Number"]) : 0;
                if(number == ran)
                {
                    ItemUsers add = new ItemUsers();
                    add.Name = chuyen.Name;
                    add.Email = chuyen.Email;
                    add.Status = chuyen.Status;
                    add.Password = BC.BCrypt.HashPassword(chuyen.Password);
                    db.Users.Add(add);
                    db.SaveChanges();
                    return Redirect("/Admin/Users");
                }
                else
                {
                    TempData["so"] = "Số Nhập Vào Không Đúng , Vui Lòng Thử Lại";
                    return View("/Admin/Sendmail");
                }
            }
            TempData["err5"] = "Không Bắt Được Biến Session của User và Số Random";
            return Redirect("/Admin/Error");


        }
        public IActionResult Delete(int ? id)
        {
            // tim vi tri cua status
            ItemUsers vt = db.Users.Where(s => s.Email == HttpContext.Session.GetString("Admin_user_email")).FirstOrDefault();

            // status = 2 cos the xoa status = 0 va 1 
            if(vt.Status == 2)
            {
                var timkiem = db.Users.Where(s => s.Id == id).FirstOrDefault();
               
                    if (timkiem != null)
                    {
                        if (timkiem.Status == 1 || timkiem.Status == 0)
                    {
                        db.Users.Remove(timkiem);
                        db.SaveChanges();
                        ItemUsers x = db.Users.Where(x => x.Status == 2).FirstOrDefault();
                        var FromAddress = new MailAddress("vutung88889@gmail.com", "Shop Mail");
                        var ToAddress = new MailAddress(timkiem.Email, timkiem.Name);
                        const string FromPassword = "mobm riko ijaw spac";
                        const string subject = "Bạn Đã Bị Xóa Khỏi Hệ Thống Admin";
                        string Body = $@"
                        <html>
                            <body>
                            <h1 style='color: blue;'>Xin chào bạn {timkiem.Name}</h1>
                            <p>Tôi có gửi cho bạn email này để thông báo việc bạn đã bị xóa khỏi hệ thống Admin</p>
                            <p>Xin Vui Lòng Liên Hệ Lại Với Admin Chính Của Hệ Thống , nếu có bất kì thắc mắc nào</p>
                            <p><strong>Contact:  Email {x.Email} Name: {x.Name}  </strong></p>
                            </body>

                        </html>
                        ";
                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                           
                            Credentials = new NetworkCredential(FromAddress.Address, FromPassword),
                            Timeout = 20000,

                        };
                        using (var message = new MailMessage(FromAddress, ToAddress)
                        {
                            Subject = subject,
                            Body = Body,
                            IsBodyHtml = true,
                        })
                        {
                            smtp.Send(message);
                        }


                    }
                }
            }
            // status = 1 co the xoa status = 0
            if (vt.Status == 1)
            {
                var timkiem = db.Users.Where(s => s.Id == id).FirstOrDefault();

                if (timkiem != null)
                {
                    if (timkiem.Status == 0)
                    {
                        db.Users.Remove(timkiem);
                        db.SaveChanges();
                        ItemUsers x = db.Users.Where(x => x.Status == 2).FirstOrDefault();
                        var FromAddress = new MailAddress("vutung88889@gmail.com", "Shop Mail");
                        var ToAddress = new MailAddress(timkiem.Email, timkiem.Name);
                        const string FromPassword = "mobm riko ijaw spac";
                        const string Subject = "Bạn Đã Bị Xóa Khỏi Hệ Thống Admin";
                        string Body = $@"
<html>
<header>
<style> p{{ font-weight: bold; color: red }} </style></header>
<body>
 <h1 style='color: blue;'>Xin chào bạn {timkiem.Name}</h1>
            <p>Tôi có gửi cho bạn email này để thông báo việc bạn đã bị xóa khỏi hệ thống Admin</p>
            <p>Xin Vui Lòng Liên Hệ Lại Với Admin Chính Của Hệ Thống , nếu có bất kì thắc mắc nào</p>
            <p><strong>Contact:  Email {x.Email} Name: {x.Name}  </strong></p>
</body>

</html>
                        ";
                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            Credentials = new NetworkCredential(FromAddress.Address, FromPassword),
                            Timeout = 20000,

                        };
                        using (var message = new MailMessage(FromAddress, ToAddress)
                        {
                            Subject = Subject,
                            Body = Body,
                            IsBodyHtml = true,
                        })
                        {
                            smtp.Send(message);
                        }

                    }
                }
            }
            // status = 0 khong the xoa bat ki thu gi , va dua ve trang error
            if (vt.Status == 0)
            {
                ViewBag.err1 = "Do Bạn Là Cộng Tác Viên Nên Không Thể Thực Hiện Thao Tác Này";
                TempData["err1"] = ViewBag.err1;
                return Redirect("/Admin/Error");
            }
           
           
            return Redirect("/Admin/Users");
        }


    }
}
