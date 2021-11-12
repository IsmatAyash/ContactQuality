using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContactQuality.Models;
using System.Data.Entity.Infrastructure;

namespace ContactQuality.Controllers
{
    public class CQGKPIController : Controller
    {
        private CQContext db = new CQContext();

        // GET: CQGKPI
        public ActionResult Index()
        {
            var cQGKPIs = db.CQGKPIs.Include(kg => kg.CQGKPIChannels);
            return View(cQGKPIs.ToList());
        }

        // GET: CQGKPI/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CQGKPI cQGKPI = db.CQGKPIs.Find(id);
            if (cQGKPI == null)
            {
                return HttpNotFound();
            }
            return View(cQGKPI);
        }

        public ActionResult Create()
        {
            var mm = new CQGKPI();
            PopulateAssignedChannelData(mm);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CQGKPIID,CQGKPIName,CQGKPIInitials")] CQGKPI cQGKPI, FormCollection frm)
        {
            if (ModelState.IsValid)
            {
                cQGKPI = UpdateKpiChannels(frm, cQGKPI);
                db.CQGKPIs.Add(cQGKPI);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cQGKPI);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CQGKPI cQGKPI = db.CQGKPIs.Include(c => c.CQGKPIChannels).Where(k => k.CQGKPIID == id).SingleOrDefault();
            PopulateAssignedChannelData(cQGKPI);
            if (cQGKPI == null)
            {
                return HttpNotFound();
            }
            return View(cQGKPI);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, FormCollection frm)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var kpiToUpdate = db.CQGKPIs.Include(c => c.CQGKPIChannels).Where(k => k.CQGKPIID == id).SingleOrDefault();
            if (TryUpdateModel(kpiToUpdate, "",
                new string[] { "CQGKPIID", "CQGKPIName", "CQGKPIInitials" }))
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
            return View(kpiToUpdate);
        }

        private void PopulateAssignedChannelData(CQGKPI cqgkpi)
        {
            var allChannels = db.CQChannels;
            var kpiChannels = new HashSet<int?>(cqgkpi.CQGKPIChannels.Select(c => c.CQCID));

            List<CQGKPIChannel> channels = new List<CQGKPIChannel>();
            foreach (var item in db.CQChannels)
            {
                CQGKPIChannel channel = new CQGKPIChannel();
                channel.CQCID = item.CQCID;
                channel.ChannelName = item.CQChannelName;
                channel.Assigned = kpiChannels.Contains(channel.CQCID);
                channel.InWgt = kpiChannels.Contains(channel.CQCID) ? cqgkpi.CQGKPIChannels.Single(c => c.CQCID == item.CQCID).InWgt : 0;
                channel.OutWgt = kpiChannels.Contains(channel.CQCID) ? cqgkpi.CQGKPIChannels.Single(c => c.CQCID == item.CQCID).OutWgt : 0;
                channels.Add(channel);
            }
            ViewBag.CQGKPIChannels = channels;
        }

        private CQGKPI UpdateKpiChannels(FormCollection frm, CQGKPI kpiToUpdate)
        {
            List<bool> assignedChannels = Request["channel.Assigned"].Split(',').Select(n => string.Compare(n, "true", true) == 0 ? true : false).ToList();
            for (int i = 0; i < assignedChannels.Count(); ++i)
            {
                if (assignedChannels[i]) assignedChannels.RemoveAt(i + 1);
            }

            var inwgtArray = frm.GetValues("channel.InWgt");
            var outwgtArray = frm.GetValues("channel.OutWgt");
            var selectedChannelsHS = new HashSet<string>(frm.GetValues("channel.CQCID"));
            var kpiChannels = new HashSet<int?>(kpiToUpdate.CQGKPIChannels.Select(k => k.CQCID));
            int channelid;
            for (int i = 0; i < assignedChannels.Count; i++)
            {
                channelid = Convert.ToInt32(selectedChannelsHS.ElementAt(i));
                if (assignedChannels[i])
                {
                    if (!kpiChannels.Contains(channelid))
                    {
                        kpiToUpdate.CQGKPIChannels.Add(new CQGKPIChannel()
                        {
                            CQCID = channelid,
                            CQGKPIID = kpiToUpdate.CQGKPIID,
                            InWgt = Convert.ToDecimal(inwgtArray[i]),
                            OutWgt = Convert.ToDecimal(outwgtArray[i])
                        });
                    }
                    else
                    {
                        kpiToUpdate.CQGKPIChannels.Single(c => c.CQCID == channelid).InWgt = Convert.ToDecimal(inwgtArray[i]);
                        kpiToUpdate.CQGKPIChannels.Single(c => c.CQCID == channelid).OutWgt = Convert.ToDecimal(outwgtArray[i]);
                    }
                }
                else
                {
                    if (kpiChannels.Contains(channelid))
                        kpiToUpdate.CQGKPIChannels.Remove(kpiToUpdate.CQGKPIChannels.Single(c => c.CQCID == channelid));
                }
            }
            return kpiToUpdate;
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CQGKPI cQGKPI = db.CQGKPIs.Find(id);
            if (cQGKPI == null)
            {
                return HttpNotFound();
            }
            return View(cQGKPI);
        }

        // POST: CQGKPI/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CQGKPI cQGKPI = db.CQGKPIs.Find(id);
            db.CQGKPIs.Remove(cQGKPI);
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
