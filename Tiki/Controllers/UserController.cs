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

        TikiDatabase db = new TikiDatabase();
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



        public ActionResult SignIn()
        {
            return View();
        }

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