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

        // GET: Bookings/ScheduleBookingView/5
        public ActionResult BookingView(int id)
        {
            var sched = db.Schedules.FirstOrDefault(s => s.Id == id);
            
            return View();
        }

        // GET: Bookings
        public ActionResult Index()
        {
            var now = DateTime.Now;
            var idsAndNames = db.Schedules.Select(s => new { Id = s.Id, Name = s.Name }).ToList();
            return View(idsAndNames);
            
            //var bookings = db.Bookings.Include(b => b.Schedule);
            //return View(bookings.ToList());
        }

        // GET: WeekView/5
        public ActionResult WeekView(int schedule)
        {
            var weekStart = Utils.Utils.GetStartOfCurrentWeek();
            var sched = db.Schedules.Find(schedule);
            ViewModels.BookingWeekVM weekVM = new ViewModels.BookingWeekVM(sched, weekStart);
            //var start = weekVM.WeekDays.First();
            //var last = weekVM.WeekDays.Last();
            //var slotBookings = sched.TimeSlots.First().Bookings.Where(b => b.Time >= start && b.Time <= last);
            return View(weekVM);
        }

        public ActionResult Next(int? Day, int? Month, int? Year, int? Schedule)
        {
            var dt = new DateTime((int)Year, (int)Month, (int)Day);
            var sched = db.Schedules.Find(Schedule);
            var next = dt.AddDays(7);
            ViewModels.BookingWeekVM weekVM = new ViewModels.BookingWeekVM(sched, next);
            //WeekViewModel weekViewModel = new WeekViewModel(next);
            //SetUpEvents(ref weekViewModel);
            //return View("Index", weekViewModel);
            return View("WeekView", weekVM);
        }

        public ActionResult Previous(int Day, int Month, int Year, int? Schedule)
        {
            var dt = new DateTime((int)Year, (int)Month, (int)Day);
            var sched = db.Schedules.Find(Schedule);
            var previous = dt.AddDays(-7);
            ViewModels.BookingWeekVM weekVM = new ViewModels.BookingWeekVM(sched, previous);
            //WeekViewModel weekViewModel = new WeekViewModel(previous);
            //SetUpEvents(ref weekViewModel);
            //return View("Index", weekViewModel);
            return View("WeekView", weekVM);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        
        //public ActionResult Create(BookingWeekVM model)
        public ActionResult Create(BookingVM model)
        {
            try
            {
                var email = model.Email;
                System.Diagnostics.Debug.WriteLine(model.WeekStartYear);
                System.Diagnostics.Debug.WriteLine(model.EventDate);
                System.Diagnostics.Debug.WriteLine(model.EventTime);
                System.Diagnostics.Debug.WriteLine(email);
                // TODO: Add insert logic here
                var eventGuid = Guid.NewGuid();

                //var t = DiaryEvent.ToDateTimeAndDuration(model.WeekStartYear, model.EventDate, model.EventTime);
                //
                //var ev = DiaryEvent.CreateNewEvent(t.Item1, t.Item2, eventGuid, email);
                Utils.EmailManager.SendConfirmationMail(email, eventGuid);


                // Need to reload currently viewed week

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
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
