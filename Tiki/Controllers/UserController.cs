using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tiki.Models;

namespace Tiki.Controllers
{
    public class UserController : Controller
    {
        readonly TikiEntities db = new TikiEntities();
        // GET: User
        public ActionResult SignUp()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(KhachHang kh)
        {
            //KT trường hợp trùng email đky tồn tại trong db
            var checkUniqueEmail = db.KhachHangs.FirstOrDefault(k => k.Email.Equals(kh.Email));

            if (ModelState.IsValid)
            {
                if (checkUniqueEmail != null)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại. Vui lòng chọn một email khác.");
                    return View(kh);
                }


                var khachHang = new KhachHang
                {
                    TenKhachHang = kh.TenKhachHang,
                    DiaChi = kh.DiaChi,
                    Email = kh.Email,
                    MatKhau = kh.MatKhau,
                    SoDienThoai = kh.SoDienThoai

                };

                db.KhachHangs.Add(khachHang);
                db.SaveChanges();

                return RedirectToAction("SignIn", "User"); // Chuyển hướng đến trang đăng nhập sau khi đăng ký thành công
            }

            return View(kh);
        }


        //GET
        public ActionResult SignIn()
        {
            if (Session["KhachHang"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }


        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(KhachHang kh)
        {
            //KT trường hợp email và mk có tồn tại trong db
            var checkKhachHang = db.KhachHangs.FirstOrDefault(k => k.Email == kh.Email && k.MatKhau == kh.MatKhau);

            if (checkKhachHang != null)
            {
                Session["KhachHang"] = checkKhachHang;

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng !");
                return View(kh);
            }


        }

        public ActionResult LogOut()
        {
            Session.Clear();
            Session.Abandon();
            Session["KhachHang"] = null;

            return RedirectToAction("Index", "Home");
        }



        //--------------------------------





        public ActionResult Invoices(int MaDH, string tenKH)
        {

            var orderDetails = db.ChiTietDonHangs
                .Where(ct => ct.MaDonHang == MaDH && ct.DonDatHang.KhachHang.TenKhachHang == tenKH)
                .Include(ct => ct.SanPham)
                .ToList();

            // Calculate the total value of the order
            decimal totalValue = (decimal)orderDetails.Sum(ct => ct.SoLuong * ct.DonGia);

            int maDH = orderDetails.FirstOrDefault().MaDonHang;
            DateTime ngayTaoDon = (DateTime)orderDetails.FirstOrDefault().DonDatHang.NgayDat;

            // Save the total value in ViewBag
            ViewBag.TongTienHoaDon = totalValue;
            ViewBag.MaDonHang = MaDH;
            ViewBag.NgayTaoDon = ngayTaoDon;
            if (orderDetails != null)
            {
                return View(orderDetails);

            }
            else
            {
                return View("Home", "Index");
            }

        }

        public ActionResult RemoveOrder(int maDH, string tenKH)
        {
            var orderToRemove = db.DonDatHangs
                                     .Include(dh => dh.ChiTietDonHangs)
                                     .FirstOrDefault(dh => dh.MaDonHang == maDH && dh.KhachHang.TenKhachHang == tenKH);

            if (orderToRemove != null)
            {
                // Hủy đơn đặt hàng
                db.DonDatHangs.Remove(orderToRemove);
                db.SaveChanges();

                return RedirectToAction("General", new { tenKH = tenKH });
            }
            else
            {
                return HttpNotFound();
            }


        }




        public ActionResult General(string tenKH)
        {

            // Lấy danh sách các đơn hàng theo TenKH
            var donHangList = db.DonDatHangs
                .Where(dh => dh.KhachHang.TenKhachHang == tenKH)
                .ToList();

            ViewBag.DonHangList = donHangList;
            var khInfo = db.KhachHangs.FirstOrDefault(c => c.TenKhachHang == tenKH);
            return View(khInfo);
        }

        [HttpPost]
        public ActionResult UpdateCustomerInfo(KhachHang kh)
        {
            if (ModelState.IsValid)
            {
                // Lấy ra KH dựa vào id
                var tenKH = ((KhachHang)Session["KhachHang"]).TenKhachHang;
                var existingCustomer = db.KhachHangs.FirstOrDefault(c => c.TenKhachHang == tenKH);

                if (existingCustomer != null)
                {
                    // Update info KH
                    existingCustomer.TenKhachHang = kh.TenKhachHang;
                    existingCustomer.DiaChi = kh.DiaChi;
                    existingCustomer.Email = kh.Email;
                    existingCustomer.SoDienThoai = kh.SoDienThoai;
                    existingCustomer.SoThe = kh.SoThe;
                    existingCustomer.NgayHH = kh.NgayHH;
                    existingCustomer.CVV = kh.CVV;


                    db.SaveChanges();


                    return RedirectToAction("General", new { tenKH = kh.TenKhachHang });
                }
                else
                {

                    return HttpNotFound();
                }
            }


            return View("Home", "Index");
        }


    }
}