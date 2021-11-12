using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContactQuality.Models;
using ContactQuality.ViewModels;
using System.Net;
using System.Data;
using System.Data.Entity;
using ContactQuality.Controllers;
using System.Linq.Expressions;
using System.Web.Routing;

namespace ContactQuality.Controllers
{
    public class CQKPIGradeController : Controller
    {
        // GET: CQKPIGrade
        private CQContext db = new CQContext();
        private CQHDRController cqhdr = new CQHDRController();

        // GET: CQHDR
        public ActionResult Index(int? detId, int? chid)
        {
            int hid = Convert.ToInt32(Session["CQHDRID"]);
            ViewBag.ContactTitle = db.CQDETs.FirstOrDefault(d => d.CQHDRID == hid).CQTitle;
            detId = detId == null ? (int)TempData["detId"] : detId.Value;
            chid = chid == null ? (int)TempData["chid"] : chid.Value;

            List<QualitySheet> qualitysheetVM = new List<QualitySheet>();
            qualitysheetVM = (from g in db.CQKPIGrades
                              join kc in db.CQKPIChannels on g.CQKPIID equals kc.CQKPIID
                              join kp in db.CQKPIs on g.CQKPIID equals kp.CQKPIID
                              join kg in db.CQGKPIs on kp.CQGKPIID equals kg.CQGKPIID
                              join gc in db.CQGKPIChannels on kg.CQGKPIID equals gc.CQGKPIID
                              where g.CQDETID == detId.Value && kc.CQCID == chid.Value && gc.CQCID == chid.Value
                              select new QualitySheet()
                              {
                                  KPIGradeID = g.CQKPIGID,
                                  KPIDetID = g.CQDETID,
                                  KPIHDRID = hid,
                                  KPIname = kp.CQKPIName,
                                  KpiRadioBtnID = kp.RadioBtnID,
                                  GradeType = kp.CQKPIType,
                                  selectedGrade = g.CQGrade,
                                  EvaluatorRemark = g.EvaluatorRemark,
                                  MaxGrade = kp.CQKPIMaxGrade,
                                  KpiInWgt = kc.InWgt,
                                  KpiOutWgt = kc.OutWgt,
                                  GrpInitials = kg.CQGKPIInitials
                              }).ToList();

            if (qualitysheetVM.Count != 0)
            {
                Dictionary<string, Tuple<string, string>> groupGrades = new Dictionary<string, Tuple<string, string>>();
                decimal val = 0;
                CQDET cqdet = db.CQDETs.Find(detId.Value);
                foreach (var grp in db.CQGKPIs)
                {
                    val = Convert.ToDecimal(cqdet.GetType().GetProperty(grp.CQGKPIInitials + "Grade").GetValue(cqdet, null));
                    groupGrades.Add(grp.CQGKPIInitials, Tuple.Create(grp.CQGKPIName, val.ToString("0%")));
                }

                ViewData["GroupGrades"] = groupGrades;
                
                // Calculate total grade : public function to calculate total grade body in CQHDRController
                string[] grades = null;
                grades = cqdet.GetType().GetProperties().Where(p => p.CanRead && p.Name.Contains("Grade") && p.Name.Length == 7).Select(p =>
                {
                    string gg = p.Name;
                    object value = p.GetValue(cqdet, null);
                    return value == null ? null : gg + " " + value.ToString();
                }).ToArray();
                decimal contactfinalgrade = grades[0] != null ? cqhdr.CalculateTotalGrade(grades, Session["JobTitle"].ToString(), cqdet.CQCID) : 0;
                ViewBag.ContactFinalGrade = contactfinalgrade;
                string gradecomment = GetGradeComment(contactfinalgrade);
                ViewBag.GradeComment = gradecomment;
            }
            return View(qualitysheetVM);
        }

        private string GetGradeComment(decimal fg)
        {
            string cmt = "";
            int finalgrade = Convert.ToInt32(fg * 100);
            if (finalgrade >= 90)
                cmt = "Exceed Expectations";
            else if (finalgrade >= 80 && finalgrade <= 89)
                cmt = "Meets Expectations";
            else if (finalgrade >= 70 && finalgrade <= 79)
                cmt = "Needs Improvement";
            else
                cmt = "Below Expectations";
            return cmt;
        }

        [HttpPost]
        public ActionResult Index(List<QualitySheet> qualitysheetVM)
        {
            int detid = qualitysheetVM.FirstOrDefault().KPIDetID;
            int channelid = db.CQDETs.Single(d => d.CQDETID == detid).CQCID;
            try
            {
                if (ModelState.IsValid)
                {
                    TempData["detId"] = detid;
                    TempData["chid"] = channelid;
                    foreach (var grade in qualitysheetVM)
                    {
                        CQKPIGrade existing_grade = new CQKPIGrade();
                        existing_grade = db.CQKPIGrades.Find(grade.KPIGradeID);
                        existing_grade.CQGrade = grade.selectedGrade ?? 0;
                        existing_grade.EvaluatorRemark = grade.EvaluatorRemark;
                        db.Entry(existing_grade).State = EntityState.Modified;
                    }
                    Dictionary<string, decimal> groupGrades = CalculateGrades(qualitysheetVM, Session["JobTitle"].ToString());
                    decimal val = 0;
                    CQDET grades = db.CQDETs.Find(detid);
                    foreach (var gg in db.CQGKPIs)
                    {
                        val = groupGrades.ContainsKey(gg.CQGKPIInitials) ? groupGrades[gg.CQGKPIInitials] : 0;
                        grades.GetType().GetProperty(gg.CQGKPIInitials + "Grade").SetValue(grades, val, null);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index", new { detId = detid, chid = channelid });
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return View(qualitysheetVM);
        }

        private Dictionary<string, decimal> CalculateGrades(List<QualitySheet> qualitygrades, string jobtitle)
        {
            int kpidenom = 1;
            decimal kpinum = 0, kpiadder = 0;
            Dictionary<string, decimal> grpgrade = new Dictionary<string, decimal>();

            var groups = (from qg in qualitygrades
                          group qg by  qg.GrpInitials into grp
                          select new { Initial = grp.Key, Items = grp.ToList()}).ToList();

            int tempden = 1;
            decimal GroupTotalGrade = 0m;

            foreach (var grp in groups)
            {
                GroupTotalGrade = 0m;
                tempden = grp.Items.Count(x => x.selectedGrade != 0 && x.selectedGrade != null);
                kpidenom = tempden == 0 ? 1 : tempden;
                kpinum = grp.Items.Where(k => k.selectedGrade == 0 || k.selectedGrade == null).Sum(w => jobtitle == "Inbound" ? w.KpiInWgt : w.KpiOutWgt) ?? 0;
                kpiadder = kpinum == 1 ? 0 : kpinum / kpidenom;
                //GroupTotalGrade = grp.Items.Sum(k => (k.selectedGrade * ((jobtitle == "Inbound" ? k.KpiInWgt : k.KpiOutWgt) + kpiadder)) / k.MaxGrade) ?? 0;
                //GroupTotalGrade = grp.Items.Sum(k => ((k.GradeType == "Foundation" && k.selectedGrade == 1 ? 0.01m : (decimal)k.selectedGrade) * ((jobtitle == "Inbound" ? k.KpiInWgt : k.KpiOutWgt) + kpiadder)) / k.MaxGrade) ?? 0m;
                foreach (var item in grp.Items)
                {
                    if (item.GradeType == "Foundation" && item.selectedGrade == 1)
                        GroupTotalGrade += 0.01m;
                    else
                    {
                        GroupTotalGrade += item.selectedGrade * ((jobtitle == "Inbound" ? item.KpiInWgt : item.KpiOutWgt) + kpiadder) / item.MaxGrade ?? 0;
                    }
                }
                grpgrade.Add(grp.Initial, GroupTotalGrade);
            }
            return grpgrade;
        }
    }
}