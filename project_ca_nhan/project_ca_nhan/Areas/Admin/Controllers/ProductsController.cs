using Microsoft.AspNetCore.Mvc;
using X.PagedList; // doi tuong phan trang
using project_ca_nhan.Models;
using project_ca_nhan.Areas.Admin.Attributes;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckLogin]
    public class ProductsController : Controller
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
            var list_record = db.Products.OrderByDescending(x => x.Id).ToList(); // linq 
            return View("Read", list_record.ToPagedList(page_num, page_size));
        }
        public IActionResult Update(int? id)
        {

            int _id = id ?? 0;
            ViewBag.action = "/Admin/Products/UpdatePost/" + _id;

            var record = db.Products.FirstOrDefault(x => x.Id == id);
            return View("CreateUpdate", record);

        }
        [HttpPost]
        public IActionResult UpdatePost(int id, IFormCollection fc)
        {


            string _Name = fc["Name"].ToString().Trim();
            string _Description = fc["Description"].ToString().Trim();
            string _Content = fc["Content"].ToString().Trim();
            double _Price = Convert.ToDouble(fc["Price"].ToString());
            int _Hot = !String.IsNullOrEmpty(fc["Hot"]) ? 1 : 0;

            var record = db.Products.Where(x => x.Id == id).FirstOrDefault();
            if (record != null)
            {
                record.Name = _Name; record.Description = _Description;
                record.Content = _Content;
                record.Price = _Price;
                record.Hot = _Hot;
                db.SaveChanges();
                try
                {
                    if (!String.IsNullOrEmpty(Request.Form.Files[0].FileName))
                    {

                        string _Photo = Request.Form.Files[0].FileName;
                        _Photo = DateTime.Now.ToFileTime() + "_" + _Photo;
                        string _Path = Path.Combine("wwwroot/Upload/Products/", _Photo);
                        //upload file
                        using (var stream = new FileStream(_Path, FileMode.Create))
                        {
                            Request.Form.Files[0].CopyTo(stream);
                        }
                        //cập nhật lại trường Photo của talbe Products
                        record.Photo = _Photo;
                        db.SaveChanges();


                        // goi ham update category
                        CreateUpdateCategoriesProducts(id);
         
                    }
                }
                catch {; }
            }
            CreateUpdateCategoriesProducts(id);
            CreateUpdateTagsProducts(id);
            return Redirect("/Admin/Products");
        }
        public void CreateUpdateCategoriesProducts(int _ProductId)
        {
            List<string> categories = Request.Form["Categories"].ToList();

            List<ItemCategoryProduct> list = db.CategoriesProducts.Where(x => x.ProductId == _ProductId).ToList();
            foreach (var item in list)
            {
                db.CategoriesProducts.Remove(item);
                db.SaveChanges();
            }

            foreach (string category in categories)
            {
                int _CategoryId = Convert.ToInt32(category);
                ItemCategoryProduct record = new ItemCategoryProduct();
                record.CategoryId = _CategoryId;
                record.ProductId = _ProductId;
                db.CategoriesProducts.Add(record);
                db.SaveChanges();
            }

        }
        public void CreateUpdateTagsProducts(int _ProductId)
        {
            //lấy giá trị của biến form có name=Categories
            List<string> tags = Request.Form["Tags"].ToList();
            //xóa hết các bản ghi tương ứng với _ProductId
            List<ItemTagProduct> list_tags_products = db.TagsProducts.Where(item => item.ProductId == _ProductId).ToList();
            foreach (var item in list_tags_products)
            {
                db.TagsProducts.Remove(item);
                db.SaveChanges();
            }
            //---
            foreach (string tag in tags)
            {
                int _TagId = Convert.ToInt32(tag);
                //thêm mới bản ghi vào table CategoriesProducts
                ItemTagProduct record = new ItemTagProduct();
                record.ProductId = _ProductId;
                record.TagId = _TagId;
                db.TagsProducts.Add(record);
                db.SaveChanges();
            }
        }
        public IActionResult Create()
        {

            ViewBag.action = "/Admin/Products/CreatePost";
            return View("CreateUpdate");

        }
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string _Name = fc["Name"].ToString().Trim();
            string _Description = fc["Description"].ToString().Trim();
            string _Content = fc["Content"].ToString().Trim();
            double _Price = Convert.ToDouble(fc["Price"].ToString());
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
                    //nối chuỗi "wwwroot/Upload/Products/" và chuỗi ở biến _Photo để thành một đường dẫn hoàn chỉnh
                    string _Path = Path.Combine("wwwroot/Upload/Products/", _Photo);
                    //upload file
                    using (var stream = new FileStream(_Path, FileMode.Create))
                    {
                        Request.Form.Files[0].CopyTo(stream);
                    }
                }
                //---
                var record = new ItemProduct();
                if (record != null)
                {
                    record.Name = _Name;
                    record.Description = _Description;
                    record.Content = _Content;
                    record.Price = _Price;
                    record.Hot = _Hot;
                    record.Photo = _Photo;
                    //insert
                    db.Products.Add(record);
                    db.SaveChanges();
                    int _ProductId = record.Id;
                    CreateUpdateCategoriesProducts(_ProductId);
                    CreateUpdateTagsProducts(_ProductId);
                }
            }
            catch {; }
            //---
            //di chuyển đến một url
            return Redirect("/Admin/Products");
        }
        public IActionResult Delete(int id)
        {
            var record = db.Products.FirstOrDefault(x => x.Id == id);
            db.Products.Remove(record);
            db.SaveChanges();

            return Redirect("/Admin/Products");
        }
    }
}