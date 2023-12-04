using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tiki.Models;
using Tiki.Models.Singleton;
using Tiki.Models.Strategy;

namespace Tiki.Controllers
{
    public class HomeController : Controller
    {
        private readonly TikiEntities db = DatabaseSingleton.Instance;

        public ActionResult Index()
        {
            ViewBag.PhanLoaiList = db.PhanLoaiSPs.ToList();


            List<SanPham> sphamList = db.SanPhams.ToList();
            //Random ds sản phẩm
            Random rd = new Random();
            sphamList = sphamList.OrderBy(r => rd.Next()).ToList();

            ViewBag.SanPhamList = sphamList;
            return View();
        }


        /* public ActionResult ProductByCategory(string tenPL, string sortOrder)
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
         }*/

        public ActionResult ProductByCategory(string tenPL, string sortOrder)
        {
            var spTheoPLList = db.SanPhams.Where(sp => sp.PhanLoaiSP.TenPhanLoai == tenPL).ToList();
            ViewBag.TenPL = tenPL;

            // Use a strategy based on the selected sort order
            ISortStrategy sortingStrategy;
            switch (sortOrder)
            {
                case "PriceAsc":
                    sortingStrategy = new PriceAscendingSortingStrategy();
                    ViewBag.PriceDesc = false;
                    ViewBag.PriceAsc = true;
                    break;
                case "PriceDesc":
                    sortingStrategy = new PriceDescendingSortingStrategy();
                    ViewBag.PriceAsc = false;
                    ViewBag.PriceDesc = true;
                    break;
                default:

                    sortingStrategy = new DefaultSortingStrategy();
                    ViewBag.PriceAsc = false;
                    ViewBag.PriceDesc = false;
                    break;
            }

            spTheoPLList = sortingStrategy.SortProducts(spTheoPLList);

            return View(spTheoPLList);
        }







        /*[HttpPost]
        public ActionResult Search(string searchString)
        {
            //Lấy ra ds sản phẩm dựa theo query của người dùng
            List<SanPham> spList = db.SanPhams.Where(s => s.TenSanPham.Contains(searchString) || s.PhanLoaiSP.TenPhanLoai.Contains(searchString)).ToList();
            if (spList == null)
            {
                return RedirectToAction("Index");
            }
            return View(spList);
        }*/


        [HttpPost]
        public ActionResult Search(string searchString)
        {
            IQueryable<SanPham> sanPhams = db.SanPhams.AsQueryable();

            // Chọn chiến lược
            ISearchStrategy searchByNameStrategy = new TenSanPhamSearchStrategy();
            ISearchStrategy searchByCateStrategy = new TenPhanLoaiSearchStrategy();

            // Tìm kiếm bằng cách sử dụng cả hai chiến lược
            List<SanPham> spListName = searchByNameStrategy.Search(searchString, sanPhams);
            List<SanPham> spListCate = searchByCateStrategy.Search(searchString, sanPhams);

            // Kết hợp kết quả từ cả hai chiến lược
            List<SanPham> combinedList = spListName.Concat(spListCate).ToList();

            if (combinedList == null || combinedList.Count == 0)
            {
                return RedirectToAction("Index");
            }

            return View(combinedList);
        }




    }

}