using AppointLess2.ViewModels.ScheduleViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppointLess2.Controllers
{
    [Authorize]
    public class ScheduleBuilderController : Controller
    {
        private Entities db = new Entities();

        // GET: SheduleBuilder
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EditTS(int id)
        {
            var ts = db.TimeSlots.FirstOrDefault(t => t.Id == id);
            var tsVM = new TimeSlotVM(ts);
            return View("EditTimeSlot", tsVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTS([Bind(Include = "TimeOfDay,LengthMinutes")] TimeSlotVM vm)
        {
            if (ModelState.IsValid)
            {
                //var schedID = int.Parse(Request.Form["ScheduleID"]);
                //var tsID = int.Parse(Request.Form["Id"]);
                ////var tsTimeOfDay = int.Parse(Request.Form["TimeOfDay"]);
                //TimeSlot slot = vm.ToTS(Request);
                //slot.ScheduleID = schedID;
                //slot.Id = tsID;

                ResetIdsFromReq(vm, Request);
                TimeSlot slot = vm.ToTS(Request);

                db.Entry(slot).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Edit", new { Id = schedID });
                return RedirectToAction("Edit", new { Id = vm.ScheduleID });
            }
            ResetIdsFromReq(vm, Request);
            //var tsVM = new TimeSlotVM(ts);
            return View("EditTimeSlot", vm);
        }

        private void ResetIdsFromReq(TimeSlotVM vm, HttpRequestBase req)
        {
            var schedID = int.Parse(Request.Form["ScheduleID"]);
            var tsID = int.Parse(Request.Form["Id"]);
            vm.Id = tsID;
            vm.ScheduleID = schedID;
            vm.SetDaysFromReq(req);
        }


        public ActionResult CreateTS(int scheduleID)
        {
            var tsVM = new TimeSlotVM(new TimeSlot());
            tsVM.ScheduleID = scheduleID;
            return View("CreateTimeSlot", tsVM);
        }

        // POST: Schedules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult CreateTS(TimeSlotVM timeSlotVM)
        public ActionResult CreateTS([Bind(Include = "TimeOfDay,LengthMinutes")] TimeSlotVM timeSlotVM)
        {
            if (ModelState.IsValid)
            {
                var schedID = int.Parse(Request.Form["ScheduleID"]);
                TimeSlot ts = timeSlotVM.ToTS(Request);
                ts.ScheduleID = schedID;
                db.TimeSlots.Add(ts);
                db.SaveChanges();
                return RedirectToAction("Edit", schedID);
            }

            //ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", timeSlotVM.UserID);
            return View(timeSlotVM);
        }


        // GET: SheduleBuilder/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }


        // GET: SheduleBuilder/Edit/5
        public ActionResult Edit(int id)
        {
            var sched = db.Schedules.FirstOrDefault(s => s.Id == id);
            var svm = new ScheduleVM(sched);
            
            return View("Builder", svm);
        }

        // POST: SheduleBuilder/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                var sched = db.Schedules.FirstOrDefault(s => s.Id == id);
                var name = collection["Name"];
                sched.Name = name;
                db.Entry(sched).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = id });
            }
            catch
            {
                return View();
            }
        }

        // GET: SheduleBuilder/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SheduleBuilder/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
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
