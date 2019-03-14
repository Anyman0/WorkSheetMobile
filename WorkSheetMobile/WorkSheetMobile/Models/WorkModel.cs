using System;
using System.Collections.Generic;
using System.Text;

namespace WorkSheetMobile.Models
{
   public class WorkModel
   {
        // Workassignment fields
        public string Operation { get; set; }       
        public string WorkTitle { get; set; } 
        public string Description { get; set; }
        public DateTime Deadline { get; set; }

        // Employee fields
        public string EmpOperation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PhoneNumber { get; set; }
        public string Email { get; set; }

        // Customer fields
        public string CustOperation { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string ContactPerson { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerEmail { get; set; }
    }
}
