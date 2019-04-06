using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppointLess2;
using AppointLess2.ViewModels;

namespace AppointLess2.Controllers
{
    

    public class BookingsController : Controller
    {
        private Entities db = new Entities();
        private static Random rnd = new Random();

        /*
        // GET: Temp
        public ActionResult Index(string yearDotMothDotDay)
        {
            //DateTime? weekStart = null;
            var weekStart = Utils.Utils.GetStartOfCurrentWeek();
            if (yearDotMothDotDay != null)
            {
                var parsed = DateTime.ParseExact(yearDotMothDotDay, "yyyy.M.d", CultureInfo.InvariantCulture);
                weekStart = Utils.Utils.WeekStartByDateTime(parsed);
            }

            var sched = db.Schedules.Find(1);
            ViewModels.BookingWeekVM weekVM = new ViewModels.BookingWeekVM(sched, (DateTime)weekStart);
            return View(weekVM);
        }
        */

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

            BookingVM vm = new BookingVM();
            vm.EventDate = eventDate;
            vm.TimeSlot = timeSlot;
            vm.TimeSlotId = timeSlot.Id;
            
            vm.Number1 = rnd.Next(15);
            vm.Number2 = rnd.Next(15);
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

            if (checkRes!= (num1 + num2))
            {
                ModelState.AddModelError("CheckField", "Väärä vastaus");
            }

            timeSlotId = int.Parse(Request.Form["TimeSlotId"]);
            eventDate = Request.Form["EventDate"];
            description = Request.Form["Description"];
            if (ModelState.IsValid)
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
                    Status = 0,
                    Descrption = description
                };
                //db.Schedules.First().TimeSlots.SelectMany(ts=>ts.Bookings.Where(b=>b.))
                db.Bookings.Add(book);
                db.SaveChanges();
                Utils.EmailManager.SendConfirmationMail(book.Email, book.UUID);
                ViewBag.Email = model.Email;
                return View("CheckEmail");
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors);

            model.TimeSlot = db.TimeSlots.Find(timeSlotId);
            model.EventDate = eventDate;
            model.Number1 = rnd.Next(100);
            model.Number2 = rnd.Next(100);
            return View("Book", model); ;
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
