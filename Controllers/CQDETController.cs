using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContactQuality.Models;
using System.IO;

namespace ContactQuality.Controllers
{
    public class CQDETController : Controller
    {
        private CQContext db = new CQContext();

        // GET: CQDET/Create
        public ActionResult Create(HttpPostedFileBase uploadFile)
        {
            var mm = new CQDET();
            
            mm.CQEvaluator = db.Employees.First(e => e.WindowsUser == User.Identity.Name.Substring(7)).FullName;
            //mm.CQEvaluator = db.Employees.First(e => e.TitleID == 6).FullName;
            ViewBag.CQCID = new SelectList(db.CQChannels, "CQCID", "CQChannelName",5);
            ViewBag.CQHDRID = new SelectList(db.CQHDRs, "CQHDRID", "AgentLogin");
            return View(mm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CQCID,CQEvalDate,CQEvaluator,CQTitle,CQRecording")] CQDET cQDET, HttpPostedFileBase uploadFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    cQDET.CQHDRID = (int)Session["CQHDRID"];
                    if (uploadFile != null)
                    {
                        cQDET.CQRecording = uploadFile.FileName;
                        string path = Path.Combine(Server.MapPath("~/Content/Recordings"), Path.GetFileName(uploadFile.FileName));
                        uploadFile.SaveAs(path);
                    }
                    db.CQDETs.Add(cQDET);
                    db.SaveChanges();

                    var kpichannel = from k in db.CQKPIs
                                     join kc in db.CQKPIChannels on k.CQKPIID equals kc.CQKPIID
                                     where kc.CQCID == cQDET.CQCID
                                     select k;

                    foreach (var kpi in kpichannel) 
                    {
                        CQKPIGrade cQKPIGrade = new CQKPIGrade();
                        cQKPIGrade.CQDETID = cQDET.CQDETID;
                        cQKPIGrade.CQKPIGID = kpi.CQGKPIID;
                        cQKPIGrade.CQKPIID = kpi.CQKPIID;
                        cQKPIGrade.CQGrade = 0;
                        cQKPIGrade.EvaluatorRemark = "";
                        db.CQKPIGrades.Add(cQKPIGrade);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index", "CQHDR", new { hdrId = cQDET.CQHDRID, detId = cQDET.CQDETID, agentLogin = @Session["AgtLogin"], cqyear = Session["CqYear"] });
                }
            }
            catch (DataException /* dex */ )
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            ViewBag.CQCID = new SelectList(db.CQChannels, "CQCID", "CQChannelName", cQDET.CQCID);
            ViewBag.CQHDRID = new SelectList(db.CQHDRs, "CQHDRID", "AgentLogin", cQDET.CQHDRID);
            return View(cQDET);
        }

        public ActionResult Edit(int? id, HttpPostedFileBase uploadFile)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CQDET cQDET = db.CQDETs.Find(id);
            if (cQDET == null)
            {
                return HttpNotFound();
            }
            ViewBag.CQCID = new SelectList(db.CQChannels, "CQCID", "CQChannelName", cQDET.CQCID);
            return View(cQDET);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id, HttpPostedFileBase uploadFile)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cqdetToUpdate = db.CQDETs.Find(id);
            if (TryUpdateModel(cqdetToUpdate, "", new string[] { "CQCID", "CQEvalDate", "CQEvaluator", "CQTitle", "CQRecording" }))
            {
                try
                {
                    if (uploadFile != null)
                    {
                        if (!string.IsNullOrEmpty(cqdetToUpdate.CQRecording))
                        {
                            string fullPath = Path.Combine(Server.MapPath("~/Content/Recordings"), cqdetToUpdate.CQRecording);
                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }
                        }
                        var path = Path.Combine(Server.MapPath("~/Content/Recordings"), Path.GetFileName(uploadFile.FileName));
                        cqdetToUpdate.CQRecording = uploadFile.FileName;
                        uploadFile.SaveAs(path);
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index", "CQHDR", new { hdrId = cqdetToUpdate.CQHDRID, detId = cqdetToUpdate.CQDETID, agentLogin = @Session["AgtLogin"], cqyear = Session["CqYear"] });
                }
                catch (DataException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            ViewBag.CQCID = new SelectList(db.CQChannels, "CQCID", "CQChannelName", cqdetToUpdate.CQCID);
            return View(cqdetToUpdate);
        }

        // GET: CQDET/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }

            CQDET cQDET = db.CQDETs.Find(id);
            if (cQDET == null)
            {
                return HttpNotFound();
            }
            return View(cQDET);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            try
            {
                //delete related grades
                db.CQKPIGrades.RemoveRange(db.CQKPIGrades.Where(g => g.CQDETID == id));
                db.SaveChanges();
                CQDET cqdet = db.CQDETs.Find(id);
                // delete the recording file from server
                if (!string.IsNullOrEmpty(cqdet.CQRecording))
                {
                    string fullPath = Path.Combine(Server.MapPath("~/Content/Recordings"), cqdet.CQRecording);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                db.CQDETs.Remove(cqdet);
                db.SaveChanges();
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            int hid = (int)Session["CQHDRID"];
            var query = db.CQDETs.Where(d => d.CQHDRID == hid).FirstOrDefault();
            int? NextDetID = query != null ? query.CQDETID : 0;
            return RedirectToAction("Index", "CQHDR", new { hdrId = Session["CQHDRID"], detId = NextDetID, agentLogin = Session["AgtLogin"], cqyear = Session["CqYear"] });
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
