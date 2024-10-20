using project_ca_nhan.Models;

namespace project_ca_nhan.MyClass
{
    public class DiscountProduct
    {
        public static List<ItemProduct> GetDiscountProduct()
        {
            // lay san pahm giam gia > 0
            MyDbcontext db = new MyDbcontext();
            List<ItemProduct> item = db.Products.OrderByDescending(x => x.Id).Where(s=>s.Discount > 0).ToList();
            return item;
        }
    }
}
