using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WorkSheetBackend.DataAccess;

using WorkSheetMobile.Models;

namespace WorkSheetBackend.Controllers
{
    public class TimesheetController : ApiController
    {

        
        // Data for pickers
        public WorkModel GetAllPickerData(int id)
        {
            WorksheetEntities entities = new WorksheetEntities();
            id = 1;

            string[] workAssignments = null;
            string[] employees = null;
            string[] contractors = null;

            try
            {
                workAssignments = (from wa in entities.WorkAssignments where (wa.Active == true) select wa.Title).ToArray();
                employees = (from e in entities.Employees where (e.Active == true) select e.FirstName + " " + e.LastName).ToArray();
                contractors = (from co in entities.Contractors where (co.Active == true) select co.CompanyName).ToArray();

                WorkModel pickerData = new WorkModel()
                {
                    WorkPickerData = workAssignments,
                    EmployeePickerData = employees,
                    ContractorPickerData = contractors
                };

                return pickerData;
            }
            finally
            {
                entities.Dispose();
            }
           
        }
       
       
        // Method to get all completed work for timesheet-page
        [HttpGet]
        public List<string> TimeSheetList(int tsID)
        {
            WorksheetEntities entities = new WorksheetEntities();
            
            List<string> realList = new List<string>();           

            tsID = 2;

            string[] sheetId = (from ts in entities.Timesheets where (ts.WorkComplete == true) select ts.Id_Contractor.ToString() + " " + ts.Id_Employee.ToString() + " " + ts.Id_WorkAssignment.ToString()).ToArray();
                                                   
            try
            {
                
                
                foreach (var item in sheetId)
                {
                    string[] data = item.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string contractor = data[0];
                    string contr = (from c in entities.Contractors where (c.Id_Contractor.ToString() == contractor) select c.CompanyName).Single();
                    string employee = data[1];
                    string emp = (from e in entities.Employees where (e.Id_Employee.ToString() == employee) select e.FirstName + " " + e.LastName).Single();
                    string workid = data[2];
                    string work = (from w in entities.WorkAssignments where (w.Id_WorkAssignment.ToString() == workid) select w.Title).Single();
                    
                    realList.Add(contr + " | " + emp + " | " + work);
                }

            }            

            finally
            {
                entities.Dispose();
            }
           
            return realList;
        }

        // Method to get same data as in timesheet mainpage, but for the chosen employee only
        public List<string> GetChosenEmployee (string Employee)
        {
            WorksheetEntities entities = new WorksheetEntities();

            string emplo = (from e in entities.Employees where (e.FirstName + " " + e.LastName == Employee) select e.Id_Employee.ToString()).Single();

            List<string> employeeWork = new List<string>();

            string[] sheet = (from ts in entities.Timesheets where (ts.Id_Employee.ToString() == emplo) select ts.Id_Contractor.ToString() + " " + ts.Id_Employee.ToString() + " " + ts.Id_WorkAssignment.ToString()).ToArray();

            try
            {


                foreach (var item in sheet)
                {
                    string[] data = item.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string contractor = data[0];
                    string contr = (from c in entities.Contractors where (c.Id_Contractor.ToString() == contractor) select c.CompanyName).Single();
                    string employee = data[1];
                    string emp = (from e in entities.Employees where (e.Id_Employee.ToString() == employee) select e.FirstName + " " + e.LastName).Single();
                    string workid = data[2];
                    string work = (from w in entities.WorkAssignments where (w.Id_WorkAssignment.ToString() == workid) select w.Title).Single();

                    employeeWork.Add(contr + " | " + emp + " | " + work);
                }

            }

            finally
            {
                entities.Dispose();
            }

            return employeeWork;
        }

    }
}
