//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WorkSheetBackend.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class WorkAssignment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkAssignment()
        {
            this.Timesheets = new HashSet<Timesheet>();
        }
    
        public int Id_WorkAssignment { get; set; }
        public Nullable<int> Id_Customer { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> Deadline { get; set; }
        public Nullable<bool> InProgress { get; set; }
        public Nullable<System.DateTime> InProgressAt { get; set; }
        public Nullable<bool> Completed { get; set; }
        public Nullable<System.DateTime> CompletedAt { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<System.DateTime> LastModifiedAt { get; set; }
        public Nullable<System.DateTime> DeletedAt { get; set; }
        public Nullable<bool> Active { get; set; }
    
        public virtual Customer Customer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Timesheet> Timesheets { get; set; }
    }
}
