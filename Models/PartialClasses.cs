using System;
using System.ComponentModel.DataAnnotations;

namespace ContactQuality.Models
{
    [MetadataType(typeof(CQKPIMetadata))]
    public partial class CQKPI
    {
    }

    [MetadataType(typeof(CQGKPIMetadata))]
    public partial class CQGKPI
    {
    }

    [MetadataType(typeof(CQDETMetadata))]
    public partial class CQDET
    {
        partial void OnCreated()
        {
            CQEvalDate = DateTime.Today;
        }
        [DisplayFormat(DataFormatString = "{0:0%}")]
        public decimal Totalgrade { get; set; }
    }

    [MetadataType(typeof(CQChannelMetadata))]
    public partial class CQChannel
    {
    }

    [MetadataType(typeof(CQKPIChannelMetadata))]
    public partial class CQKPIChannel
    {
        public bool Assigned { get; set; }
        public string ChannelName { get; set; }
    }

    [MetadataType(typeof(CQGKPIChannelMetadata))]
    public partial class CQGKPIChannel
    {
        public bool Assigned { get; set; }
        public string ChannelName { get; set; }
    }

    [MetadataType(typeof(CQHDRMetadata))]
    public partial class CQHDR
    {
        partial void OnCreated()
        {
            CQYear = DateTime.Today.Year;
        }
    }

    [MetadataType(typeof(CQPeriodMetadata))]
    public partial class CQPeriod
    {
    }

    [MetadataType(typeof(EmployeeMetadata))]
    public partial class Employee
    {
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

    }

    [MetadataType(typeof(CQTNeedMetadata))]
    public partial class CQTNeed
    {
    }

}