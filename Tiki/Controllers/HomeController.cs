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
        readonly TikiDatabase db = new TikiDatabase();
        public ActionResult Index()
        {
            ViewBag.PhanLoaiList = db.PhanLoaiSPs.ToList();
            ViewBag.SanPhamList = db.SanPhams.ToList();

            return View();
        }


        public ActionResult ProductByCategory(string tenPL)
        {
            var spTheoPLList = db.SanPhams.Where(sp => sp.PhanLoaiSP.TenPhanLoai == tenPL).ToList();
            return View(spTheoPLList);
        }



    }

}