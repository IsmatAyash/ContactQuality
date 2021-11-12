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
    public class CQChannelController : Controller
    {
        private CQContext db = new CQContext();

        // GET: CQChannel
        public ActionResult Index()
        {
            return View(db.CQChannels.ToList());
        }

        // GET: CQChannel/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CQChannel cQChannel = db.CQChannels.Find(id);
            if (cQChannel == null)
            {
                return HttpNotFound();
            }
            return View(cQChannel);
        }

        // GET: CQChannel/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CQChannel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CQCID,CQChannelName")] CQChannel cQChannel)
        {
            if (ModelState.IsValid)
            {
                db.CQChannels.Add(cQChannel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cQChannel);
        }

        // GET: CQChannel/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CQChannel cQChannel = db.CQChannels.Find(id);
            if (cQChannel == null)
            {
                return HttpNotFound();
            }
            return View(cQChannel);
        }

        // POST: CQChannel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CQCID,CQChannelName")] CQChannel cQChannel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cQChannel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cQChannel);
        }

        // GET: CQChannel/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CQChannel cQChannel = db.CQChannels.Find(id);
            if (cQChannel == null)
            {
                return HttpNotFound();
            }
            return View(cQChannel);
        }

        // POST: CQChannel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CQChannel cQChannel = db.CQChannels.Find(id);
            db.CQChannels.Remove(cQChannel);
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
