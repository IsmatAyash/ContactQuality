using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContactQuality.Models;
using System.ComponentModel.DataAnnotations;

namespace ContactQuality.ViewModels
{
    public class HdrTneedsViewModel
    {
        [Key]
        public CQHDR Agent { get; set; }
        public IList<CQTNeed> TrainingNeeds { get; set; }
    }
}