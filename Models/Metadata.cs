using System;
using System.ComponentModel.DataAnnotations;

namespace ContactQuality.Models
{
    public class CQKPIMetadata
    {
        [Display(Name = "KPI Group")]
        public int CQGKPIID { get; set; }
        [Display(Name ="KPI")]
        public int CQKPIID { get; set; }

        [Display(Name ="Description")]
        public string CQKPIName { get; set; }
        [Display(Name ="Type")]
        public string CQKPIType { get; set; }
        [Display(Name ="Max Grade")]
        public int CQKPIMaxGrade { get; set; }
    }

    public class CQGKPIMetadata
    {
        [Display(Name ="Group")]
        public int CQGKPIID { get; set; }
        [Display(Name ="Group Name")]
        public string CQGKPIName { get; set; }
        [Display(Name ="Initials")]
        public string CQGKPIInitials { get; set; }
    }

    public class CQDETMetadata
    {
        [Display(Name = "Channel")]
        public string CQCID { get; set; }
        [Display(Name = "Evaluation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CQEvalDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:0%}")]
        public decimal GCGrade { get; set; }
        [DisplayFormat(DataFormatString = "{0:0%}")]
        public decimal CEGrade { get; set; }
        [DisplayFormat(DataFormatString = "{0:0%}")]
        public decimal CCGrade { get; set; }
        [DisplayFormat(DataFormatString = "{0:0%}")]
        public decimal CHGrade { get; set; }
        [DisplayFormat(DataFormatString = "{0:0%}")]
        public decimal PKGrade { get; set; }
        [DisplayFormat(DataFormatString = "{0:0%}")]
        public decimal SFGrade { get; set; }
        [DisplayFormat(DataFormatString = "{0:0%}")]
        public decimal ERGrade { get; set; }

        [DisplayFormat(DataFormatString = "{0:0%}")]
        public decimal SSGrade { get; set; }
        [DisplayFormat(DataFormatString = "{0:0%}")]

        [Display(Name = "Evaluator")]
        public string CQEvaluator { get; set; }

        [Display(Name = "Contact Title")]
        public string CQTitle { get; set; }

        [Display(Name = "Contact Recording")]
        public string CQRecording { get; set; }
    }

    public class CQChannelMetadata
    {
        [Display(Name ="Channel")]
        public int CQCID { get; set; }

        [Display(Name ="Channel Description")]
        public string CQChannelName { get; set; }
    }

    public class CQKPIChannelMetadata
    {
        [Display(Name = "Channel")]
        public int CQCID { get; set; }
        [Display(Name = "KPI")]
        public int CQKPIID { get; set; }
        [Display(Name = "In Weight")]
        public decimal InWgt { get; set; }
        [Display(Name = "Out Weight")]
        public decimal OutWgt { get; set; }
    }

    public class CQGKPIChannelMetadata
    {
        [Display(Name = "Channel")]
        public int CQCID { get; set; }
        [Display(Name = "KPI Group")]
        public int CQGKPIID { get; set; }
        [Display(Name = "In Weight")]
        public decimal InWgt { get; set; }
        [Display(Name = "Out Weight")]
        public decimal OutWgt { get; set; }
    }

    public class CQHDRMetadata
    {
        [Display(Name = "Evaluation ID")]
        public int CQHDRID { get; set; }
        [Display(Name = "Agent")]
        public string AgentLogin { get; set; }
        [Display(Name = "Year")]
        public Nullable<int> CQYear { get; set; }
        [Display(Name = "Period")]
        public int CQPID { get; set; }
    }

    public class CQPeriodMetadata
    {
        [Display(Name = "Period")]
        public int CQPID { get; set; }

        [Display(Name = "Period Name")]
        public string CQPeriodName { get; set; }
    }

    public class EmployeeMetadata
    {
        [Display(Name = "CRM User")]
        public string CRMUser { get; set; }
        [Display(Name = "WIndows User")]
        public string WindowsUser { get; set; }
        [Display(Name ="Title")]
        public int TitleID { get; set; }
        [Display(Name = "Team")]
        public int TeamID { get; set; }
    }

    public class CQTNeedMetadata
    {
        [Display(Name = "Competency Name")]
        public int CompID { get; set; }
        [Display(Name = "Competency Level")]
        public string TNLevel { get; set; }
        [Display(Name = "Status")]
        public string TNStatus { get; set; }
    }

}