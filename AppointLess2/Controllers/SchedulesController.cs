﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppointLess2;
using Microsoft.AspNet.Identity;

namespace AppointLess2.Controllers
{
    [Authorize]
    public class SchedulesController : Controller
    {
        private Entities db = new Entities();

        // GET: Schedules
        public ActionResult Index()
        {
            Console.WriteLine(User.Identity);
            Console.WriteLine(User.Identity.Name);
            System.Diagnostics.Debug.WriteLine("***************");
            System.Diagnostics.Debug.WriteLine(User.Identity.Name);
            
            //var schedules = db.Schedules.Include(s => s.AspNetUser).Where(s=>s.UserID==User.Identity);
            var schedules = db.Schedules.Include(s => s.AspNetUser).Where(s => s.AspNetUser.Email == User.Identity.Name);
            return View(schedules.ToList());
        }

        // GET: Schedules/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schedule schedule = db.Schedules.Find(id);
            if (schedule == null)
            {
                return HttpNotFound();
            }
            return View(schedule);
        }

        // GET: Schedules/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        // POST: Schedules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Name,UserID")] Schedule schedule)
        public ActionResult Create([Bind(Include = "Name,UserID")] Schedule schedule)
        {
            var id =Request.Form["UserId"];
            System.Diagnostics.Debug.WriteLine(User.Identity.Name);
            System.Diagnostics.Debug.WriteLine(Request.LogonUserIdentity.User.Value);
            System.Diagnostics.Debug.WriteLine("");
            string userId = User.Identity.GetUserId();

            schedule.UserID = User.Identity.GetUserId();


            if (ModelState.IsValid)
            {
                
                db.Schedules.Add(schedule);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", schedule.UserID);
            return View(schedule);
        }

        // GET: Schedules/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schedule schedule = db.Schedules.Find(id);
            if (schedule == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", schedule.UserID);
            return View(schedule);
        }

        // POST: Schedules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,UserID")] Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                db.Entry(schedule).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            //ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", schedule.UserID);
            //ViewBag.UserID = new SelectList(db.Schedules, "Id,Name,UserID", schedule.UserID);
            return View(schedule);
        }

        // GET: Schedules/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schedule schedule = db.Schedules.Find(id);
            if (schedule == null)
            {
                return HttpNotFound();
            }
            return View(schedule);
        }

        // POST: Schedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Schedule schedule = db.Schedules.Find(id);
            db.Schedules.Remove(schedule);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
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
