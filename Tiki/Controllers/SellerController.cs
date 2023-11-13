using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tiki.Models;

namespace Tiki.Controllers
{
    public class SellerController : Controller
    {
        readonly TikiEntities db = new TikiEntities();


        // GET: Seller
        public ActionResult Dashboard()
        {
            if (Session["NhaCungCap"] == null)
            {
                return RedirectToAction("SellerSignIn", "Seller");
            }
            ViewBag.IsLoading = true;

            var maNCC = ((NhaCungCap)Session["NhaCungCap"]).MaNCC;

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
            ViewBag.Tab = "Dashboard";
            ViewBag.IsLoading = false;



            return View();
        }

        public ActionResult Management()
        {
            if (Session["NhaCungCap"] == null)
            {
                return RedirectToAction("SellerSignIn", "Seller");
            }
            ViewBag.Tab = "Management";

            var maNCC = ((NhaCungCap)Session["NhaCungCap"]).MaNCC;

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
            if (Session["NhaCungCap"] != null)
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
                Session["NhaCungCap"] = checkNCC;


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
            Session.Clear();
            Session.Abandon();
            Session["NhaCungCap"] = null;

            return RedirectToAction("SellerSignIn", "Seller");
        }

        public ActionResult ProductList()
        {
            if (Session["NhaCungCap"] == null)
            {
                return RedirectToAction("SellerSignIn", "Seller");
            }

            ViewBag.IsLoading = true;


            // Lấy mã chủ nhà từ session
            var maNCC = ((NhaCungCap)Session["NhaCungCap"]).MaNCC;

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
            if (Session["NhaCungCap"] == null)
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

                var nhaCungCap = (NhaCungCap)Session["NhaCungCap"];
                sanPham.MaNCC = nhaCungCap.MaNCC;

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
                // Lưu thông tin sản phẩm đã chỉnh sửa vào cơ sở dữ liệu
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ProductList");
            }


            ViewBag.PhanLoaiList = new SelectList(db.PhanLoaiSPs, "MaPhanLoai", "TenPhanLoai");
            return View(sanPham);
        }


        //public ActionResult SalesReportByProduct(int maNCC)
        //{
        //    using (var context = new TikiEntities()) 
        //    {
        //        var salesData = context.ChiTietDonHangs
        //     .Where(ct => ct.DonDatHang.KhachHang.SanPham.MaNCC == maNCC)
        //     .Select(ct => new
        //     {
        //         NgayBan = ct.DonDatHang.NgayDat,
        //         MaDonHang = ct.MaDonHang,
        //         TenSanPham = ct.SanPham.TenSanPham,
        //         Gia = ct.DonGia,
        //         SoLuong = ct.SoLuong,
        //         TongCong = ct.DonGia * ct.SoLuong
        //     })
        //     .ToList();

        //        return View(salesData);
        //    }
        //}


    }

}