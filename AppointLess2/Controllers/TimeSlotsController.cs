//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using AppointLess2;

//namespace AppointLess2.Controllers
//{
//    [Authorize]
//    public class TimeSlotsController : Controller
//    {
//        private Entities db = new Entities();

//        // GET: TimeSlots/schedId
//        public ActionResult Index(long? schedId)
//        {
//            //var timeSlots = db.TimeSlots.Include(t => t.Schedule);
//            var timeSlots = db.TimeSlots.Where(ts => ts.ScheduleID == schedId);//.Include(t => t.Schedule);
//            return View(timeSlots.ToList());
//        }

//        // GET: TimeSlots/Details/5
//        public ActionResult Details(long? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            TimeSlot timeSlot = db.TimeSlots.Find(id);
//            if (timeSlot == null)
//            {
//                return HttpNotFound();
//            }
//            return View(timeSlot);
//        }

//        // GET: TimeSlots/Create
//        public ActionResult Create()
//        {
//            ViewBag.ScheduleID = new SelectList(db.Schedules, "Id", "Name");
//            return View();
//        }

//        // POST: TimeSlots/Create
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create([Bind(Include = "Id,DaysOfWeek,TimeOfDay,LengthMinutes,ScheduleID")] TimeSlot timeSlot)
//        {
//            if (ModelState.IsValid)
//            {
//                db.TimeSlots.Add(timeSlot);
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            ViewBag.ScheduleID = new SelectList(db.Schedules, "Id", "Name", timeSlot.ScheduleID);
//            return View(timeSlot);
//        }

//        // GET: TimeSlots/Edit/5
//        public ActionResult Edit(long? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            TimeSlot timeSlot = db.TimeSlots.Find(id);
//            if (timeSlot == null)
//            {
//                return HttpNotFound();
//            }
//            ViewBag.ScheduleID = new SelectList(db.Schedules, "Id", "Name", timeSlot.ScheduleID);
//            return View(timeSlot);
//        }

//        // POST: TimeSlots/Edit/5
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "Id,DaysOfWeek,TimeOfDay,LengthMinutes,ScheduleID")] TimeSlot timeSlot)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Entry(timeSlot).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }
//            ViewBag.ScheduleID = new SelectList(db.Schedules, "Id", "Name", timeSlot.ScheduleID);
//            return View(timeSlot);
//        }

//        // GET: TimeSlots/Delete/5
//        public ActionResult Delete(long? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            TimeSlot timeSlot = db.TimeSlots.Find(id);
//            if (timeSlot == null)
//            {
//                return HttpNotFound();
//            }
//            return View(timeSlot);
//        }

//        // POST: TimeSlots/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(long id)
//        {
//            TimeSlot timeSlot = db.TimeSlots.Find(id);
//            db.TimeSlots.Remove(timeSlot);
//            db.SaveChanges();
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}
