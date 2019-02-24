﻿using AppointLess2.ViewModels;
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
            //if (schedule.AspNetUser.UserName != User.Identity.Name) { return null; }

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
        public new ActionResult WeekViewAdmin(int schedule)
        {
            var sched = db.Schedules.Find(schedule);
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
            return View("Cancel", bkng);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CancelConfirmed()
        {
            int bid = int.Parse(Request.Form["bookingID"]);
            var bkng = db.Bookings.Find(bid);
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