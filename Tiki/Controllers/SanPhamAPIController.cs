using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tiki.Models;

namespace Tiki.Controllers
{
    public class SanPhamAPIController : Controller
    {
        // GET: SanPhamAPI
        public JsonResult Index()
        {
            DataModel db = new DataModel();
           ArrayList dataList = db.Get("EXEC LAYTTSANPHAM;");
            return Json(dataList, JsonRequestBehavior.AllowGet);
        }



    }
}