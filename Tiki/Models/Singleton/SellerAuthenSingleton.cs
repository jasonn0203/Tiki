using System.Web;

namespace Tiki.Models
{
    public class SellerAuthenSingleton
    {
        private static NhaCungCap instance;

        public static NhaCungCap Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (NhaCungCap)HttpContext.Current.Session["NhaCungCap"];
                }
                return instance;
            }
            set
            {
                instance = value;
                HttpContext.Current.Session["NhaCungCap"] = value;
            }
        }

        private SellerAuthenSingleton() { }

    }
}