// nhin hay file MyDbContext
using project_ca_nhan.Models;
namespace project_ca_nhan.MyClass
{

	public class Adv
	{
		public static List<ItemAdv> GetAdv(int _Position)
		{
			// khoi tao doi tuong tren 1 csdl
		MyDbcontext db = new MyDbcontext();
		List<ItemAdv> list_cord = db.Adv.Where(s => s.Position == _Position).ToList();
			return list_cord;
		}
	}
}
