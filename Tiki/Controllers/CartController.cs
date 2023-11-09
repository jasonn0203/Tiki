using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tiki.Models;

namespace Tiki.Controllers
{
    public class CartController : Controller
    {
        readonly TikiEntities db = new TikiEntities();
        // GET: Cart
        public ActionResult Checkout(int maKH)
        {
            if (Session["KhachHang"] == null)
            {
                return RedirectToAction("SignIn", "User");
            }

            List<GioHang> gioHang = GetGioHang();

            var cartItems = gioHang.Where(item => item.MaKH == maKH).ToList();


            ViewBag.TongSL = TinhTongSL();
            ViewBag.TongTienGioHang = TinhTongTien();
            return View(cartItems);
        }

        public List<GioHang> GetGioHang()
        {
            List<GioHang> gioHang = Session["GioHang"] as List<GioHang>;

            if (gioHang == null)
            {
                gioHang = new List<GioHang>();
                Session["GioHang"] = gioHang;
            }
            return gioHang;
        }


        public ActionResult AddToCart(int maSP, short soLuong)
        {

            if (Session["KhachHang"] == null)
            {
                return RedirectToAction("SignIn", "User");
            }
            var maKH = ((KhachHang)Session["KhachHang"]).MaKH;


            List<GioHang> gioHang = GetGioHang();
            GioHang sp = gioHang.Find(s => s.MaSP == maSP);

            var existingCartItem = gioHang.FirstOrDefault(item => item.MaKH == maKH && item.MaSP == maSP);

            if (existingCartItem != null)
            {
                existingCartItem.SoLuong += soLuong;


            }
            else if (sp == null)
            {
                sp = new GioHang(maSP, soLuong);
                sp.MaKH = maKH;
                gioHang.Add(sp);
                return RedirectToAction("Checkout", new { maKH = maKH });
            }

            return RedirectToAction("Checkout", new { maKH = maKH });
        }


        public ActionResult DeleteFromCart(int MaSP)
        {
            List<GioHang> gioHang = GetGioHang();
            //Lấy sản phẩm trong giỏ hàng
            var sanpham = gioHang.FirstOrDefault(sp => sp.MaSP == MaSP);
            if (sanpham != null)
            {
                gioHang.RemoveAll(sp => sp.MaSP == MaSP);
                return RedirectToAction("Checkout", new { maKH = ((KhachHang)Session["KhachHang"]).MaKH }); //Quay về trang giỏ hàng
            }
            if (gioHang.Count == 0) //Quay về trang chủ nếu giỏ hàng không có gì
                return RedirectToAction("Index", "Home");
            return RedirectToAction("Checkout", new { maKH = ((KhachHang)Session["KhachHang"]).MaKH });
        }


        private int TinhTongSL()
        {
            int tongSL = 0;
            List<GioHang> gioHang = GetGioHang();
            if (gioHang != null)
                tongSL = gioHang.Sum(sp => sp.SoLuong);

            return tongSL;
        }

        private decimal TinhTongTien()
        {
            decimal TongTien = 0;
            List<GioHang> gioHang = GetGioHang();
            if (gioHang != null)
                TongTien = gioHang.Sum(sp => sp.TongTien);
            TongTien += 25000;
            return TongTien;
        }


        /* private int TinhTongSLCart(int maKH)
         {
             var cartQuantity = db.GioHangs.Count(item => item.MaKH == maKH);

             return cartQuantity;
         }*/



        /*     private decimal TinhTongTien(int maKH)
             {
                 //Lấy giỏ hàng
                 var cartItems = db.GioHangs.Where(g => g.MaKH == maKH).ToList();

                 decimal totalPrice = 0;
                 decimal shipPrice = 25000;

                 // Lặp qua các item trong giỏ hàng
                 foreach (var cartItem in cartItems)
                 {
                     // Get giá SP
                     decimal productPrice = (decimal)db.SanPhams.Find(cartItem.MaSP).Gia;

                     // Tính tổng giá từng item
                     decimal subtotal = (decimal)(productPrice * cartItem.SoLuong);

                     // Cộng tổng giá từng item vào tổng tiền
                     totalPrice += subtotal;
                 }

                 // Cộng thêm phí ship 
                 totalPrice += shipPrice;

                 return totalPrice;
             }*/



        public ActionResult CartPartial(int maKH)
        {
            ViewBag.TongSL = TinhTongSL();
            return PartialView();
        }


        public ActionResult Pay()
        {
            return View();
        }




        public ActionResult SuccessfulPay()
        {

            return View();
        }
    }
}