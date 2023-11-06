using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tiki.Models;

namespace Tiki.Controllers
{
    public class SellerController : Controller
    {

        TikiDatabase db = new TikiDatabase();
        // GET: Seller
        public ActionResult Dashboard()
        {
            ViewBag.Tab = "Dashboard";
            if (Session["NhaCungCap"] == null)
            {
                return RedirectToAction("SellerSignIn", "Seller");
            }

            return View();
        }

        public ActionResult Management()
        {
            ViewBag.Tab = "Management";

            return View();
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
            var checkUniqueEmail = db.NhaCungCaps.FirstOrDefault(n => n.Email.Equals(n.Email));

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
            return View();
        }

    }
}