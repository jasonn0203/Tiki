using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tiki.Models;

namespace Tiki.Controllers
{
    public class ProductController : Controller
    {
        readonly TikiDatabase db = new TikiDatabase();

        // GET: Detail
        public ActionResult Detail(string tenSP)
        {
            var sanPham = db.SanPhams.FirstOrDefault(sp => sp.TenSanPham == tenSP);

            return View(sanPham);
        }
    }
}