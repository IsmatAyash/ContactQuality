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
    
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            this.CQHDRs = new HashSet<CQHDR>();
            this.Teams = new HashSet<Team>();
            this.CQTNeeds = new HashSet<CQTNeed>();
          OnCreated();
        }
    
    	partial void OnCreated();
    
        public string AgentLogin { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string CRMUser { get; set; }
        public string WindowsUser { get; set; }
        public int TitleID { get; set; }
        public int TeamID { get; set; }
        public string JobTitle { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CQHDR> CQHDRs { get; set; }
        public virtual Team Team { get; set; }
        public virtual Title Title { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Team> Teams { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CQTNeed> CQTNeeds { get; set; }
    }
}
