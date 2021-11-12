using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ContactQuality.ViewModels
{
    public class AssignedChannelData
    {
        [Key]
        public int CQCID { get; set; }
        public string ChannelName { get; set; }
        public decimal InWgt { get; set; }
        public decimal OutWgt { get; set; }
        public bool Assigned { get; set; }
    }
}