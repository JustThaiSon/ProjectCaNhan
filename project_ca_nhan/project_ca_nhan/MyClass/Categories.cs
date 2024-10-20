using Microsoft.AspNetCore.Mvc;
using project_ca_nhan.Models;
namespace project_ca_nhan.MyClass
{
    public class Categories
    {
        public static string GetNameDanhMuc(int _CategoriesId)
        {
            // _id o day la Id cua category tu Product/Categories voi viewbag.CategoriesId nhap vao
            MyDbcontext db = new MyDbcontext();
            ItemCategory x = db.Categories.Where(s=>s.Id == _CategoriesId).FirstOrDefault();
            return x!= null ? x.Name : "Categories";
        }
        // muc liet ke cac danh muc san pham
        // neu id ==0 thi la danh muc cha
        // neu id # 0 thi la danh muc con
        public static List<ItemCategory> GetDanhMuc(int id)
        {
            MyDbcontext db = new MyDbcontext();
            List<ItemCategory> d = db.Categories.Where(s=>s.ParentId == id).OrderByDescending(s=>s.Id).ToList();
            return d;
        }
        // lay tat cac cac danh muc cha voi parent Id =0 va no chi lay dung 6 danh muc cha dau tien de hien thi
        public static List<ItemCategory> GetDanhMucCha(int id)
        {
            MyDbcontext db = new MyDbcontext();
            List<ItemCategory> d = db.Categories.Where(s => s.ParentId == id).OrderByDescending(s => s.Id).Skip(0).Take(6).ToList();
            return d;
        }
        // lấy tất cả các danh mục con , không phải danh mụcn cha , nếu pảentid #0
        public static List<ItemCategory> GetDanhMucCon()
        {
            MyDbcontext db = new MyDbcontext();
            List<ItemCategory> d = db.Categories.Where(s => s.Id != 0).OrderByDescending(s => s.Id).Skip(0).Take(6).ToList();
            return d;
        }



    }
}
