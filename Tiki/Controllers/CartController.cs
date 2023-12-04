using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Tiki.Models;
using Tiki.Models.Singleton;

namespace Tiki.Controllers
{
    public class CartController : Controller
    {
        private readonly TikiEntities db = DatabaseSingleton.Instance;
        private readonly CartFacade cartFacade;

        public CartController()
        {
            cartFacade = new CartFacade(db, HttpContext);
        }

        public int GetMaKH()
        {
            return UserAuthenSingleton.Instance.MaKH;
        }

        // GET: Cart
        public ActionResult Checkout(int maKH)
        {

            if (UserAuthenSingleton.Instance == null)
            {
                return RedirectToAction("SignIn", "User");
            }


            //List<GioHang> gioHang = GetGioHang();
            List<GioHang> gioHang = cartFacade.GetCartItems();


            var cartItems = gioHang.Where(item => item.MaKH == maKH).ToList();


            ViewBag.TongSL = TinhTongSL();
            ViewBag.TongTienGioHang = TinhTongTien();
            return View(cartItems);
        }

        /*        public List<GioHang> GetGioHang()
                {
                    List<GioHang> gioHang = Session["GioHang"] as List<GioHang>;

                    if (gioHang == null)
                    {
                        gioHang = new List<GioHang>();
                        Session["GioHang"] = gioHang;
                    }
                    return gioHang;
                }*/


        /*        public ActionResult AddToCart(int maSP, short soLuong)
                {

                    if (UserAuthenSingleton.Instance == null)
                    {
                        return RedirectToAction("SignIn", "User");
                    }
                    var maKH = GetMaKH();


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
                }*/


        public ActionResult AddToCart(int maSP, short soLuong)
        {
            if (UserAuthenSingleton.Instance == null)
            {
                return RedirectToAction("SignIn", "User");
            }

            var maKH = GetMaKH();
            cartFacade.AddToCart(maSP, soLuong, maKH);

            cartFacade.SaveChangesCart();

            return RedirectToAction("Checkout", new { maKH = maKH });
        }



        public ActionResult DeleteFromCart(int MaSP)
        {
            //List<GioHang> gioHang = GetGioHang();
            /* List<GioHang> gioHang = cartFacade.GetCartItems();
             //Lấy sản phẩm trong giỏ hàng
             var sanpham = gioHang.FirstOrDefault(sp => sp.MaSP == MaSP);
             if (sanpham != null)
             {
                 gioHang.RemoveAll(sp => sp.MaSP == MaSP);
                 return RedirectToAction("Checkout", new { maKH = GetMaKH() }); //Quay về trang giỏ hàng
             }
             if (gioHang.Count == 0) //Quay về trang chủ nếu giỏ hàng không có gì
                 return RedirectToAction("Index", "Home");*/


            cartFacade.RemoveFromCart(MaSP);
            cartFacade.SaveChangesCart();
            return RedirectToAction("Checkout", new { maKH = GetMaKH() });
        }

        public ActionResult DeleteAllFromCart()
        {
            //List<GioHang> gioHang = GetGioHang();
            /*List<GioHang> gioHang = cartFacade.GetCartItems();

            gioHang.Clear();*/

            cartFacade.ClearCart();
            cartFacade.SaveChangesCart();

            return RedirectToAction("Checkout", new { maKH = GetMaKH() });
        }

        public ActionResult UpdateCartQuantity(int maSP, FormCollection form)
        {
            //List<GioHang> gioHang = GetGioHang();
            List<GioHang> gioHang = cartFacade.GetCartItems();

            var sanpham = gioHang.SingleOrDefault(sp => sp.MaSP == maSP);
            //Ktra nếu có sp thì mới đc sửa sluong
            if (sanpham != null)
            {
                sanpham.SoLuong = short.Parse(form["soLuong"].ToString());
            }
            //Delay 0.75s
            Thread.Sleep(250);
            return RedirectToAction("Checkout", new { maKH = GetMaKH() });


        }

        public ActionResult Pay()
        {
            KhachHang kh = UserAuthenSingleton.Instance;
            if (kh == null)
            {
                return RedirectToAction("SignIn", "User");
            }

            // Lấy giỏ hàng hiện tại
            //List<GioHang> gioHang = GetGioHang();
            List<GioHang> gioHang = cartFacade.GetCartItems();


            // Tạo đơn đặt hàng
            DonDatHang donDatHang = new DonDatHang();
            donDatHang.MaKH = kh.MaKH;
            donDatHang.NgayDat = DateTime.Now;




            db.DonDatHangs.Add(donDatHang);
            db.SaveChanges();

            // Lưu vào chi tiết đơn hàng
            foreach (var sp in gioHang)
            {
                ChiTietDonHang chiTietDonHang = new ChiTietDonHang();
                chiTietDonHang.MaDonHang = donDatHang.MaDonHang;
                chiTietDonHang.MaSP = sp.MaSP;
                chiTietDonHang.SoLuong = sp.SoLuong;
                chiTietDonHang.DonGia = sp.Gia;
                db.ChiTietDonHangs.Add(chiTietDonHang);
            }

            kh.DaThanhToan = true;
            db.SaveChanges();



            // Xóa giỏ hàng
            Session["GioHang"] = null;

            return RedirectToAction("SuccessfulPay");
        }



        public ActionResult SuccessfulPay()
        {
            KhachHang kh = UserAuthenSingleton.Instance;

            if (kh != null && kh.DaThanhToan)
            {
                // Nếu khách hàng đã thanh toán, cho phép truy cập trang
                return View();
            }
            else
            {
                // Khách hàng chưa thanh toán, chuyển hướng hoặc hiển thị thông báo không cho phép truy cập
                return RedirectToAction("Index", "Home");
            }
        }



        //------------------------------------------------------------------------------------------------------
        private int TinhTongSL()
        {
            int tongSL = 0;
            //List<GioHang> gioHang = GetGioHang();
            List<GioHang> gioHang = cartFacade.GetCartItems();

            if (gioHang != null)
                tongSL = gioHang.Sum(sp => sp.SoLuong);

            return tongSL;
        }

        private decimal TinhTongTien()
        {
            decimal TongTien = 0;
            //List<GioHang> gioHang = GetGioHang();
            List<GioHang> gioHang = cartFacade.GetCartItems();

            if (gioHang != null)
                TongTien = gioHang.Sum(sp => sp.TongTien);
            TongTien += 25000;
            return TongTien;
        }




        public ActionResult CartPartial(int maKH)
        {
            ViewBag.TongSL = TinhTongSL();
            return PartialView();
        }


    }
}