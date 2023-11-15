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
        readonly TikiEntities db = new TikiEntities();
        public ActionResult Index()
        {
            ViewBag.PhanLoaiList = db.PhanLoaiSPs.ToList();
            ViewBag.SanPhamList = db.SanPhams.ToList();

            return View();
        }


        public ActionResult ProductByCategory(string tenPL, string sortOrder)
        {
            // Giá trị mặc định ban đầu 
            var spTheoPLList = db.SanPhams.Where(sp => sp.PhanLoaiSP.TenPhanLoai == tenPL).ToList();
            ViewBag.PriceDesc = false;
            ViewBag.PriceAsc = false;
            switch (sortOrder)
            {
                case "PriceAsc":
                    spTheoPLList = spTheoPLList.OrderBy(s => s.Gia).ToList();
                    ViewBag.PriceDesc = false;
                    ViewBag.PriceAsc = true;
                    break;
                case "PriceDesc":
                    spTheoPLList = spTheoPLList.OrderByDescending(s => s.Gia).ToList();
                    ViewBag.PriceAsc = false;
                    ViewBag.PriceDesc = true;
                    break;
                default:
                    // Sort products by product name by default
                    spTheoPLList = spTheoPLList.OrderBy(s => s.TenSanPham).ToList();
                    break;
            }
            ViewBag.TenPL = tenPL;

            return View(spTheoPLList);
        }


        [HttpPost]
        public ActionResult Search(string searchString)
        {
            //Lấy ra ds sản phẩm dựa theo query của người dùng
            List<SanPham> spList = db.SanPhams.Where(s => s.TenSanPham.Contains(searchString) || s.PhanLoaiSP.TenPhanLoai.Contains(searchString)).ToList();
            if (spList == null)
            {
                return RedirectToAction("Index");
            }
            return View(spList);
        }


    }

}