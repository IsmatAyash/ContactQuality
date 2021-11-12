using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContactQuality.Models;

namespace ContactQuality.ViewModels
{
    public class DashboardViewModel
    {
        // contains the group name and corresponding sum of all grades
        public Dictionary<string, decimal> GroupGrades { get; set; }

        // contains the group initials as key with period date text and corresponding sum of all grades in the tuple
        public Dictionary<string, decimal> GroupGradesByPeriod { get; set; }

        // contains the period date text and corresponding sum of all grades consolidated
        public Dictionary<string, decimal> GroupsGradesByPeriod { get; set; }
        public IEnumerable<Team> Teams { get; set; }
        public IEnumerable<Employee> TeamAgents { get; set; }
    }

    public class GroupGrades
    {
        public GroupGrades(string p, decimal g)
        {
            period = p;
            grade = g;
        }
        public string period { get; set; }
        public decimal grade { get; set; }
    }
}