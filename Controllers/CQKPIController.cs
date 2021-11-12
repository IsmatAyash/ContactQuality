using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContactQuality.Models;
using ContactQuality.ViewModels;
using System.Data.Entity.Infrastructure;

namespace ContactQuality.Controllers
{
    public class CQKPIController : Controller
    {
        private CQContext db = new CQContext();

        // GET: CQKPI
        public ActionResult Index()
        {
            var cQKPIs = db.CQKPIs.Include(kg => kg.CQGKPI).Include(ch => ch.CQKPIChannels);
            return View(cQKPIs.ToList());
        }

        // GET: CQKPI/Create
        public ActionResult Create()
        {
            var mm = new CQKPI();
            PopulateAssignedChannelData(mm);
            ViewBag.CQGKPIID = new SelectList(db.CQGKPIs, "CQGKPIID", "CQGKPIName");
            return View();
        }

        // POST: CQKPI/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CQKPIID,CQGKPIID,CQKPIName,CQKPIType,CQKPIMaxGrade")] CQKPI cQKPI, FormCollection frm)
        {
            if (ModelState.IsValid)
            {
                cQKPI.RadioBtnID = db.CQGKPIs.Single(g => g.CQGKPIID == cQKPI.CQGKPIID).CQGKPIInitials + (db.CQKPIs.Count(g => g.CQGKPIID == cQKPI.CQGKPIID) + 1).ToString().PadLeft(2,'0');
                cQKPI = UpdateKpiChannels(frm , cQKPI);
                db.CQKPIs.Add(cQKPI);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
      
            ViewBag.CQGKPIID = new SelectList(db.CQGKPIs, "CQGKPIID", "CQGKPIName", cQKPI.CQGKPIID);
            return View(cQKPI);
        }

        // GET: CQKPI/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //CQKPI cQKPI = db.CQKPIs.Find(id);
            CQKPI cQKPI = db.CQKPIs.Include(c => c.CQKPIChannels).Where(k => k.CQKPIID == id).SingleOrDefault();
            PopulateAssignedChannelData(cQKPI);
            if (cQKPI == null)
            {
                return HttpNotFound();
            }
            ViewBag.CQGKPIID = new SelectList(db.CQGKPIs, "CQGKPIID", "CQGKPIName", cQKPI.CQGKPIID);
            return View(cQKPI);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, FormCollection frm)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var kpiToUpdate = db.CQKPIs.Include(c => c.CQKPIChannels).Where(k => k.CQKPIID == id).SingleOrDefault();
            if (TryUpdateModel(kpiToUpdate, "",
                new string[] { "CQKPIID", "CQGKPIID", "CQKPIName", "CQKPIType", "CQKPIMaxGrade" }))
            {
                try
                {
                    UpdateKpiChannels(frm, kpiToUpdate);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateAssignedChannelData(kpiToUpdate);
            ViewBag.CQGKPIID = new SelectList(db.CQGKPIs, "CQGKPIID", "CQGKPIName", kpiToUpdate.CQGKPIID);
            return View(kpiToUpdate);
        }

        private void PopulateAssignedChannelData(CQKPI cqkpi)
        {
            var allChannels = db.CQChannels;
            var kpiChannels = new HashSet<int?>(cqkpi.CQKPIChannels.Select(c => c.CQCID));

            List<CQKPIChannel> channels = new List<CQKPIChannel>();
            foreach (var item in db.CQChannels)
            {
                CQKPIChannel channel = new CQKPIChannel();
                channel.CQCID = item.CQCID;
                channel.ChannelName = item.CQChannelName;
                channel.Assigned = kpiChannels.Contains(channel.CQCID);
                channel.InWgt = kpiChannels.Contains(channel.CQCID) ? cqkpi.CQKPIChannels.Single(c => c.CQCID == item.CQCID).InWgt : 0;
                channel.OutWgt = kpiChannels.Contains(channel.CQCID) ? cqkpi.CQKPIChannels.Single(c => c.CQCID == item.CQCID).OutWgt : 0;
                channels.Add(channel);
            }
            ViewBag.CQKPIChannels = channels;
        }

        private CQKPI UpdateKpiChannels(FormCollection frm, CQKPI kpiToUpdate)
        {
            List<bool> assignedChannels = Request["channel.Assigned"].Split(',').Select(n => string.Compare(n, "true", true) == 0 ? true : false).ToList();
            for (int i = 0; i < assignedChannels.Count(); ++i)
            {
                if (assignedChannels[i]) assignedChannels.RemoveAt(i + 1);
            }

            var inwgtArray = frm.GetValues("channel.InWgt");
            var outwgtArray = frm.GetValues("channel.OutWgt");
            var selectedChannelsHS = new HashSet<string>(frm.GetValues("channel.CQCID"));
            var kpiChannels = new HashSet<int?>(kpiToUpdate.CQKPIChannels.Select(k => k.CQCID));
            int channelid;
            for (int i = 0; i < assignedChannels.Count; i++)
            {
                channelid = Convert.ToInt32(selectedChannelsHS.ElementAt(i));
                if (assignedChannels[i])
                {
                    if(!kpiChannels.Contains(channelid))
                    {
                        kpiToUpdate.CQKPIChannels.Add(new CQKPIChannel()
                        {
                            CQCID = channelid,
                            CQKPIID = kpiToUpdate.CQKPIID,
                            InWgt = Convert.ToDecimal(inwgtArray[i]),
                            OutWgt = Convert.ToDecimal(outwgtArray[i])
                        });
                    }
                    else
                    {
                        kpiToUpdate.CQKPIChannels.Single(c => c.CQCID == channelid).InWgt = Convert.ToDecimal(inwgtArray[i]);
                        kpiToUpdate.CQKPIChannels.Single(c => c.CQCID == channelid).OutWgt = Convert.ToDecimal(outwgtArray[i]);
                    }
                }
                else
                {
                    if (kpiChannels.Contains(channelid))
                        kpiToUpdate.CQKPIChannels.Remove(kpiToUpdate.CQKPIChannels.Single(c => c.CQCID == channelid));
                }
            }
            return kpiToUpdate;
        }

        // GET: CQKPI/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CQKPI cQKPI = db.CQKPIs.Find(id);
            if (cQKPI == null)
            {
                return HttpNotFound();
            }
            return View(cQKPI);
        }

        // POST: CQKPI/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CQKPI cQKPI = db.CQKPIs.Find(id);
            db.CQKPIs.Remove(cQKPI);
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
