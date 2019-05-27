using AppointLess2.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
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
            string returnUrl = Request.Form["return_url"];

            BookingVM vm = new BookingVM();
            vm.EventDate = eventDate;
            vm.TimeSlot = timeSlot;
            vm.TimeSlotId = timeSlot.Id;
            vm.ReturnUrl = returnUrl;
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
            string returnUrl = Request.Form["return_url"];

            bool availabilitySuccess = false;
            if (ModelState.IsValid)
            {
                using (TransactionScope tc = new TransactionScope(TransactionScopeOption.Required))
                {
                    DateTime eventDT = Utils.Utils.FromString(eventDate);

                    var book = new Booking()
                    {
                        Email = model.Email,
                        Name = model.BookerName,
                        Phone = model.PhoneNumber,
                        Time = eventDT,
                        TimeSlotID = timeSlotId,
                        UUID = Guid.NewGuid(),
                        Status = 1,
                        Descrption = description
                    };
                    //db.Schedules.First().TimeSlots.SelectMany(ts=>ts.Bookings.Where(b=>b.))

                    // check if is still free
                    if (!Utils.BookingManager.CheckIfBookingForSlotIsStillAwailable(book, db))
                    {
                        availabilitySuccess = false;
                    }
                    else
                    {
                        availabilitySuccess = true;
                        db.Bookings.Add(book);
                        db.SaveChanges();
                        //Utils.EmailManager.SendConfirmationMail(book.Email, book.UUID); // throws if fails
                        //ViewBag.Email = model.Email;
                    }

                    tc.Complete();
                }
                if (availabilitySuccess)
                {
                    model.ReturnUrl = returnUrl;
                    return View("Reserved", model);
                }
                else
                {
                    return View("ReservedError", model);
                }
            }
            //var errors = ModelState.Values.SelectMany(v => v.Errors);
            //errors.First().
            model.TimeSlot = db.TimeSlots.Find(timeSlotId);
            model.EventDate = eventDate;

            return View("Reserve", model);
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