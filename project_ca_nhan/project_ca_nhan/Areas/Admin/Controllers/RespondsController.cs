using Microsoft.AspNetCore.Mvc;
// su dung ADo de lam bai
using X.PagedList;
using System.Data.SqlClient;
using System.Data;
using project_ca_nhan.Areas.Admin;
using project_ca_nhan.Areas.Admin.Attributes;
using project_ca_nhan.Models;
// de su dung cho datatable
namespace project_ca_nhan.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckLogin]
    public class RespondsController : Controller
    {
        // khoi tao bien toan cuc de su dung cho toan bien 
        public string strConnectionString;
        // khoi tao khong tham so ket noi den appsetting
        public RespondsController() {
            // de ket noi den appsetting
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

            this.strConnectionString = config.GetConnectionString("MyConnectionString");
        }

        public IActionResult Index()
        {
            return View("Read");
        }
        public IActionResult Read(int ? page)
        {
            // tao ra 1 bang 
            DataTable da = new DataTable();
            // ket noi den appsetting tong qua bien toan cuc strconnecstring
            using(SqlConnection connection = new SqlConnection(strConnectionString))
            {
                // vi la read nen  ta dugn sqladapter . su dung cau lenh cua sql
                SqlDataAdapter dt = new SqlDataAdapter("select * from Respond",connection);
                dt.Fill(da);


            }
            // chuyen doi kieu string cua Adapter sang cac kieu du lieu tuong ung
            List<ItemRespond> res = new List<ItemRespond>();
            // duyet cac phan tu cua adapter saiu do tra ve 
            foreach(DataRow x in da.Rows)
            {
                res.Add(new ItemRespond() { Id = Convert.ToInt32(x["Id"]), CustomerId = Convert.ToInt32(x["CustomerId"]),Message= x["Message"].ToString(), Date = Convert.ToDateTime(x["Date"]) });

            }
            // phan trang 
            int pageNumber = page ?? 1;
            int pageSize = 5;
            List<ItemRespond> i = res.OrderByDescending(s => s.Id).ToList();
            return View("Read",i.ToPagedList(pageNumber,pageSize));
        }
        public IActionResult ChiTiet(int ? id)
        {
            int _id = id ?? 0;
            // tim kiem pan hoi can update
            DataTable da = new DataTable();
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                // vi la update nen phai su dung sqlcommand
                SqlDataAdapter dt = new SqlDataAdapter("select * from Customers Where Id = " + _id, conn);
                dt.Fill(da);
            }
            // tao ra 1 hang moi trong datatable
            DataRow x = da.NewRow();
            if (da.Rows.Count > 0)
            {
                x = da.Rows[0];
            }
            ItemCustomer p = new ItemCustomer() { Id = Convert.ToInt32(x["Id"]), Name = x["Name"].ToString(), Address = x["Address"].ToString(), Email = x["Email"].ToString(), Phone = x["Phone"].ToString() };

           
            return View("ThongTinChiTiet", p);
        }
        // Update
        public IActionResult Update(int ? id)
        {
            int _id = id ?? 0;
            // cai nay su dungj ADO
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                // tim kiem vi tri can update
                SqlDataAdapter da = new SqlDataAdapter("select * from Respond Where Id="+id,conn);
                da.Fill(dt);
            }
            // liet ke ra 1 dong de chua cai cot tim kiem nay
            DataRow x = dt.NewRow();
            if(dt.Rows.Count > 0)
            {
                x = dt.Rows[0];
            }
            // chuyen doi dong string nay thanh cac thuoc tinh tuong ung cua ItemRespond
            ItemRespond chuyendoi = new ItemRespond() { Id = Convert.ToInt32(x["Id"]), CustomerId = Convert.ToInt32(x["CustomerId"]), Date = Convert.ToDateTime(x["Date"]), Message = x["Message"].ToString() };

                ViewBag.action = "/Admin/Responds/UpdatePost?id=" + _id;
            return View("CreateUpdate",chuyendoi);
        }
        [HttpPost,ValidateAntiForgeryToken]
        public IActionResult UpdatePost(int ? id)
        {
            int _id = id ?? 0;
            string message = !String.IsNullOrEmpty(Request.Form["Message"]) ? Request.Form["Message"] : "";
            DataTable dt = new DataTable();
            using(SqlConnection conn = new SqlConnection(strConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("update Respond set Message = @var_message where Id =@var_id",conn);
                cmd.Parameters.AddWithValue("var_message",message);
                cmd.Parameters.AddWithValue("var_id", _id);
                cmd.ExecuteNonQuery();
            }
            return Redirect("/Admin/Responds/Read");
        }

      
    }
}
//int _id = id ?? 0;
//// tim kiem pan hoi can update
//DataTable da = new DataTable();
//using (SqlConnection conn = new SqlConnection(strConnectionString))
//{
//    // vi la update nen phai su dung sqlcommand
//    SqlDataAdapter dt = new SqlDataAdapter("Select * from Respond Where Id = " + _id, conn);
//    dt.Fill(da);
//}
//// tao ra 1 hang moi trong datatable
//DataRow x = da.NewRow();
//if (da.Rows.Count > 0)
//{
//    x = da.Rows[0];
//}