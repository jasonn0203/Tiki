﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        }

        private UserAuthenSingleton() { }
    }

}