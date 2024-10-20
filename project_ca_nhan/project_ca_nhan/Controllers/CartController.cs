using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using project_ca_nhan.Models;
namespace project_ca_nhan.Controllers
{
    public class CartController : Controller
    {
        public MyDbcontext db = new MyDbcontext();
        public IActionResult Index()
        {
            return RedirectToAction("Read");
        }
        public IActionResult Read(int id)
        {
            string cart = HttpContext.Session.GetString("cart");
            List<Item> Items = new List<Item>();
            if(!String.IsNullOrEmpty(cart))
            {
                Items = JsonConvert.DeserializeObject<List<Item>>(cart);
            }
            return View("Read",Items);
        }
        public IActionResult Add(int id)
        {
            Cart.CarAdd(HttpContext.Session, id);
            return Redirect("/Cart/Read");
        }
        // xoa 1 san pham khoi gio hang
        public IActionResult Delete(int id)
        {
            Cart.CartRemove(HttpContext.Session, id);
            return Redirect("/Cart/Read");
        }
        // cap nhat lai so luong gio hang cho dung
        public IActionResult CartUpdate()
        {
            string da = HttpContext.Session.GetString("cart");
            List<Item> items = new List<Item>();
            if(!String.IsNullOrEmpty(da))
            {
                items = JsonConvert.DeserializeObject<List<Item>>(da);
                foreach(var x in items)
                {
                   
                    int quantity = Convert.ToInt32(Request.Form["product_"+x.ProductRecord.Id]);
                    Cart.CartUpdate(HttpContext.Session, x.ProductRecord.Id, quantity);
                }
            }
            return Redirect("/Cart/Read");
        }
        public IActionResult Destroy()
        {
            Cart.CartDestroy(HttpContext.Session);
            return Redirect("/Cart/Read");
        }
        // kiem tra checkout xem tai khoan da co dang nhap hay chua , neu co roi thi se mua ngay
        // checkout o day la mua ngay
        public IActionResult Checkout()
        {
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("customer_user_id")))
                return Redirect("/Account/Login");
            else
            {
                Cart.CartCheckOut(HttpContext.Session, Convert.ToInt32(HttpContext.Session.GetString("customer_user_id")));
                
            }
            return Redirect("Read");
        }
    }
}
