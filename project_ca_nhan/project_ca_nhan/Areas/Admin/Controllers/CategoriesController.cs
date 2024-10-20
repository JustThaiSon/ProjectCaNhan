using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using project_ca_nhan.Areas.Admin.Attributes;
using project_ca_nhan.Models;
//để sử dụng ADO thì cần import thư viện sau
using System.Data.SqlClient;//sử dụng cho các đối tượng SqlConnection, SqlDataAdapter, SqlCommand...
//để sử dụng đối tượng DataTable, DataSet cần import thư viện sau
using System.Data;
//đọc các biến trong file appsettings.json
using Microsoft.Extensions.Configuration;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckLogin]
    public class CategoriesController : Controller
    {
        //tạo biến lưu chuỗi kết nối csdl
        public string strConnectionString;
        //hàm tạo được mặc định triệu gọi đầu tiên khi class hoạt động
        public CategoriesController() 
        {
            //đọc thông tin trong file appsettings.json, trả kết quả về đối tượng config
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            this.strConnectionString = config.GetConnectionString("MyConnectionString");
        }
        public IActionResult Index()
        {
            return RedirectToAction("Read");
        }
        public IActionResult Read(int? page)
        {       
            //tạo đối tượng DataTable
            DataTable dtCategories = new DataTable();
            using(SqlConnection conn = new SqlConnection(strConnectionString)) 
            {
                SqlDataAdapter da = new SqlDataAdapter("select * from Categories where ParentId = 0 order by Id desc",conn);
                //fill dữ liệu vào biến DataTable
                da.Fill(dtCategories);
            }
            //do đối tượng X.PagedList sử dụng List để phân trang, vì vậy cần chuyển dữ liệu của DataTable dtCategories sang biến kiểu List
            List<ItemCategory> list_categories = new List<ItemCategory>();
            //duyệt các phần tử của DataTable
            foreach(DataRow row in dtCategories.Rows)
            {
                list_categories.Add(new ItemCategory() { Id = Convert.ToInt32(row["Id"]),ParentId = Convert.ToInt32(row["ParentId"]),Name = row["Name"].ToString(),DisplayHomePage = Convert.ToInt32(row["DisplayHomePage"]) });
            }
            //phân trang sử dụng đối tượng X.PagedList
            //thiết lập số bản ghi trên một trang
            int page_size = 4;
            //thiết lập trang hiện tại
            int page_number = page ?? 1;//nếu p là null thì page_number=1, không thì page_number=p
            return View("Read", list_categories.ToPagedList(page_number, page_size));
        }
        //update
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            //khai báo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Categories/UpdatePost/" + _id;
            //tạo biến DataTable
            DataTable dtCategory = new DataTable();
            using(SqlConnection conn = new SqlConnection(strConnectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("select * from Categories where Id = " + _id,conn);
                da.Fill(dtCategory);
            }
            //tạo một row mới
            DataRow row = dtCategory.NewRow();
            if (dtCategory.Rows.Count > 0)
                row = dtCategory.Rows[0];
            return View("CreateUpdate", row);
        }
        //update post
        [HttpPost]
        public IActionResult UpdatePost(int id, IFormCollection fc) 
        {
            string _Name = fc["Name"].ToString().Trim();
            int _ParentId = Convert.ToInt32(fc["ParentId"]);
            int _DisplayHomePage = !String.IsNullOrEmpty(fc["DisplayHomePage"]) ? 1 : 0;
            using(SqlConnection conn = new SqlConnection(strConnectionString))
            {
                //phải có lệnh sau thì mới update được
                conn.Open();
                SqlCommand cmd = new SqlCommand("update Categories set Name=@var_name, ParentId = @var_parent_id, DisplayHomePage = @var_display_home_page where Id = @var_id",conn);
                cmd.Parameters.AddWithValue("var_name", _Name);
                cmd.Parameters.AddWithValue("var_parent_id", _ParentId);
                cmd.Parameters.AddWithValue("var_display_home_page", _DisplayHomePage);
                cmd.Parameters.AddWithValue("var_id", id);
                //update
                cmd.ExecuteNonQuery();
            }
            return Redirect("/Admin/Categories");
        }
        //create
        public IActionResult Create()
        {
            //khai báo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Categories/CreatePost";
            return View("CreateUpdate");
        }
        //create post
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string _Name = fc["Name"].ToString().Trim();
            int _ParentId = Convert.ToInt32(fc["ParentId"]);
            int _DisplayHomePage = !String.IsNullOrEmpty(fc["DisplayHomePage"]) ? 1 : 0;
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                //phải có lệnh sau thì mới update được
                conn.Open();
                SqlCommand cmd = new SqlCommand("insert into Categories(Name,ParentId,DisplayHomePage) values(@var_name,@var_parent_id,@var_display_home_page)", conn);
                cmd.Parameters.AddWithValue("var_name", _Name);
                cmd.Parameters.AddWithValue("var_parent_id", _ParentId);
                cmd.Parameters.AddWithValue("var_display_home_page", _DisplayHomePage);
                //update
                cmd.ExecuteNonQuery();
            }
            return Redirect("/Admin/Categories");
        }
        //delete
        public IActionResult Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                //phải có lệnh sau thì mới update được
                conn.Open();
                SqlCommand cmd = new SqlCommand("delete from Categories where Id = @var_id", conn);
                cmd.Parameters.AddWithValue("var_id", id);
                //update
                cmd.ExecuteNonQuery();
            }
            return Redirect("/Admin/Categories");
        }
    }
}
