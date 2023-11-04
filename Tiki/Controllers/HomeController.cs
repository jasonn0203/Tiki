using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tiki.Models;

namespace Tiki.Controllers
{
    public class HomeController : Controller
    {
        TikiDatabase db = new TikiDatabase(); 
        public ActionResult Index()
        {
            var phanLoaiList = db.PhanLoaiSPs.ToList();
            return View(phanLoaiList);
        }

        public ActionResult ProductByCategory()
        {
            return View();
        }


    }
}