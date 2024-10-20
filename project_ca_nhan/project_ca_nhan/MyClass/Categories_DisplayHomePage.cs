using Microsoft.Identity.Client;
using project_ca_nhan.Models;
namespace project_ca_nhan.MyClass
{
    public class Categories_DisplayHomePage
    {
        public static List<ItemCategory> GetCategories()
        {
            MyDbcontext db = new MyDbcontext();
            List<ItemCategory> x = db.Categories.Where(s => s.DisplayHomePage == 1).OrderByDescending(s => s.Id).ToList();
            return x;
        }
        public static List<ItemProduct> GetProducts(int id)
        {
            MyDbcontext db = new MyDbcontext();
            List<ItemProduct> x = (from product in db.Products
                                   join category_product in db.CategoriesProducts on product.Id equals category_product.ProductId
                                   join categories in db.Categories on category_product.CategoryId equals categories.Id
                                   where categories.Id == id
                                   select product
                                   ).ToList();
            return x;
        }
    }
}
