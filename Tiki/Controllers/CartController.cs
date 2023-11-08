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
        readonly TikiDatabase db = new TikiDatabase();
        // GET: Cart
        public ActionResult Checkout(int maKH)
        {
            if (Session["KhachHang"] == null)
            {
                return RedirectToAction("SignIn", "User");
            }

            var cartItems = db.GioHangs.Where(item => item.MaKH == maKH).ToList();

            var cartQuantity = db.GioHangs.Count(item => item.MaKH == maKH);

            ViewData["SoLuongSP"] = cartQuantity;
            ViewBag.TongTienGioHang = TinhTongTien(maKH);
            return View(cartItems);
        }



        public ActionResult AddToCart(int maSP, int soLuong)
        {

            if (Session["KhachHang"] == null)
            {
                return RedirectToAction("SignIn", "User");
            }
            int maKH = ((KhachHang)Session["KhachHang"]).MaKH;

            var existingCartItem = db.GioHangs.FirstOrDefault(item => item.MaKH == maKH && item.MaSP == maSP);

            if (existingCartItem != null)
            {
                existingCartItem.SoLuong += (short)soLuong;
                existingCartItem.TongTien = existingCartItem.SoLuong * existingCartItem.SanPham.Gia;
            }
            else
            {
                var newCartItem = new GioHang
                {
                    MaKH = maKH,
                    MaSP = maSP,
                    SoLuong = (short?)soLuong,
                    TongTien = soLuong * db.SanPhams.Find(maSP).Gia
                };
                db.GioHangs.Add(newCartItem);

            }

            db.SaveChanges();
            

            return RedirectToAction("Checkout", new { maKH = ((KhachHang)Session["KhachHang"]).MaKH });
        }

        private decimal TinhTongTien(int maKH)
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

                // Tính tổng giá trị giỏ hàng
                totalPrice += subtotal + shipPrice;
            }

            return totalPrice;
        }


    }
}