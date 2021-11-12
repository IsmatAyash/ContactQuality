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
using System.IO;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Reflection;
using System.Data.Entity.Infrastructure;

namespace ContactQuality.Controllers
{
    public class CQHDRController : Controller
    {
        private CQContext db = new CQContext();

        // GET: CQHDR
        public ActionResult Index(int? hdrId, int? detId, string agentLogin, int? cqyear)
        {
            var ccagents = db.Employees.Where(emp => emp.TitleID < 3).Select(s => new
                    {
                        FullName = s.FirstName + ", " + s.LastName,
                        AgentLogin = s.AgentLogin,
                        JobTitle = s.JobTitle
                    }).ToList();

            ViewBag.CCAgents = new SelectList(ccagents, "AgentLogin", "FullName");
            //ViewBag.AgentLogin = new SelectList(db.Employees.Where(emp => emp.TitleID < 3), "AgentLogin", "FirstName");
            var viewModel = new AgentQualityData();

            if (!string.IsNullOrEmpty(agentLogin))
            {
                viewModel.Agents = db.CQHDRs
                    .Include(hdr => hdr.CQPeriod)
                    .Include(hdr => hdr.Employee)
                    .Where(emp => emp.AgentLogin == agentLogin);
                viewModel.Agents = cqyear != null ? viewModel.Agents.Where(s => s.CQYear == cqyear) : viewModel.Agents.Where(s => s.CQYear == DateTime.Now.Year);
                Session["AgtLogin"] = agentLogin;
                Session["AgtName"] = ccagents.Where(cc => cc.AgentLogin == agentLogin).FirstOrDefault().FullName;
                Session["JobTitle"] = ccagents.Where(cc => cc.AgentLogin == agentLogin).FirstOrDefault().JobTitle;
                Session["CqYear"] = cqyear;
            }

            if (hdrId != null)
            {
                Session["CQHDRID"] = hdrId.Value;
                ViewBag.PeriodName = viewModel.Agents.Single(hdr => hdr.CQHDRID == hdrId.Value).CQPeriod.CQPeriodName;
                viewModel.Contacts = viewModel.Agents.Single(hdr => hdr.CQHDRID == hdrId.Value).CQDETs;

                //Dictionary<string, string> grades = new Dictionary<string, string>();
                string[] grades = null;
                foreach (var item in viewModel.Contacts)
                {
                    if (item.CQKPIGrades.Count != 0)
                    {
                        grades = item.GetType().GetProperties().Where(p => p.CanRead && p.Name.Contains("Grade") && p.Name.Length == 7).Select(p =>
                        {
                            string gg = p.Name;
                            object value = p.GetValue(item, null);
                            return value == null ? null : gg + " " + value.ToString();
                        }).ToArray();

                        item.Totalgrade = grades[0] != null ? CalculateTotalGrade(grades, Session["JobTitle"].ToString(), item.CQCID) : 0;
                    }
                }
                decimal finalgrade = viewModel.Contacts.Count(h => h.CQHDRID == hdrId) != 0 ? viewModel.Contacts.Average(c => c.Totalgrade) * 100 : 0;
                if (finalgrade >= 90)
                    ViewBag.Expectation = "Exceed Expectations";
                else if (finalgrade >= 80 && finalgrade <= 89)
                    ViewBag.Expectation = "Meets Expectations";
                else if (finalgrade >= 70 && finalgrade <= 79)
                    ViewBag.Expectation = "Needs Improvement";
                else
                    ViewBag.Expectation = "Below Expectations";

                finalgrade /= 100;
                ViewBag.FinalGrade = finalgrade.ToString("0%");
            }

            if (detId != null && detId != 0)
            {
                ViewBag.CQDETID = detId.Value;
                viewModel.Grades = viewModel.Contacts.Single(det => det.CQDETID == detId.Value).CQKPIGrades;
            }
            return View(viewModel);
        }

        public decimal CalculateTotalGrade(string[] grades, string jobtitle, int chid)
        {
            int tempGroupDenom = 0, Grpid = 0;
            decimal ggrade = 0, wgt = 0;
            decimal Groupnum = 0;

            // distribute equall portions of the weight of a group if ZERO among other weights
            CQGKPIChannel cqgkpich = new CQGKPIChannel();
            Dictionary<string, Tuple<decimal, decimal>> GradeValues = new Dictionary<string, Tuple<decimal, decimal>>();
            foreach(var grade in grades)
            {
                Grpid = db.CQGKPIs.Where(g => g.CQGKPIInitials == grade.Substring(0, 2)).Single().CQGKPIID;
                ggrade = Convert.ToDecimal(grade.Substring(grade.IndexOf(" ")+1));
                cqgkpich = db.CQGKPIChannels.Where(c => c.CQGKPIID == Grpid && c.CQCID == chid).Single();
                wgt = (jobtitle == "Inbound" ? cqgkpich.InWgt : cqgkpich.OutWgt) ?? 0;
                Groupnum += ggrade == 0 ? wgt : 0;
                tempGroupDenom += ggrade != 0 ? 1 : 0;
                GradeValues.Add(grade.Substring(0, 2), Tuple.Create(ggrade, wgt));
            }
            int GroupDenom = tempGroupDenom == 0 ? 1 : tempGroupDenom;
            decimal GroupAdder = Groupnum / GroupDenom;

            return GradeValues.Sum(i => i.Value.Item1 * (i.Value.Item2 + GroupAdder));
        }

        public ActionResult Create(string agentlogin)
        {
            string qtr = "Quarter " + ((DateTime.Today.Month - 1) / 3 + 1).ToString();
            var mm = new CQHDR();
            mm.CQPID = db.CQPeriods.Single(p => p.CQPeriodName.Contains(qtr)).CQPID;
            ViewBag.CQPID = new SelectList(db.CQPeriods, "CQPID", "CQPeriodName", mm.CQPID);
            return View(mm);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateGrades([Bind(Include = "AgentLogin,CQYear,CQPID")] CQHDR cQHDR, string agentlogin)
        {
            var checkperiod = db.CQHDRs.Where(p => p.AgentLogin == cQHDR.AgentLogin && p.CQYear == cQHDR.CQYear && p.CQPID == cQHDR.CQPID).SingleOrDefault();
            if (checkperiod != null)
            {
                ModelState.AddModelError("", "You cannot create the same period for the same agent at the same year TWICE, please review");
                ViewBag.CQPID = new SelectList(db.CQPeriods, "CQPID", "CQPeriodName", cQHDR.CQPID);
                return View(cQHDR);
            }
            if (ModelState.IsValid)
            {
                db.CQHDRs.Add(cQHDR);
                db.SaveChanges();
                return RedirectToAction("Index", new { hdrId = Session["CQHDRID"], agentLogin = Session["AgtLogin"]});
            }

            ViewBag.CQPID = new SelectList(db.CQPeriods, "CQPID", "CQPeriodName", cQHDR.CQPID);
            return View(cQHDR);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CQHDR cqhdr = db.CQHDRs.Find(id);
            if (cqhdr == null)
            {
                return HttpNotFound();
            }
            ViewBag.CQPID = new SelectList(db.CQPeriods, "CQPID", "CQPeriodName", cqhdr.CQPID);
            return View(cqhdr);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cqhdrToUpdate = db.CQHDRs.Find(id);

            if (TryUpdateModel(cqhdrToUpdate, "",
               new string[] { "AgentLogin", "CQYear", "CQPID" }))
            {
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            ViewBag.CQPID = new SelectList(db.CQPeriods, "CQPID", "CQPeriodName", cqhdrToUpdate.CQPID);
            return View(cqhdrToUpdate);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CQHDR cQHDR = db.CQHDRs.Find(id);
            if (cQHDR == null)
            {
                return HttpNotFound();
            }
            return View(cQHDR);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var query = db.CQDETs.Where(d => d.CQHDRID == id);
            foreach(var row in query)
            {
                db.CQKPIGrades.RemoveRange(db.CQKPIGrades.Where(g => g.CQDETID == row.CQDETID));
            }
            db.SaveChanges();
            db.CQDETs.RemoveRange(db.CQDETs.Where(d => d.CQHDRID == id));
            db.SaveChanges();

            CQHDR cQHDR = db.CQHDRs.Find(id);
            db.CQHDRs.Remove(cQHDR);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult PlayRecording(string clip)
        {
            ViewBag.Clip = clip;
            return View();
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
