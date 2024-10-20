using Microsoft.AspNetCore.Mvc;
using X.PagedList; // doi tuong phan trang
using project_ca_nhan.Models;
using project_ca_nhan.Areas.Admin.Attributes;
using project_ca_nhan.Models;
using Microsoft.EntityFrameworkCore;
using project_ca_nhan.Areas.Admin.Attributes;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckLogin]
    public class NewsController : Controller
    {
        public MyDbcontext db = new MyDbcontext();

        public IActionResult Index()
        {
            return RedirectToAction("Read");
        }
        public IActionResult Read(int? page)
        {
            int page_size = 4; // so ban ghi tren 1 trang
            int page_num = page ?? 1; // neu p null thi p bang 1 khong thi p = p 
            var list_record = db.News.OrderByDescending(x => x.Id).ToList(); // linq 
            return View("Read", list_record.ToPagedList(page_num, page_size));
        }
        public IActionResult Update(int? id)
        {

            int _id = id ?? 0;
            ViewBag.action = "/Admin/News/UpdatePost/" + _id;

            var record = db.News.FirstOrDefault(x => x.Id == id);
            return View("CreateUpdate", record);

        }
        [HttpPost]
        public IActionResult UpdatePost(int id, IFormCollection fc)
        {


            string _Name = fc["Name"].ToString().Trim();
            string _Description = fc["Description"].ToString().Trim();
            string _Content = fc["Content"].ToString().Trim();
           
            int _Hot = !String.IsNullOrEmpty(fc["Hot"]) ? 1 : 0;

            var record = db.News.Where(x => x.Id == id).FirstOrDefault();
            if (record != null)
            {
                record.Name = _Name; record.Description = _Description;
                record.Content = _Content;
                record.Hot = _Hot;
                db.SaveChanges();
                try
                {
                    if (!String.IsNullOrEmpty(Request.Form.Files[0].FileName))
                    {

                        string _Photo = Request.Form.Files[0].FileName;
                        _Photo = DateTime.Now.ToFileTime() + "_" + _Photo;
                        string _Path = Path.Combine("wwwroot/Upload/News/", _Photo);
                        //upload file
                        using (var stream = new FileStream(_Path, FileMode.Create))
                        {
                            Request.Form.Files[0].CopyTo(stream);
                        }
                        //cập nhật lại trường Photo của talbe News
                        record.Photo = _Photo;
                        db.SaveChanges();


                        // goi ham update category
                      
                    }
                }
                catch {; }
            }
        
            return Redirect("/Admin/News");
        }
  
      
        public IActionResult Create()
        {

            ViewBag.action = "/Admin/News/CreatePost";
            return View("CreateUpdate");

        }
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string _Name = fc["Name"].ToString().Trim();
            string _Description = fc["Description"].ToString().Trim();
            string _Content = fc["Content"].ToString().Trim();
            int _Hot = !String.IsNullOrEmpty(fc["Hot"]) ? 1 : 0;
            string _Photo = "";
            //---
            //kiểm tra ảnh để thực hiện upload ảnh
            try
            {
                if (!String.IsNullOrEmpty(Request.Form.Files[0].FileName))
                {
                    //return Content("ok");
                    //lấy tên file
                    _Photo = Request.Form.Files[0].FileName;
                    //thêm chuỗi để tạo ra tên file khác nhau
                    _Photo = DateTime.Now.ToFileTime() + "_" + _Photo;
                    //nối chuỗi "wwwroot/Upload/News/" và chuỗi ở biến _Photo để thành một đường dẫn hoàn chỉnh
                    string _Path = Path.Combine("wwwroot/Upload/News/", _Photo);
                    //upload file
                    using (var stream = new FileStream(_Path, FileMode.Create))
                    {
                        Request.Form.Files[0].CopyTo(stream);
                    }
                }
                //---
                var record = new ItemNews();
                if (record != null)
                {
                    record.Name = _Name;
                    record.Description = _Description;
                    record.Content = _Content;
                
                    record.Hot = _Hot;
                    record.Photo = _Photo;
                    //insert
                    db.News.Add(record);
                    db.SaveChanges();
                    int _ProductId = record.Id;
                    
                }
            }
            catch {; }
            //---
            //di chuyển đến một url
            return Redirect("/Admin/News");
        }
        public IActionResult Delete(int id)
        {
            var record = db.News.FirstOrDefault(x => x.Id == id);
            db.News.Remove(record);
            db.SaveChanges();

            return Redirect("/Admin/News");
        }
    }
}