using System.Web;

namespace Tiki.Models
{
    public class UserAuthenSingleton
    {
        private static KhachHang instance;

        public static KhachHang Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (KhachHang)HttpContext.Current.Session["KhachHang"];
                }
                return instance;
            }
            set
            {
                instance = value;
                HttpContext.Current.Session["KhachHang"] = value;
            }
        }

        private UserAuthenSingleton() { }
    }


}