using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using project_ca_nhan.Models;
using project_ca_nhan.MyClass;
using X.PagedList;

namespace project_ca_nhan.Controllers
{
    public class ProductController : Controller
    {
        public MyDbcontext db = new MyDbcontext();
        //danh mục sản phẩm
        public IActionResult Categories(int? id, int? page, string getnumber, string orderby)
        {
            int _CategoryId = id ?? 0;
            //return Content(_CategoryId.ToString());
            //truyền biến CategoryId ra bên ngoài view
            ViewBag.CategoriesId = _CategoryId;
            List<ItemProduct> list_record = new List<ItemProduct>();
            //lấy các sản phẩm trong trường hợp không có CategoryId
            if(_CategoryId == 0)
            {
                list_record = db.Products.OrderByDescending(item => item.Id).ToList();
            }
           
            //nếu có CategoryId thì thêm một bước truy vấn linq để lọc theo CategoryId
            if (_CategoryId != 0)
            {
                list_record = (from product in db.Products join category_product in db.CategoriesProducts on product.Id equals category_product.ProductId join category in db.Categories on category_product.CategoryId equals category.Id where category.Id == _CategoryId select product).OrderByDescending(item => item.Id).ToList();
                //lấy giá trị của biến orderby truyền từ url


                if (!String.IsNullOrEmpty("orderby"))
                {
                    //truyền biến orderby ra ngoài view
                    switch (orderby)
                    {
                        case "Name-asc":
                            list_record = list_record.OrderBy(item => item.Name).ToList();
                            break;
                        case "Name-desc":
                            list_record = list_record.OrderByDescending(item => item.Name).ToList();
                            break;
                        case "Price-asc":
                            list_record = list_record.OrderBy(item => item.Price).ToList();
                            break;
                        case "Price-desc":
                            list_record = list_record.OrderByDescending(item => item.Price).ToList();
                            break;
                    }
                }

                ViewBag.orderby = orderby;

            }

            int page_size = 9;
            int page_number = page ?? 1;
            // phan trang de sap xep 
            return View("Categories", list_record.ToPagedList(page_number, page_size));
        }
        //chi tiết sản phẩm
        public IActionResult Detail(int? id)
        {
            int _id = id ?? 0;
            ItemProduct record = db.Products.Where(item => item.Id == _id).FirstOrDefault();
            ViewBag.detailId = _id;
            return View("Detail", record);
        }
        //Đánh giá số sao (Rate)
        public IActionResult Rate(int? id)
        {
            int _id = id ?? 0;
            int _star = !String.IsNullOrEmpty(Request.Query["star"]) ? Convert.ToInt32(Request.Query["star"]) : 0;
            //thêm một bản ghi vào table Rating
            ItemRating record = new ItemRating();
            record.ProductId = _id;
            record.Star = _star;
            db.Rating.Add(record);
            db.SaveChanges();
            //di chuyển đến url chi tiết sản phẩm
            return Redirect("/Products/Detail/" + _id);
        }
        //tìm kiếm theo mức giá
        public IActionResult SearchPrice(int? page)
        {
            double FromPrice = !String.IsNullOrEmpty(Request.Query["FromPrice"]) ? Convert.ToDouble(Request.Query["FromPrice"]) : 0;
            double ToPrice = !String.IsNullOrEmpty(Request.Query["ToPrice"]) ? Convert.ToDouble(Request.Query["ToPrice"]) : 0;
            //---
            int pageSize = 9;
            int pageNumber = page ?? 1;
            List<ItemProduct> list_record = db.Products.Where(item => item.Price >= FromPrice && item.Price <= ToPrice).ToList();
            //---
            ViewBag.FromPrice = FromPrice;
            ViewBag.ToPrice = ToPrice;
            //---
            return View("SearchPrice", list_record.ToPagedList(pageNumber, pageSize));
        }
        //tag
        public IActionResult Tags(int? id, int? page)
        {
            int _id = id ?? 0;
            //---
            int pageSize = 9;
            int pageNumber = page ?? 1;
            //---
            ViewBag.tagId = _id;
            //---
            List<ItemProduct> list_record = new List<ItemProduct>();
            if (_id == 0)
            {
                //lấy các sản phẩm trong trường hợp không có CategoryId
                list_record  = db.Products.OrderByDescending(item => item.Id).ToList();
                //nếu có CategoryId thì thêm một bước truy vấn linq để lọc theo CategoryId
            }

            if (_id != 0)
            {


                list_record = (from product in db.Products join tag_product in db.TagsProducts on product.Id equals tag_product.ProductId join tag in db.Tags on tag_product.TagId equals tag.Id where tag.Id == _id select product).OrderByDescending(item => item.Id).ToList();
                //---
                string orderby = !String.IsNullOrEmpty("orderby") ? Request.Query["orderby"] : "";
                //truyền biến orderby ra ngoài view

                switch (orderby)
                {
                    case "Name-asc":
                        list_record = list_record.OrderBy(item => item.Name).ToList();
                        break;
                    case "Name-desc":
                        list_record = list_record.OrderByDescending(item => item.Name).ToList();
                        break;
                    case "Price-asc":
                        list_record = list_record.OrderBy(item => item.Price).ToList();
                        break;
                    case "Price-desc":
                        list_record = list_record.OrderByDescending(item => item.Price).ToList();
                        break;
                }
                ViewBag.orderby = orderby;
                // - sap xep theo lay san pham 
                string getnumber = !String.IsNullOrEmpty("getnumber") ? Request.Query["getnumber"] : "";
                //truyền biến orderby ra ngoài view

                switch (getnumber)
                {
                    case "3":
                        list_record = list_record.Skip(0).Take(3).ToList();
                        break;
                    case "6":
                        list_record = list_record.Take(6).ToList();
                        break;
                    case "9":
                        list_record = list_record.Skip(0).Take(9).ToList();
                        break;
                    case "12":
                        list_record = list_record.Skip(0).Take(12).ToList();
                        break;
                }
                ViewBag.getnumber = getnumber;
            }
           

            return View("Tags", list_record.ToPagedList(pageNumber, pageSize));
        }
        //search name
        public IActionResult SearchName(int? page)
        {
            string key = !String.IsNullOrEmpty(Request.Query["key"]) ? Request.Query["key"] : "";
            //---
            int pageSize = 9;
            int pageNumber = page ?? 1;
            List<ItemProduct> list_record = db.Products.Where(item => item.Name.Contains(key)).ToList();
            //---
            ViewBag.key = key;
            // -- 
            
            //---
            return View("SearchName", list_record.ToPagedList(pageNumber, pageSize));
        }
        //ajax search
        public IActionResult SearchAjax()
        {
            string key = !String.IsNullOrEmpty(Request.Query["key"]) ? Request.Query["key"] : "";
            List<ItemProduct> list_record = db.Products.Where(item => item.Name.Contains(key)).ToList();
            string str = "";
            foreach (var item in list_record)
            {
                str += "<li><img src=\"/Upload/Products/" + item.Photo + "\" /><a href=\"/Products/Detail/" + item.Id + "\">" + item.Name + "</a></li>";
            }
            return Content(str);
        }

        //public IActionResult SearchCategires(int? page)
        //{
        //    int categories = !String.IsNullOrEmpty(Request.Query["categories"]) ? Convert.ToInt32(Request.Query["categories"]) : 0;
        //    string keyword = !String.IsNullOrEmpty(Request.Query["keyword"]) ? Request.Query["keyword"] : "";
        //    //---

        //    int pageSize = 9;
        //    int pageNumber = page ?? 1;
        //    List<ItemProduct> list_record = (from product in db.Products join cate_product in db.CategoriesProducts on product.Id equals cate_product.ProductId join cate in db.Categories on cate_product.CategoryId equals cate.Id where cate.Id == categories && product.Name.Contains(keyword) select product).OrderByDescending(item => item.Id).ToList();
        //    ViewBag.key = keyword;
        //    ViewBag.CateId = categories;
        //    return View("SearchCategires", list_record.ToPagedList(pageNumber, pageSize));
        //}

        public IActionResult SearchCategires(int? page)
        {
            // Lấy giá trị categories và keyword từ URL
            int categories = !String.IsNullOrEmpty(Request.Query["categories"]) ? Convert.ToInt32(Request.Query["categories"]) : 0;
            string keyword = !String.IsNullOrEmpty(Request.Query["keyword"]) ? Request.Query["keyword"] : "";

            // Thiết lập kích thước trang và số trang mặc định
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var list_record = new List<ItemProduct>();
            // Thực hiện truy vấn LINQ để lấy danh sách sản phẩm theo category và keyword
            if(categories == 0)
            {
                list_record = db.Products.Where(s => s.Name.Contains(keyword)).ToList();
            }
            else
            {
                list_record = (from product in db.Products
                            join cate_product in db.CategoriesProducts on product.Id equals cate_product.ProductId
                            join cate in db.Categories on cate_product.CategoryId equals cate.Id
                            where cate.Id == categories && product.Name.Contains(keyword)
                            select product).OrderByDescending(item => item.Id).ToList();
            }
            

            // Sắp xếp theo Id giảm dần và lấy trang hiện tại
            

            // Truyền dữ liệu cần thiết đến view
            ViewBag.key = keyword;
            ViewBag.CateId = categories;

            // Trả về view với danh sách sản phẩm đã phân trang
            return View("SearchCategires", list_record.ToPagedList(pageNumber,pageSize));
        }

        public IActionResult SearchSanPham()
        {
            string keyword = !String.IsNullOrEmpty(Request.Query["keyword"]) ? Request.Query["keyword"] : "";
            int categories = !String.IsNullOrEmpty(Request.Query["categories"]) ? Convert.ToInt32(Request.Query["categories"]) : 0;
            List<ItemProduct> list_record = (from product in db.Products join cate_product in db.CategoriesProducts on product.Id equals cate_product.ProductId join cate in db.Categories on cate_product.CategoryId equals cate.Id where cate.Id == categories && product.Name.Contains(keyword)  select product).OrderByDescending(item => item.Id).ToList();
            string str = "";
            foreach (var item in list_record)
            {
                str += "<li><img src=\"/Upload/Products/" + item.Photo + "\" /><a href=\"/Products/Detail/" + item.Id + "\">" + item.Name + "</a></li>";
            }
            return Content(str);
        }
        public IActionResult Men(int? page)
        {
            // Lấy danh mục có tên chứa "Nam" và Id là 0
            ItemCategory x = db.Categories.Where(s =>s.Name.Contains("Nam") && s.ParentId == 0).FirstOrDefault();

            // Kiểm tra nếu x là null
            if (x == null)
            {
                return Content("Không tìm thấy danh mục phù hợp.");
            }

            string _CategoryId = x.Id.ToString();
            ViewBag.CategoriesId = _CategoryId;

            // Khởi tạo danh sách sản phẩm
            List<ItemProduct> them = new List<ItemProduct>();

            int _parentId = x.ParentId;

            if (_parentId == 0)
            {
                // Lấy danh sách các danh mục con
                List<ItemCategory> danhsach = db.Categories.Where(s => s.ParentId == x.Id).ToList();
                foreach (var p in danhsach)
                {
                    List<ItemProduct> duyet = (from product in db.Products
                                               join category_product in db.CategoriesProducts on product.Id equals category_product.ProductId
                                               join category in db.Categories on category_product.CategoryId equals category.Id
                                               where category.Id == p.Id
                                               select product).ToList();

                    them.AddRange(duyet);
                }
            }
            else
            {
                // Nếu không có danh mục con thì lấy tất cả sản phẩm thuộc danh mục gốc
                them = (from product in db.Products
                        join category_product in db.CategoriesProducts on product.Id equals category_product.ProductId
                        join category in db.Categories on category_product.CategoryId equals category.Id
                        where category.Id == x.Id
                        select product).ToList();
            }

            // Phân trang
            int page_size = 9;
            int page_number = page ?? 1;

            return View("Men", them.ToPagedList(page_number, page_size));
        }
        public IActionResult Women(int? page)
        {
            // Lấy danh mục có tên chứa "Nam" và Id là 0
            ItemCategory x = db.Categories.Where(s => s.Name.Contains("Nữ") && s.ParentId == 0).FirstOrDefault();

            // Kiểm tra nếu x là null
            if (x == null)
            {
                return Content("Không tìm thấy danh mục phù hợp.");
            }

            string _CategoryId = x.Id.ToString();
            ViewBag.CategoriesId = _CategoryId;

            // Khởi tạo danh sách sản phẩm
            List<ItemProduct> them = new List<ItemProduct>();

            int _parentId = x.ParentId;

            if (_parentId == 0)
            {
                // Lấy danh sách các danh mục con
                List<ItemCategory> danhsach = db.Categories.Where(s => s.ParentId == x.Id).ToList();
                foreach (var p in danhsach)
                {
                    List<ItemProduct> duyet = (from product in db.Products
                                               join category_product in db.CategoriesProducts on product.Id equals category_product.ProductId
                                               join category in db.Categories on category_product.CategoryId equals category.Id
                                               where category.Id == p.Id
                                               select product).ToList();

                    them.AddRange(duyet);
                }
            }
            else
            {
                // Nếu không có danh mục con thì lấy tất cả sản phẩm thuộc danh mục gốc
                them = (from product in db.Products
                        join category_product in db.CategoriesProducts on product.Id equals category_product.ProductId
                        join category in db.Categories on category_product.CategoryId equals category.Id
                        where category.Id == x.Id
                        select product).ToList();
            }

            // Phân trang
            int page_size = 9;
            int page_number = page ?? 1;

            return View("Women", them.ToPagedList(page_number, page_size));
        }


    }
}
