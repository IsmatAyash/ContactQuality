using System.Collections.Generic;
using ContactQuality.Models;

namespace ContactQuality.ViewModels
{
    public class AgentQualityData
    {
        public IEnumerable<CQHDR> Agents { get; set; }
        public IEnumerable<CQDET> Contacts { get; set; }
        public IEnumerable<CQKPIGrade> Grades { get; set; }
    }
}