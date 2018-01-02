using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parents_Bank_Application.Controllers
{
    public class HomeController : Controller
    {
        //[Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Financial_resources()
        {
           
            return View();
        }
        public ActionResult Bank_Account()
        {
            return RedirectToAction("Index", "Bank_Account");
        }
        public ActionResult Transactions()
        {
            return RedirectToAction("Index", "Transactions");
        }
        public ActionResult WishList_Item()
        {
            return RedirectToAction("Index", "WishList_Item");
        }
        public ActionResult About()
        {
            ViewBag.Message = "About Us";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Us";

            return View();
        }
    }
}