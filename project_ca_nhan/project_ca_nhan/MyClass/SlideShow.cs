using project_ca_nhan.Models;
namespace project_ca_nhan.MyClass
{
    public class SlideShow
    {
        public static List<ItemSlide> GetSlides()
        {
            MyDbcontext db = new MyDbcontext();
            List<ItemSlide> x = db.Slides.OrderByDescending(s=>s.Id).ToList();
            return x;
        }
    }
}
