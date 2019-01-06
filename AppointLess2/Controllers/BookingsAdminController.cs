using AppointLess2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppointLess2.Controllers
{
    [Authorize]
    public class BookingsAdminController : Controller
    {
        private Entities db = new Entities();

        [Authorize]
        //// GET: BookingsAdmin
        // GET: WeekViewAdmin/5
        public ActionResult WeekViewAdmin(int schedule)
        {
            var sched = db.Schedules.Find(schedule);
            var dt = Utils.Utils.GetStartOfCurrentWeek();            

            BookingWeekVM vm = new BookingWeekVM(sched, dt, BookingViewMode.Admin) ;
            //return View("~/Views/Bookings/WeekView", vm);
            return View("WeekView", vm);
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