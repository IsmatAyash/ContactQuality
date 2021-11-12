using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContactQuality.Models;

namespace ContactQuality.Controllers
{
    public class CQPeriodController : Controller
    {
        private CQContext db = new CQContext();

        // GET: CQPeriod
        public ActionResult Index()
        {
            return View(db.CQPeriods.ToList());
        }

        // GET: CQPeriod/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CQPeriod cQPeriod = db.CQPeriods.Find(id);
            if (cQPeriod == null)
            {
                return HttpNotFound();
            }
            return View(cQPeriod);
        }

        // GET: CQPeriod/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CQPeriod/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CQPID,CQPeriodName")] CQPeriod cQPeriod)
        {
            if (ModelState.IsValid)
            {
                db.CQPeriods.Add(cQPeriod);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cQPeriod);
        }

        // GET: CQPeriod/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CQPeriod cQPeriod = db.CQPeriods.Find(id);
            if (cQPeriod == null)
            {
                return HttpNotFound();
            }
            return View(cQPeriod);
        }

        // POST: CQPeriod/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CQPID,CQPeriodName")] CQPeriod cQPeriod)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cQPeriod).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cQPeriod);
        }

        // GET: CQPeriod/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CQPeriod cQPeriod = db.CQPeriods.Find(id);
            if (cQPeriod == null)
            {
                return HttpNotFound();
            }
            return View(cQPeriod);
        }

        // POST: CQPeriod/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CQPeriod cQPeriod = db.CQPeriods.Find(id);
            db.CQPeriods.Remove(cQPeriod);
            db.SaveChanges();
            return RedirectToAction("Index");
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
