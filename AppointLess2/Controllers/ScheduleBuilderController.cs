﻿using AppointLess2.ViewModels.ScheduleViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
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


        public ActionResult DeleteTS(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeSlot ts = db.TimeSlots.Find(id);
            if (ts == null)
            {
                return HttpNotFound();
            }
            if (!Utils.AccessCheck.IsEntityCreator(ts, User)) { return new HttpUnauthorizedResult(); }
            return View(ts);
        }

        [HttpPost, ActionName("DeleteTS")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTS(int id, FormCollection collection)
        {
            TimeSlot timeSlot = db.TimeSlots.Find(id);
            var schedId = timeSlot.ScheduleID;
            db.TimeSlots.Remove(timeSlot);
            db.SaveChanges();
            return RedirectToAction("Edit", new { Id = schedId });
        }

        public ActionResult EditTS(int id)
        {
            var ts = db.TimeSlots.FirstOrDefault(t => t.Id == id);
            if (!Utils.AccessCheck.IsEntityCreator(ts, User)) { return new HttpUnauthorizedResult(); }

            var tsVM = new TimeSlotVM(ts);
            return View("EditTimeSlot", tsVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTS([Bind(Include = "TimeOfDay,LengthMinutes")] TimeSlotVM vm)
        {

            if (ModelState.IsValid)
            {
                //var chedIdStr = Request.Form["ScheduleID"];
                //slot.ScheduleID= int.Parse(chedIdStr);
                ResetIdsFromReq(vm, Request);
                TimeSlot slot = vm.ToTS(Request, db);
                //if (slot.Schedule == null) {
                //    slot.Schedule = db.Schedules.Find(slot.ScheduleID);
                //}
                //var oldSlot = db.TimeSlots.Find(slot.Id);
                //if (oldSlot == null) { return new HttpNotFoundResult(); }
                //slot.Schedule = oldSlot.Schedule;

                if (!Utils.AccessCheck.IsEntityCreator(slot, User)) { return new HttpUnauthorizedResult(); }

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
            var sch = db.Schedules.FirstOrDefault(s => s.Id == scheduleID);

            ActionResult excep = Utils.AccessCheck.CheckEntity(sch, User);
            if (excep != null) { return excep; }

            var tsVM = new TimeSlotVM(new TimeSlot(), sch);
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
                TimeSlot ts = timeSlotVM.ToTS(Request, db, schedID);

                // Check that user is the owner of specified schedule:
                ActionResult excep = Utils.AccessCheck.CheckEntity(ts, User);
                if (excep != null) { return excep; }

                //ts.ScheduleID = schedID;

                db.TimeSlots.Add(ts);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = schedID });
                //return View("Edit", new { id = schedID });
            }

            //ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", timeSlotVM.UserID);
            return View(timeSlotVM);
        }


        //// GET: SheduleBuilder/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        // GET: SheduleBuilder/Edit/5
        public ActionResult Edit(int id)
        {
            var sched = db.Schedules.FirstOrDefault(s => s.Id == id);
            if (sched != null)
            {
                if (!Utils.AccessCheck.IsEntityCreator(sched, User)) { return new HttpUnauthorizedResult(); }

                var svm = new ScheduleVM(sched);
                return View("Builder", svm);
            }
            else {
                return HttpNotFound();
                //return View("Builder", null);
            }
        }

        // POST: SheduleBuilder/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                var sched = db.Schedules.FirstOrDefault(s => s.Id == id);
                if (!Utils.AccessCheck.IsEntityCreator(sched, User)) { return new HttpUnauthorizedResult(); }

                var name = collection["Name"];
                var start   = int.Parse(collection["DailyStartTime"]);
                var end     = int.Parse(collection["DailyEndTime"]);
                var show    = Utils.Utils.CheckBoxToBool(collection["ShowOnFrontPage"]);
                System.Diagnostics.Debug.WriteLine(show.GetType().Name);
                System.Diagnostics.Debug.WriteLine(show);
                sched.Name = name;
                sched.OnFrontPage = show;
                if (start < end) {
                    sched.StartOfDay = start;
                    sched.EndOfDay   = end;
                }
                db.Entry(sched).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = id });
            }
            catch
            {
                //return View();
                return RedirectToAction("Edit", new { id = id });
            }
        }

        //// GET: SheduleBuilder/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}
        
        //// POST: SheduleBuilder/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here
        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
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
