using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tiki.Models;

namespace Tiki.Controllers
{
    public class UserController : Controller
    {
        readonly TikiDatabase db = new TikiDatabase();
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





        public ActionResult Invoices()
        {
            return View();
        }

        public ActionResult General()
        {
            return View();
        }
    }
}