using AppointLess2.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppointLess2.Controllers
{
    [Authorize]
    //public class BookingsAdminController : BookingsController
    public class BookingsAdminController : Controller
    {
        private Entities db = new Entities();


        [Authorize]
        public ActionResult Index(int? schedID, string yearDotMothDotDay)
        {
            var schedule = db.Schedules.Find(schedID);

            // TODO CHECK IF IT'S USERS SCHEDULE
            if (schedule.AspNetUser.UserName != User.Identity.Name) { return null; }

            //DateTime? weekStart = null;
            var weekStart = Utils.Utils.GetStartOfCurrentWeek();
            if (yearDotMothDotDay != null)
            {
                var parsed = DateTime.ParseExact(yearDotMothDotDay, "yyyy.M.d", CultureInfo.InvariantCulture);
                weekStart = Utils.Utils.WeekStartByDateTime(parsed);
            }

            //var sched = db.Schedules.Find(1);
            ViewModels.BookingWeekVM weekVM = new ViewModels.BookingWeekVM(schedule, (DateTime)weekStart);
            return View("WeekViewAdmin", weekVM);
        }


        [Authorize]
        //// GET: BookingsAdmin
        // GET: WeekViewAdmin/5
        public ActionResult WeekViewAdmin(int schedule)
        {
            var sched = db.Schedules.Find(schedule);
            if (sched.AspNetUser.UserName != User.Identity.Name) { return null; }

            var dt = Utils.Utils.GetStartOfCurrentWeek();            

            BookingWeekVM vm = new BookingWeekVM(sched, dt, BookingViewMode.Admin) ;
            //return View("~/Views/Bookings/WeekView", vm);

            return View("WeekView", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Cancel() {

            int bid = int.Parse(Request.Form["bookingID"]);
            var bkng = db.Bookings.Find(bid);
            //bkng.TimeSlot.Schedule.AspNetUser
            return View("Cancel", bkng);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReserveBooking()
            {
            var eventDate = Request.Form["Booking.EventDate"];
            var timeSlotId = Request.Form["Booking.TimeSlotId"];
            var timeSlot = db.TimeSlots.Find(int.Parse(timeSlotId));

            BookingVM vm = new BookingVM();
            vm.EventDate = eventDate;
            vm.TimeSlot = timeSlot;
            vm.TimeSlotId = timeSlot.Id;

            return View("Reserve", vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create(BookingWeekVM model)
        public ActionResult Create([Bind(Include = "Email, BookerName, PhoneNumber")] BookingVM model)
        {
            int timeSlotId = -1;
            string eventDate = null;
            string description = null;

            timeSlotId = int.Parse(Request.Form["TimeSlotId"]);
            eventDate = Request.Form["EventDate"];
            description = Request.Form["Description"];

            bool availabilitySuccess = false;
            if (ModelState.IsValid)
            {

            }
            return null;
        }


        [Authorize]
        [HttpPost]
        public ActionResult CancelConfirmed()
        {
            int bid = int.Parse(Request.Form["bookingID"]);
            var bkng = db.Bookings.Find(bid);
            if (bkng.TimeSlot.Schedule.AspNetUser.UserName != User.Identity.Name) { return null; }
            int schedId = bkng.TimeSlot.ScheduleID;
            db.Bookings.Remove(bkng);
            db.SaveChanges();
            return RedirectToAction("Index", "BookingsAdmin", new { schedID = schedId });
        }

        //public ActionResult Next(int? Day, int? Month, int? Year, int? Schedule)
        //{
        //    ActionResult ar = base.Next(Day, Month, Year, Schedule);

        //    return ar;
        //}

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