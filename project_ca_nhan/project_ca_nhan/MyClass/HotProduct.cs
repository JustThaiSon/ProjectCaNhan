using project_ca_nhan.Models;
using System.Security.Policy;
namespace project_ca_nhan.MyClass
{
    public class HotProduct
    {
        public static List<ItemProduct> GetHotProduct()
        {
            MyDbcontext db = new MyDbcontext();
            List<ItemProduct> items = db.Products.OrderByDescending(s => s.Id).Where(s => s.Hot == 1).ToList();
            return items;
        }
    }
}
