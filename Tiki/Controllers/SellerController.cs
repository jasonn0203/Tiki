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
        readonly TikiDatabase db = new TikiDatabase();


        // GET: Seller
        public ActionResult Dashboard()
        {
            if (Session["NhaCungCap"] == null)
            {
                return RedirectToAction("SellerSignIn", "Seller");
            }
            ViewBag.Tab = "Dashboard";


            return View();
        }

        public ActionResult Management()
        {
            if (Session["NhaCungCap"] == null)
            {
                return RedirectToAction("SellerSignIn", "Seller");
            }
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
            if (Session["NhaCungCap"] == null)
            {
                return RedirectToAction("SellerSignIn", "Seller");
            }
            ViewBag.Tab = "ProductList";


            /*Lấy ds sp*/
            // Lấy mã chủ nhà từ session
            var maNCC = ((NhaCungCap)Session["NhaCungCap"]).MaNCC;

            // Lấy danh sách các phòng thuộc mã chủ nhà
            List<SanPham> spList = db.SanPhams.Where(s => s.MaNCC == maNCC).ToList();

            if (spList == null || spList.Count == 0)
            {
                ViewBag.Message = "List sản phẩm hiện đang trống !";
                return View();
            }
            return View(spList);

        }


        /*THÊM SP*/
        public ActionResult AddProduct()
        {
            if (Session["NhaCungCap"] == null)
            {
                return RedirectToAction("SellerSignIn", "Seller");
            }
            // Thêm logic tại đây để hiển thị giao diện thêm sản phẩm
            ViewBag.PhanLoaiList = new SelectList(db.PhanLoaiSPs, "MaPhanLoai", "TenPhanLoai");

            var nhaCungCap = (NhaCungCap)Session["NhaCungCap"];
            ViewBag.MaNCC = nhaCungCap.MaNCC;

            return View();
        }

        [HttpPost]
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
    }

}