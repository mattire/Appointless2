using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppointLess2.Controllers
{
    // Scrapboard controller for dev ideas
    //[Authorize]
    public class TempController : Controller
    {
        private Entities db = new Entities();

        // GET: Temp
        public ActionResult Index()
        {
            
            var weekStart = Utils.Utils.GetStartOfCurrentWeek();
            var sched = db.Schedules.Find(1);
            ViewModels.BookingWeekVM weekVM = new ViewModels.BookingWeekVM(sched, weekStart);
            return View(weekVM);
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
