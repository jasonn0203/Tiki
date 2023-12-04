using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tiki.Controllers;

namespace Tiki.Models
{
    public class CartFacade
    {

        private readonly TikiEntities db;
        private readonly HttpContextBase httpContext;

        public CartFacade(TikiEntities dbContext, HttpContextBase context)
        {
            db = dbContext;
            httpContext = new HttpContextWrapper(HttpContext.Current);
        }


        //---------------
        public List<GioHang> GetCartItems()
        {
            List<GioHang> gioHang = httpContext.Session["GioHang"] as List<GioHang>;

            if (gioHang == null)
            {
                gioHang = new List<GioHang>();
                httpContext.Session["GioHang"] = gioHang;
            }

            return gioHang;
        }


        public void AddToCart(int maSP, short soLuong, int maKH)
        {
            List<GioHang> gioHang = GetCartItems();
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
            }
        }


        public void RemoveFromCart(int maSP)
        {
            List<GioHang> gioHang = GetCartItems();
            var sanpham = gioHang.FirstOrDefault(sp => sp.MaSP == maSP);

            if (sanpham != null)
            {
                gioHang.RemoveAll(sp => sp.MaSP == maSP);
            }
        }


        public void ClearCart()
        {
            List<GioHang> gioHang = GetCartItems();
            gioHang.Clear();
        }


        public void SaveChangesCart()
        {
            httpContext.Session["GioHang"] = GetCartItems();
        }

    }
}