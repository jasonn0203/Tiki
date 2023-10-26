using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tiki.Controllers
{
    public class SellerController : Controller
    {
        // GET: Seller
        public ActionResult Dashboard()
        {
            ViewBag.Tab = "Dashboard";
            return View();
        }

        public ActionResult Management()
        {
            ViewBag.Tab = "Management";

            return View();
        }

        public ActionResult SellerSignUp()
        {
            return View();
        }

        public ActionResult SellerSignIn()
        {
            return View();
        }
    }
}