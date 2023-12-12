using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Tiki.Models;
using Tiki.Models.Singleton;

namespace Tiki.Controllers
{
    public class SellerController : Controller
    {
        private readonly TikiEntities db = DatabaseSingleton.Instance;

        public int GetMaNCC()
        {
            return SellerAuthenSingleton.Instance.MaNCC;
        }

        // GET: Seller
        public ActionResult Dashboard()
        {
            if (SellerAuthenSingleton.Instance == null)
            {
                return RedirectToAction("SellerSignIn", "Seller");
            }
            ViewBag.IsLoading = true;


            var maNCC = GetMaNCC();

            //Số sp đăng bán
            int spCount = 0;
            spCount = db.SanPhams.Count(c => c.MaNCC == maNCC);


            //Số SP đã bán
            var totalQuantitySoldBySupplier = db.ChiTietDonHangs
                    .Where(ct => ct.SanPham.MaNCC == maNCC)
                    .GroupBy(ct => ct.SanPham.MaNCC)
                    .Select(group => group.Count());



            ViewBag.SoLuongBan = totalQuantitySoldBySupplier.Count();
            ViewBag.SoLuongSP = spCount;
            ViewBag.TongDoanhThu = TinhTongDoanhThu(maNCC);
            ViewBag.TongBL = TinhTongDanhGia(maNCC);
            ViewBag.BinhLuanList = getBinhLuanList(maNCC);

            ViewBag.Tab = "Dashboard";
            ViewBag.IsLoading = false;



            return View();
        }

        //Tính tổng doanh thu cho seller
        private decimal? TinhTongDoanhThu(int maNCC)
        {
            decimal? tongTien = db.ChiTietDonHangs
                                    .Where(n => n.SanPham.MaNCC == maNCC)
                                    .Sum(t => (decimal?)t.SoLuong * t.DonGia);

            return tongTien ?? 0;
        }


        //Tính tổng SL đánh giá
        private int TinhTongDanhGia(int maNCC)
        {

            int tongBL = db.BinhLuans.Where(n => n.SanPham.MaNCC == maNCC).Count();
            return tongBL;
        }

        //Lấy dsach đánh giá từ KH
        public List<BinhLuan> getBinhLuanList(int maNCC)
        {
            return db.BinhLuans
                     .Where(bl => db.SanPhams.Any(sp => sp.MaNCC == maNCC && sp.MaSP == bl.MaSP))
                     .Include(bl => bl.KhachHang).OrderByDescending(t => t.NgayBinhLuan)
                     .ToList();
        }


        public ActionResult Management()
        {
            if (SellerAuthenSingleton.Instance == null)
            {
                return RedirectToAction("SellerSignIn", "Seller");
            }
            ViewBag.Tab = "Management";

            var maNCC = GetMaNCC();

            //Doanh thu theo sản phẩm ( Tạo thêm 1 class để chứa thông tin )
            var doanhThuTheoSanPham = db.ChiTietDonHangs
                .Include(ct => ct.SanPham)
                .Where(ct => ct.SanPham.MaNCC == maNCC)
                .Select(ct => new DoanhThuTheoSanPham
                {
                    NgayBan = (DateTime)ct.DonDatHang.NgayDat,
                    IDDonHang = ct.MaDonHang,
                    TenSanPham = ct.SanPham.TenSanPham,
                    Gia = (decimal)ct.DonGia,
                    SoLuong = (short)ct.SoLuong,
                    TongCong = (decimal)(ct.DonGia * ct.SoLuong)
                })
                .ToList();




            return View(doanhThuTheoSanPham);
        }




        //GET
        public ActionResult SellerSignUp()
        {
            if (SellerAuthenSingleton.Instance != null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SellerSignUp(NhaCungCap ncc)
        {
            //KT trường hợp trùng email đky tồn tại trong db
            var checkUniqueEmail = db.NhaCungCaps.FirstOrDefault(n => n.Email.Equals(ncc.Email));

            if (ModelState.IsValid)
            {
                if (checkUniqueEmail != null)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại. Vui lòng chọn một email khác.");
                    return View(ncc);
                }

                var nhaCungCap = new NhaCungCap
                {
                    TenNhaCungCap = ncc.TenNhaCungCap,
                    DiaChi = ncc.DiaChi,
                    Email = ncc.Email,
                    MatKhau = ncc.MatKhau,
                    SoDienThoai = ncc.SoDienThoai

                };

                db.NhaCungCaps.Add(nhaCungCap);
                db.SaveChanges();

                return RedirectToAction("SellerSignIn", "Seller"); // Chuyển hướng đến trang đăng nhập sau khi đăng ký thành công
            }

            return View(ncc);
        }



        //GET

        public ActionResult SellerSignIn()
        {
            if (SellerAuthenSingleton.Instance != null)
            {
                return RedirectToAction("SellerSignIn", "Seller");
            }
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SellerSignIn(NhaCungCap ncc)
        {
            //KT trường hợp email và mk có tồn tại trong db

            var checkNCC = db.NhaCungCaps.FirstOrDefault(n => n.Email == ncc.Email && n.MatKhau == ncc.MatKhau);

            if (checkNCC != null)
            {
                SellerAuthenSingleton.Instance = checkNCC;


                return RedirectToAction("Dashboard", "Seller");

            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng !");

                return View(ncc);
            }


        }

        public ActionResult LogOut()
        {
            SellerAuthenSingleton.Instance = null;

            Session.Clear();
            Session.Abandon();
            Session["NhaCungCap"] = null;

            return RedirectToAction("SellerSignIn", "Seller");
        }

        public ActionResult ProductList()
        {
            if (SellerAuthenSingleton.Instance == null)
            {
                return RedirectToAction("SellerSignIn", "Seller");
            }

            ViewBag.IsLoading = true;


            // Lấy mã chủ nhà từ session
            var maNCC = GetMaNCC();

            // Lấy danh sách các phòng thuộc mã chủ nhà
            List<SanPham> spList = db.SanPhams.Where(s => s.MaNCC == maNCC).ToList();

            if (spList == null || spList.Count == 0)
            {
                ViewBag.Message = "List sản phẩm hiện đang trống !";
                return View();
            }

            ViewBag.Tab = "ProductList";
            ViewBag.IsLoading = false;
            return View(spList);

        }


        /*THÊM SP*/
        public ActionResult AddProduct()
        {
            if (SellerAuthenSingleton.Instance == null)
            {
                return RedirectToAction("SellerSignIn", "Seller");
            }

            //Lấy dsach tên ploai
            ViewBag.PhanLoaiList = new SelectList(db.PhanLoaiSPs, "MaPhanLoai", "TenPhanLoai");

            var nhaCungCap = (NhaCungCap)Session["NhaCungCap"];
            ViewBag.MaNCC = nhaCungCap.MaNCC;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProduct(SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                /*Lấy mã nhà cung cấp*/


                sanPham.MaNCC = GetMaNCC();

                db.SanPhams.Add(sanPham);
                db.SaveChanges();
                return RedirectToAction("ProductList");
            }

            ViewBag.PhanLoaiList = new SelectList(db.PhanLoaiSPs, "MaPhanLoai", "TenPhanLoai");
            ViewBag.MaNCC = sanPham.MaNCC;

            return View(sanPham);
        }

        //EDIT SP
        // GET: 
        public ActionResult EditProduct(int? maSP)
        {
            if (Session["NhaCungCap"] == null)
            {
                return RedirectToAction("SellerSignIn", "Seller");
            }

            if (maSP == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Lấy thông tin sản phẩm từ cơ sở dữ liệu
            SanPham sp = db.SanPhams.Find(maSP);

            if (sp == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách phân loại để hiển thị lựa chọn
            ViewBag.PhanLoaiList = new SelectList(db.PhanLoaiSPs, "MaPhanLoai", "TenPhanLoai");

            return View(sp);
        }



        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(SanPham sanPham)
        {

            if (ModelState.IsValid)
            {
                var existingEntry = db.ChangeTracker.Entries<SanPham>().FirstOrDefault(e => e.Entity.MaSP == sanPham.MaSP);

                if (existingEntry != null)
                {
                    existingEntry.State = EntityState.Detached;
                }

                db.SanPhams.Attach(sanPham);
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("ProductList");
            }



            ViewBag.PhanLoaiList = new SelectList(db.PhanLoaiSPs, "MaPhanLoai", "TenPhanLoai");
            return View(sanPham);
        }

        public ActionResult DeleteProduct(int? maSP)
        {
            if (maSP == null)
            {
                return RedirectToAction("ProductList");
            }

            SanPham sp = db.SanPhams.Find(maSP);

            if (sp == null)
            {
                return RedirectToAction("ProductList");
            }

            //Tìm ràng buộc trong chi tiết đơn hàng
            var ctdh = db.ChiTietDonHangs.Where(ct => ct.MaSP == maSP).ToList();
            foreach (var ct in ctdh)
            {
                db.ChiTietDonHangs.Remove(ct);
            }


            db.SanPhams.Remove(sp);
            db.SaveChanges();

            return RedirectToAction("ProductList");
        }


        /*Chart*/
        public ActionResult Chart()
        {
            if (SellerAuthenSingleton.Instance == null)
            {
                return RedirectToAction("SellerSignIn", "Seller");
            }

            // Lấy mã chủ nhà từ session
            var maNCC = GetMaNCC();



            //Linechart
            var revenueData = GetRevenueDataForChart(maNCC);

            var chartData = revenueData.Select(d => new { Date = d.NgayDat.ToString("yyyy-MM-dd"), Revenue = d.TongDoanhThu });
            ViewBag.ChartData = chartData;

            //PieChart
            var pieChartData = GetQuantityByProductPieChart(maNCC);

            ViewBag.ProductQuantityData = pieChartData;

            ViewBag.TongDoanhThu = TinhTongDoanhThu(maNCC);
            ViewBag.Tab = "Chart";
            return View();
        }

        private List<LineChartModel> GetRevenueDataForChart(int maNCC)
        {
            // Áp dụng where trước khi tính tổng doanh thu lọc các ngày k có doanh thu
            var revenueData = db.ChiTietDonHangs
                .Where(ct => ct.SanPham.MaNCC == maNCC && ct.DonDatHang.NgayDat.HasValue)
                .GroupBy(ct => DbFunctions.TruncateTime(ct.DonDatHang.NgayDat))
                .Select(g => new LineChartModel
                {
                    NgayDat = (DateTime)g.Key,
                    TongDoanhThu = (decimal)g.Sum(ct => ct.DonGia * ct.SoLuong)
                })
                .OrderBy(data => data.NgayDat)
                .ToList();

            return revenueData;
        }


        private List<PieChartModel> GetQuantityByProductPieChart(int maNCC)
        {
            var productQuantityData = db.ChiTietDonHangs
                 .Where(ct => ct.SanPham.MaNCC == maNCC && ct.DonDatHang.NgayDat.HasValue)
                 .GroupBy(ct => ct.SanPham.TenSanPham)
                 .Select(g => new PieChartModel
                 {
                     TenSP = g.Key,
                     SoLuong = (int)g.Sum(ct => ct.SoLuong)
                 })
                 .ToList();


            return productQuantityData;
        }



    }



}

