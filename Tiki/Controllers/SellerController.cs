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
        public ActionResult Index()
        {
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