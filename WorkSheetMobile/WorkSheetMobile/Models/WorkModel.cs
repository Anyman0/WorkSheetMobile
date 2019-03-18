using System;
using System.Collections;
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
        public int EmployeeId { get; set; }
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

        // Contractor fields
        public string ContOperation { get; set; }
        public int ContractorId { get; set; }
        public string ContractorName { get; set; }
        public string ContractorContactPerson { get; set; }
        public string ContractorPhoneNumber { get; set; }
        public string ContractorEmail { get; set; }
        public string VatId { get; set; }
        public string HourlyRate { get; set; }

        // Timesheet fields           
        public string[] WorkPickerData { get; set; } 
        public string[] EmployeePickerData { get; set; }
        public string[] ContractorPickerData { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public double CountedHours { get; set; }

        // Login fields
        public string UserName { get; set; }
        public byte[] Password { get; set; } 
        public string PasswordString { get; set; } 
    }
}
