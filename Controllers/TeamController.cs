using System;
using System.Linq;
using System.Web.Mvc;
using ContactQuality.Models;
using ContactQuality.ViewModels;

namespace ContactQuality.Controllers
{
    public class TeamController : Controller
    {
        private CQContext db = new CQContext();
        public ActionResult Teamboard(int? teamid)
        {
            ViewBag.TeamID = teamid;
            var teamboard = new DashboardViewModel();
            teamboard.GroupGrades = (from h in db.CQHDRs
                                     join d in db.CQDETs on h.CQHDRID equals d.CQHDRID
                                     join g in db.CQKPIGrades on d.CQDETID equals g.CQDETID
                                     join kk in db.CQKPIs on g.CQKPIID equals kk.CQKPIID
                                     join kg in db.CQGKPIs on kk.CQGKPIID equals kg.CQGKPIID
                                     join emp in db.Employees on h.Employee.AgentLogin equals emp.AgentLogin
                                     where h.CQYear == DateTime.Today.Year && (emp.TeamID == teamid)
                                     group d by new { GInit = kg.CQGKPIInitials, GName = kg.CQGKPIName } into grp
                                     select new
                                     {
                                         Group = grp.Key.GName,
                                         Grade = grp.Key.GInit == "GC" ? grp.Average(g => g.GCGrade.Value) : 
                                                 grp.Key.GInit == "CE" ? grp.Average(g => g.CEGrade.Value) :
                                                 grp.Key.GInit == "CC" ? grp.Average(g => g.CCGrade.Value) :
                                                 grp.Key.GInit == "CH" ? grp.Average(g => g.CHGrade.Value) :
                                                 grp.Key.GInit == "PK" ? grp.Average(g => g.PKGrade.Value) :
                                                 grp.Key.GInit == "SF" ? grp.Average(g => g.SFGrade.Value) :
                                                 grp.Key.GInit == "ER" ? grp.Average(g => g.ERGrade.Value) :
                                                 grp.Key.GInit == "SS" ? grp.Average(g => g.SSGrade.Value) : 0
                                    }).ToDictionary(t => t.Group, t => t.Grade);
            return View(teamboard);
        }

        public ActionResult GetTeamGradesByGroup(string gname, int teamid)
        {
            var data = (from h in db.CQHDRs
                        join p in db.CQPeriods on h.CQPID equals p.CQPID
                        join d in db.CQDETs on h.CQHDRID equals d.CQHDRID
                        join gg in db.CQKPIGrades on d.CQDETID equals gg.CQDETID
                        join k in db.CQKPIs on gg.CQKPIID equals k.CQKPIID
                        join emp in db.Employees on h.AgentLogin equals emp.AgentLogin
                        where (h.CQYear == DateTime.Today.Year) && (emp.TeamID == teamid)
                        group gg by p.CQPID into grp
                        let ggrade = gname == "Greeting Closing" ? grp.Average(g => g.CQDET.GCGrade.Value) :
                                     gname == "Contact Etiquette" ? grp.Average(g => g.CQDET.CEGrade.Value) :
                                     gname == "Customer Care" ? grp.Average(g => g.CQDET.CCGrade.Value) :
                                     gname == "Contact Handling" ? grp.Average(g => g.CQDET.CHGrade.Value) :
                                     gname == "Product Knowledge" ? grp.Average(g => g.CQDET.PKGrade.Value) :
                                     gname == "System Familiarity" ? grp.Average(g => g.CQDET.SFGrade.Value) :
                                     gname == "Effective Response" ? grp.Average(g => g.CQDET.ERGrade.Value) :
                                     gname == "Selling Skills" ? grp.Average(g => g.CQDET.SSGrade.Value) : 0
                        select new { period = DateTime.Today.Year.ToString() + " Q" + grp.Key.ToString(), grade = (int)(ggrade * 100) }).OrderBy(p => p.period).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTeamGradesByPeriod(int teamid)
        {
            var data = (from h in db.CQHDRs
                        join p in db.CQPeriods on h.CQPID equals p.CQPID
                        join d in db.CQDETs on h.CQHDRID equals d.CQHDRID
                        join gg in db.CQKPIGrades on d.CQDETID equals gg.CQDETID
                        join k in db.CQKPIs on gg.CQKPIID equals k.CQKPIID
                        join emp in db.Employees on h.AgentLogin equals emp.AgentLogin
                        where (h.CQYear == DateTime.Today.Year) && (emp.TeamID == teamid)
                        group gg by p.CQPID into grp
                        let ggrade = grp.Average(g => (g.CQDET.GCGrade.Value + g.CQDET.CEGrade.Value + g.CQDET.CHGrade.Value + g.CQDET.CCGrade.Value +
                                                       g.CQDET.ERGrade.Value + g.CQDET.PKGrade.Value + g.CQDET.SFGrade.Value + g.CQDET.SSGrade.Value) / 8) 
                        select new { period = DateTime.Today.Year.ToString() + " Q" + grp.Key.ToString(), grade = (int)(ggrade * 100) }).OrderBy(p => p.period).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Parameter()
        {
            return View();
        }
    }
}