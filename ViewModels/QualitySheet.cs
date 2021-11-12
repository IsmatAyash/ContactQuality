using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContactQuality.Models;
using System.ComponentModel.DataAnnotations;

namespace ContactQuality.ViewModels
{
    public class QualitySheet
    {
        [Key]
        public int KPIGradeID { get; set; }
        public int KPIDetID { get; set; }
        public int KPIHDRID { get; set; }
        public string KPIname { get; set; }
        public string GradeType { get; set; }
        public int? selectedGrade { get; set; }
        public string EvaluatorRemark { get; set; }
        public int? MaxGrade { get; set; }
        public decimal? KpiInWgt { get; set; }
        public decimal? KpiOutWgt { get; set; }
        public string KpiRadioBtnID { get; set; }
        //public decimal? GInWgt { get; set; }
        //public decimal? GOutWgt { get; set; }
        public string GrpInitials { get; set; }
        //public string GrpName { get; set; }
    }
}