using project_ca_nhan.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
namespace project_ca_nhan.Models
{
    public class Cart
    {
        // chuyen doi session["cart"} sang kieu list
        protected static readonly MyDbcontext db = new MyDbcontext();
        public static T GetObjectFromJson<T>(ISession session, string key){
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);

       }
        // lay gio hang dang ton tai
        public static List<Item> GetCart(ISession session)
        {
            // chuyen doi session sang kieu list
            List<Item> Car = Cart.GetObjectFromJson<List<Item>>(session, "cart");
            return Car;
        }
        // kiem tra vi tri cua san pham trong gio hang
        public static int IsExit(ISession session,int id)
        {
            List<Item> sanpham = Cart.GetObjectFromJson<List<Item>>(session, "cart");
            for(int i = 0; i < sanpham.Count; i++)
            {
                if (sanpham[i].ProductRecord.Id == id)
                {
                    return i;
                }
            }
            return -1;
        }
        // them san pham vao gio hang
        public static void CarAdd(ISession session, int id)
        {
            
            if(Cart.GetObjectFromJson<List<Item>>(session, "cart")  == null)
            {
                List<Item> bang_car = new List<Item>();
                ItemProduct x = db.Products.FirstOrDefault(s => s.Id == id);
                Item y = new Item();
                y.ProductRecord = x;
                y.Quantity = 1;

                bang_car.Add(y);
                session.SetString("cart", JsonConvert.SerializeObject(bang_car));

            }
            else
            {
                List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");
                // kiem tra xem vi tri cua no
                int vitri = Cart.IsExit(session, id);
                if (vitri != -1)
                {
                    cart[vitri].Quantity++;
                }
                else
                {
                    
                    ItemProduct x = db.Products.FirstOrDefault(s => s.Id == id);
                    Item y = new Item();
                    y.ProductRecord = x;
                    y.Quantity = 1;
                    cart.Add(y);

                    
                }
                session.SetString("cart", JsonConvert.SerializeObject(cart));


            }
        }
        // xoa 1 hang trong bang cart
        public static void CartRemove(ISession session,int id)
        {
            List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");
            // tim vi tri can xoa
            for(int i = 0; i < cart.Count; i++)
            {
                if (cart[i].ProductRecord.Id == id)
                {
                    cart.RemoveAt(i);
                }
            }
            session.SetString("cart",JsonConvert.SerializeObject(cart));
        }
        // xoa tat ca cac bang
        public static void CartDestroy(ISession session) {
            // khoi tang bang 
            List<Item> cart = new List<Item>();
            session.SetString("cart", JsonConvert.SerializeObject(cart));
        }
        // cap nhat lai so luong cua bang
        public static void CartUpdate(ISession session,int id,int quantity)
        {
            List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");
            // tim kiem vi tri cap nhat thong qua id
            for(int i=0;i< cart.Count; i++)
            {
                if (cart[i].ProductRecord.Id == id)
                {
                    cart[i].Quantity = quantity;
                }
            }
            session.SetString("cart",JsonConvert.SerializeObject(cart));
        }
        // Tính tổng số tiền cho tat ca sản phẩm
        public static double CartTotal(ISession session)
        {
            List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");
            if (cart.Count != 0)
            {
                double tong = 0;
                for (int i = 0; i < cart.Count; i++)
                {
                    tong += cart[i].Quantity * (cart[i].ProductRecord.Price - (cart[i].ProductRecord.Price * cart[i].ProductRecord.Discount) / 100);
                }
                return tong;
            }
            else
            {
                return 0;
            }
        }
        //lấy số sản phẩm trong giỏ hàng
        public static int CartQuantity(ISession session)
        {
            List<Item> cart = Cart.GetCart(session);
            if(cart.Count != 0)
            {
                return cart.Count;
            }
            return 0;
        }
        // kiem tra 
        public static void CartCheckOut(ISession session, int customer_id)
        {
            //khởi tạo đối tượng thao tác csdl
            MyDbcontext db = new MyDbcontext();
            //---
            List<Item> _cart = Cart.GetCart(session);
            //insert du lieu vao table Orders
            ItemOrder _RecordOrder = new ItemOrder();
            _RecordOrder.CustomerId = customer_id;
            _RecordOrder.Create = DateTime.Now;
            _RecordOrder.Price = _cart.Sum(tbl => tbl.ProductRecord.Price * tbl.Quantity);
            db.Orders.Add(_RecordOrder);
            db.SaveChanges();
            //lay id vua insert
            int order_id = _RecordOrder.Id;
            //duyet cac san pham trong session, moi phan tu se add thanh 1 ban ghi trong OrdersDetail
            foreach (var item in _cart)
            {
                ItemOrderDetail _RecordOrdersDetail = new ItemOrderDetail();
                _RecordOrdersDetail.OrderId = order_id;
                _RecordOrdersDetail.ProductId = item.ProductRecord.Id;
                _RecordOrdersDetail.Price = item.ProductRecord.Price - (item.ProductRecord.Price * item.ProductRecord.Discount) / 100;
                _RecordOrdersDetail.Quantity = item.Quantity;
                //---
                db.OrdersDetail.Add(_RecordOrdersDetail);
                db.SaveChanges();
            }
            //xoa tat cac cac phan tu trong gio hang
            Cart.CartDestroy(session);
        }





    }
}
