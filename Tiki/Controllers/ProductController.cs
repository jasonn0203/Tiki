using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tiki.Models;

namespace Tiki.Controllers
{
    public class ProductController : Controller
    {
        readonly TikiEntities db = new TikiEntities();

        // GET: Detail
        public ActionResult Detail(string tenSP)
        {
            var sanPham = db.SanPhams.FirstOrDefault(sp => sp.TenSanPham == tenSP);


            //Lay ds comment
            List<BinhLuan> binhLuanList = new List<BinhLuan>();
            if (sanPham != null)
            {
                binhLuanList = db.BinhLuans.Where(bl => bl.MaSP == sanPham.MaSP).ToList();
            }

            //Số lượng sp theo nhà cung cấp
            int spCount = 0;
            spCount = db.SanPhams.Count(c => c.MaNCC == sanPham.MaNCC);



            ViewBag.SoLuongSP = spCount;
            ViewBag.BinhLuanList = binhLuanList;

            return View(sanPham);
        }


        [HttpPost]
        public ActionResult AddComment(int maSP, int maKH, string noiDung, string TenSanPham)
        {
            if (UserAuthenSingleton.Instance == null)
            {
                return RedirectToAction("SignIn", "User");
            }

            // Kiểm tra comment không rỗng
            if (string.IsNullOrWhiteSpace(noiDung))
            {
                // Xử lý lỗi nếu cần thiết
                return RedirectToAction("Detail", new { tenSP = TenSanPham });
            }


            BinhLuan binhLuan = new BinhLuan
            {
                NoiDung = noiDung,
                MaSP = maSP,
                MaKH = maKH,
                NgayBinhLuan = DateTime.Now
            };

            using (var db = new TikiEntities())
            {
                db.BinhLuans.Add(binhLuan);
                db.SaveChanges();

            }
            return RedirectToAction("Detail", new { tenSP = TenSanPham });

        }

    }
}