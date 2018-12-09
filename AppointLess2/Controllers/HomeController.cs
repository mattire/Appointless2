using AppointLess2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppointLess2.Controllers
{
    public class HomeController : Controller
    {
        private Entities db = new Entities();


        public ActionResult Index()
        {
            IQueryable<Schedule> scheds = null;

            if (User.Identity != null) {
                System.Diagnostics.Debug.WriteLine("Auth");
                scheds = DbUtils.GetUserSchedules(db, User);
            }

            return View(scheds);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}