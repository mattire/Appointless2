﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using AppointLess2;
using AppointLess2.ViewModels;

namespace AppointLess2.Controllers
{
    

    public class BookingsController : Controller
    {
        private Entities db  = new Entities();
        private static Random rnd = new Random();


        private Object bookingCreationLock = new Object(); // prevent double bookings any time

        //*
        // GET: Temp
        public ActionResult Index(int? schedID, string yearDotMothDotDay)
        {
            //DateTime? weekStart = null;
            var weekStart = Utils.Utils.GetStartOfCurrentWeek();
            if (yearDotMothDotDay != null)
            {
                var parsed = DateTime.ParseExact(yearDotMothDotDay, "yyyy.M.d", CultureInfo.InvariantCulture);
                weekStart = Utils.Utils.WeekStartByDateTime(parsed);
            }
            if (schedID != null)
            {
                var sched = db.Schedules.Find(schedID);

                ViewModels.BookingWeekVM weekVM = new ViewModels.BookingWeekVM(sched, (DateTime)weekStart);
                return View("WeekView", weekVM);
            }
            return null;
        }
        //*/

        // GET: WeekView/5
        public ActionResult BookingView(string name)
        {
            var weekStart = Utils.Utils.GetStartOfCurrentWeek();
            var sched = db.Schedules.FirstOrDefault(s => s.Name == name);
            ViewModels.BookingWeekVM weekVM = new ViewModels.BookingWeekVM(sched, weekStart);
            return View("WeekView", weekVM);
        }

        // GET: WeekView/5
        public ActionResult WeekView(int schedule)
        {
            var weekStart = Utils.Utils.GetStartOfCurrentWeek();
            var sched = db.Schedules.Find(schedule);
            ViewModels.BookingWeekVM weekVM = new ViewModels.BookingWeekVM(sched, weekStart);
            return View(weekVM);
        }

        public ActionResult Next(int? Day, int? Month, int? Year, int? Schedule)
        {
            var dt = new DateTime((int)Year, (int)Month, (int)Day);
            var sched = db.Schedules.Find(Schedule);
            var next = dt.AddDays(7);
            ViewModels.BookingWeekVM weekVM = new ViewModels.BookingWeekVM(sched, next);
            return View("WeekView", weekVM);
        }

        public ActionResult Previous(int Day, int Month, int Year, int? Schedule)
        {
            var dt = new DateTime((int)Year, (int)Month, (int)Day);
            var sched = db.Schedules.Find(Schedule);
            var previous = dt.AddDays(-7);
            ViewModels.BookingWeekVM weekVM = new ViewModels.BookingWeekVM(sched, previous);
            return View("WeekView", weekVM);
        }

        // GET: Bookings/CreateBooking1
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBooking1()
        {
            var eventDate = Request.Form["Booking.EventDate"];
            var timeSlotId = Request.Form["Booking.TimeSlotId"];
            var timeSlot = db.TimeSlots.Find(int.Parse(timeSlotId));
            string returnUrl = Request.Form["return_url"];

            BookingVM vm = new BookingVM();
            vm.EventDate = eventDate;
            vm.TimeSlot = timeSlot;
            vm.TimeSlotId = timeSlot.Id;
            
            vm.Number1 = rnd.Next(9);
            vm.Number2 = rnd.Next(9);
            vm.ReturnUrl = returnUrl;
            //vm.EventTime = timeSlot.TimeOfDay
            return View("Book", vm);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        //public ActionResult Create(BookingWeekVM model)
        public ActionResult Create([Bind(Include = "Email, BookerName, PhoneNumber")] BookingVM model)
        {
            int    timeSlotId = -1;
            string eventDate  = null;
            string description = null;

            int checkRes = int.Parse(Request.Form["CheckField"]);
            int num1 = int.Parse(Request.Form["Number1"]);
            int num2 = int.Parse(Request.Form["Number2"]);
            string returnUrl = Request.Form["return_url"];

            if (checkRes!= (num1 + num2))
            {
                ModelState.AddModelError("CheckField", "Väärä vastaus");
            }

            timeSlotId = int.Parse(Request.Form["TimeSlotId"]);
            eventDate = Request.Form["EventDate"];
            description = Request.Form["Description"];

            bool availabilitySuccess = false;
            if (ModelState.IsValid)
            {
                lock (bookingCreationLock) {
                    using (TransactionScope tc = new TransactionScope(TransactionScopeOption.Required))
                    {
                        DateTime eventDT = Utils.Utils.FromString(eventDate);

                        var book = new Booking()
                        {
                            Email = model.Email,
                            Name  = model.BookerName,
                            Phone = model.PhoneNumber,
                            Time  = eventDT,
                            TimeSlotID = timeSlotId,
                            UUID = Guid.NewGuid(),
                            Status = 0,
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
                            Utils.EmailManager.SendConfirmationMail(book.Email, book.UUID); // throws if fails
                            ViewBag.Email = model.Email;
                            ViewBag.ReturnUrl = returnUrl;
                        }

                        tc.Complete();
                    }
                }
                if (availabilitySuccess)
                {
                    return View("CheckEmail");
                }
                else {
                    return View("BookedError", model);
                }
            }
            //var errors = ModelState.Values.SelectMany(v => v.Errors);

            model.TimeSlot = db.TimeSlots.Find(timeSlotId);
            model.EventDate = eventDate;
            model.Number1 = rnd.Next(100);
            model.Number2 = rnd.Next(100);
            return View("Book", model);
        }


        // Confirmation, Cancellation
        [AllowAnonymous]
        // GET: /Booking/ClientManage?strGuid=8aad565a-7226-4365-a123-92fed8ce45d1"
        [HttpGet]
        public ActionResult ClientManage(string strGuid)
        {
            Guid guid;
            Booking bkng = null;
            if (Guid.TryParse(strGuid, out guid))
            {
                bkng = db.Bookings.FirstOrDefault(c => c.UUID == guid);
                return View("ClientManage", bkng);
            }
            ViewBag.Message = "Varausta ei löytynyt";
            return View("Message");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm() {
            int bkngId = int.Parse(Request.Form["Id"]);
            var bkng = db.Bookings.Find(bkngId);
            if (bkng != null)
            {
                bkng.Status = 1;
                db.SaveChanges();
                return RedirectToAction(
                        "ClientManage",
                        new { strGuid = bkng.UUID });
            }
            else {
                ViewBag.Message = "Varausta ei löytynyt";
                return View("Message");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel()
        {
            int bkngId = int.Parse(Request.Form["Id"]);
            var bkng = db.Bookings.Find(bkngId);
            if (bkng != null) {
                db.Bookings.Remove(bkng);
                db.SaveChanges();
                ViewBag.Message = "Varaus poistettu";
                return View("Message");
            }
            ViewBag.Message = "Varausta ei löytynyt";
            return View("Message");
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
