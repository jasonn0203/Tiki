using System;
using System.Collections.Generic;
using System.Linq;
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
                if (instance == null && HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    instance = (NhaCungCap)HttpContext.Current.Session["NhaCungCap"];
                }
                return instance;
            }
        }

        private SellerAuthenSingleton() { }

    }
}