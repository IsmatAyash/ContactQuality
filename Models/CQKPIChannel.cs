//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ContactQuality.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CQKPIChannel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CQKPIChannel()
        {
          OnCreated();
        }
    
    	partial void OnCreated();
    
        public int CQKPICID { get; set; }
        public Nullable<int> CQKPIID { get; set; }
        public Nullable<int> CQCID { get; set; }
        public Nullable<decimal> InWgt { get; set; }
        public Nullable<decimal> OutWgt { get; set; }
    
        public virtual CQChannel CQChannel { get; set; }
        public virtual CQKPI CQKPI { get; set; }
    }
}
