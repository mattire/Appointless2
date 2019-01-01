using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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

        //// GET: Bookings/ScheduleBookingView/5
        //public ActionResult BookingView(int id)
        //{
        //    var sched = db.Schedules.FirstOrDefault(s => s.Id == id);
        //    
        //    return View();
        //}

        //// GET: Bookings
        //public ActionResult Index()
        //{
        //    var now = DateTime.Now;
        //    var idsAndNames = db.Schedules.Select(s => new { Id = s.Id, Name = s.Name }).ToList();
        //    return View(idsAndNames);
        //    
        //    //var bookings = db.Bookings.Include(b => b.Schedule);
        //    //return View(bookings.ToList());
        //}

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
            //vm.EventTime = timeSlot.TimeOfDay
            return View("Book", vm);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        //public ActionResult Create(BookingWeekVM model)
        public ActionResult Create([Bind(Include = "Email, BookerName, PhoneNumber")] BookingVM model)
        {
            if (ModelState.IsValid)
            {
                var timeSlotId = int.Parse(Request.Form["TimeSlotId"]);
                string eventDate = Request.Form["EventDate"];

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
                };
                //db.Schedules.First().TimeSlots.SelectMany(ts=>ts.Bookings.Where(b=>b.))
                db.Bookings.Add(book);
                db.SaveChanges();
                Utils.EmailManager.SendConfirmationMail(book.Email, book.UUID);
                ViewBag.Email = model.Email;
                return View("CheckEmail");
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors);

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
                //ViewBag.Message = "Varaus vahvistettu";
                //return View("Message");
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

        //// GET: Bookings/Details/5
        //public ActionResult Details(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Booking booking = db.Bookings.Find(id);
        //    if (booking == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(booking);
        //}

        //// GET: Bookings/Create
        //public ActionResult Create()
        //{
        //    ViewBag.ScheduleID = new SelectList(db.Schedules, "Id", "Name");
        //    return View();
        //}

        //// POST: Bookings/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Time,LengthMinutes,Email,Name,Phone,Status,ScheduleID")] Booking booking)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Bookings.Add(booking);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.ScheduleID = new SelectList(db.Schedules, "Id", "Name", booking.ScheduleID);
        //    return View(booking);
        //}

        //// GET: Bookings/Edit/5
        //public ActionResult Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Booking booking = db.Bookings.Find(id);
        //    if (booking == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.ScheduleID = new SelectList(db.Schedules, "Id", "Name", booking.ScheduleID);
        //    return View(booking);
        //}

        //// POST: Bookings/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,Time,LengthMinutes,Email,Name,Phone,Status,ScheduleID")] Booking booking)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(booking).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.ScheduleID = new SelectList(db.Schedules, "Id", "Name", booking.ScheduleID);
        //    return View(booking);
        //}

        //// GET: Bookings/Delete/5
        //public ActionResult Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Booking booking = db.Bookings.Find(id);
        //    if (booking == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(booking);
        //}

        //// POST: Bookings/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(long id)
        //{
        //    Booking booking = db.Bookings.Find(id);
        //    db.Bookings.Remove(booking);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
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
