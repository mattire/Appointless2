using System;
using System.Collections.Generic;
using System.Globalization;
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
        public ActionResult Index(string yearDotMothDotDay)
        {
            //DateTime? weekStart = null;
            var weekStart = Utils.Utils.GetStartOfCurrentWeek();
            if (yearDotMothDotDay != null) {
                var parsed = DateTime.ParseExact(yearDotMothDotDay, "yyyy.M.d", CultureInfo.InvariantCulture);
                weekStart = Utils.Utils.WeekStartByDateTime(parsed);
            }

            var sched = db.Schedules.Find(1);
            ViewModels.BookingWeekVM weekVM = new ViewModels.BookingWeekVM(sched, (DateTime)weekStart);
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
