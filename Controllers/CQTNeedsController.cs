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
    public class CQTNeedsController : Controller
    {
        private CQContext db = new CQContext();

        // GET: CQTNeeds
        public ActionResult Index(string sortOrder, string searchString)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            var tneeds = db.CQTNeeds.Include(c => c.Competency).Include(c => c.Employee);
            if (!String.IsNullOrEmpty(searchString))
            {
                tneeds = tneeds.Where(s => s.Employee.LastName.Contains(searchString)
                                       || s.Employee.FirstName.Contains(searchString));
            }
            if (sortOrder == "name_desc")
            {
                tneeds = tneeds.OrderByDescending(s => s.Employee.LastName);
            }
            else
            {
                tneeds = tneeds.OrderBy(s => s.Employee.LastName);
            }
            return View(tneeds.ToList());
        }

        // GET: CQTNeeds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CQTNeed cQTNeed = db.CQTNeeds.Find(id);
            if (cQTNeed == null)
            {
                return HttpNotFound();
            }
            return View(cQTNeed);
        }

        // GET: CQTNeeds/Create
        public ActionResult Create()
        {
            ViewBag.CompID = new SelectList(db.Competencies, "CompID", "CompName");
            ViewBag.AgentLogin = new SelectList(db.Employees, "AgentLogin", "FullName");
            return View();
        }

        // POST: CQTNeeds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TNID,CompID,AgentLogin,TNLevel,TNStatus")] CQTNeed cQTNeed)
        {
            if (ModelState.IsValid)
            {
                db.CQTNeeds.Add(cQTNeed);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CompID = new SelectList(db.Competencies, "CompID", "CompName", cQTNeed.CompID);
            ViewBag.AgentLogin = new SelectList(db.Employees, "AgentLogin", "FullName", cQTNeed.AgentLogin);
            return View(cQTNeed);
        }

        // GET: CQTNeeds/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CQTNeed cQTNeed = db.CQTNeeds.Find(id);
            if (cQTNeed == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompID = new SelectList(db.Competencies, "CompID", "CompName", cQTNeed.CompID);
            ViewBag.AgentLogin = new SelectList(db.Employees, "AgentLogin", "LastName", cQTNeed.AgentLogin);
            return View(cQTNeed);
        }

        // POST: CQTNeeds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TNID,CompID,AgentLogin,TNLevel,TNStatus")] CQTNeed cQTNeed)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cQTNeed).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompID = new SelectList(db.Competencies, "CompID", "CompName", cQTNeed.CompID);
            ViewBag.AgentLogin = new SelectList(db.Employees, "AgentLogin", "LastName", cQTNeed.AgentLogin);
            return View(cQTNeed);
        }

        // GET: CQTNeeds/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CQTNeed cQTNeed = db.CQTNeeds.Find(id);
            if (cQTNeed == null)
            {
                return HttpNotFound();
            }
            return View(cQTNeed);
        }

        // POST: CQTNeeds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CQTNeed cQTNeed = db.CQTNeeds.Find(id);
            db.CQTNeeds.Remove(cQTNeed);
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
