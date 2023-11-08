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
            return View(cartItems);
        }






        /*      [HttpPost]
              public ActionResult AddToCart(int maSP, int soLuong)
              {
                  if (Session["KhachHang"] == null)
                  {
                      return RedirectToAction("SignIn", "User");
                  }

                  // Lấy thông tin sản phẩm từ cơ sở dữ liệu, ví dụ:
                  var sanPham = db.SanPhams.FirstOrDefault(sp => sp.MaSP == maSP);

                  if (sanPham == null || soLuong <= 0)
                  {
                      // Xử lý lỗi, ví dụ: sản phẩm không tồn tại hoặc số lượng không hợp lệ.
                      return HttpNotFound();
                  }

                  // Lấy giỏ hàng từ Session, nếu không có thì tạo mới
                  var gioHang = Session["GioHang"] as List<GioHang> ?? new List<GioHang>();

                  // Tìm sản phẩm trong giỏ hàng (nếu có)
                  var gioHangItem = gioHang.FirstOrDefault(item => item.MaSP == maSP);

                  if (gioHangItem != null)
                  {
                      // Nếu sản phẩm đã có trong giỏ hàng, cập nhật số lượng.
                      gioHangItem.SoLuong += (short)soLuong;
                  }
                  else
                  {

                      // Nếu sản phẩm chưa có trong giỏ hàng, thêm vào giỏ hàng.

                      gioHangItem = new GioHang
                      {
                          MaSP = maSP,
                          SoLuong = (short)soLuong,
                          MaKH = ((KhachHang)Session["KhachHang"]).MaKH,
                          TongTien = soLuong * sanPham.Gia


                      };

                      db.GioHangs.Add(gioHangItem);
                      db.SaveChanges();

                  }

                  // Lưu giỏ hàng vào Session
                  Session["GioHang"] = gioHang;

                  // Sau khi thêm sản phẩm vào giỏ hàng, bạn có thể chuyển người dùng đến trang giỏ hàng hoặc trang khác.
                  return RedirectToAction("Checkout", new { maKH = ((KhachHang)Session["KhachHang"]).MaKH });
              }*/


        public ActionResult AddToCart(int maSP, int soLuong)
        {
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
            decimal TongTien = 0;
            List<GioHang> gioHang = db.GioHangs.Where(item => item.MaKH == maKH).ToList();
            if (gioHang != null)
                TongTien = (decimal)gioHang.Sum(sp => sp.TongTien);
            return TongTien;
        }

    }
}