using System;
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

        // TODO: GET-methods for timesheet data. Model.Operation - controller

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

        // Exploring with IDs
        public string[] GetAllContractors(string chosenEntity)
        {
            WorkModel data = new WorkModel();
            WorksheetEntities entities = new WorksheetEntities();
            Contractor contr = (from c in entities.Contractors where (c.CompanyName == chosenEntity) select c).FirstOrDefault();           
           
            string[] allIds = (from e in entities.Timesheets where (e.Id_Contractor == contr.Id_Contractor) select e.Id_Customer + " " + e.Id_Employee + " " + e.Id_WorkAssignment).ToArray();
                                       

            return allIds;
        }

        // Get Model for chosen entity  TODO: FIX LISTING ISSUES!!
        /*public WorkModel GetModel (string chosenEntity)
        {
            WorkModel model = new WorkModel();
            WorksheetEntities entities = new WorksheetEntities();
            WorkAssignment assignment = (from wa in entities.WorkAssignments where (wa.Title == chosenEntity) select wa).FirstOrDefault();
            Employee employee = (from e in entities.Employees where (e.FirstName + " " + e.LastName == chosenEntity) select e).FirstOrDefault();
            Contractor contractor = (from co in entities.Contractors where (co.CompanyName == chosenEntity) select co).FirstOrDefault();
            string chosenId = chosenEntity;
            int chosenWorkid = 0;
            int chosenEmployeeId = 0;
            int chosenContractorId = 0;

            if (assignment != null)
            {
                assignment = (from wa in entities.WorkAssignments where (wa.Title == chosenEntity) select wa).FirstOrDefault();
                chosenWorkid = assignment.Id_WorkAssignment;
            }
            else if (employee != null)
            {
                employee = (from e in entities.Employees where (e.FirstName + " " + e.LastName == chosenEntity) select e).FirstOrDefault();
                chosenEmployeeId = employee.Id_Employee;
            }
            else if (contractor != null)
            {
                contractor = (from co in entities.Contractors where (co.CompanyName == chosenEntity) select co).FirstOrDefault();
                chosenContractorId = contractor.Id_Contractor;
            }

            try
            {
                
                Timesheet chosenEntityData = (from chd in entities.Timesheets where (chd.Id_WorkAssignment == chosenWorkid) || (chd.Id_Employee == chosenEmployeeId) || (chd.Id_Contractor == chosenContractorId) select chd).FirstOrDefault();
                contractor = (from co in entities.Contractors where (co.Id_Contractor == chosenEntityData.Id_Contractor) select co).FirstOrDefault();
                employee = (from e in entities.Employees where (e.Id_Employee == chosenEntityData.Id_Employee) select e).FirstOrDefault();
                assignment = (from wa in entities.WorkAssignments where (wa.Id_WorkAssignment == chosenEntityData.Id_WorkAssignment) select wa).FirstOrDefault();               
                string[] everyContractor = (from ec in entities.Timesheets where (ec.Id_Contractor == contractor.Id_Contractor) select contractor.CompanyName + " | " + employee.FirstName + " | " + assignment.Title).ToArray();


                WorkModel chosenEntityModel = new WorkModel()
                {
                    ContractorPickerData = everyContractor,
                    ContractorName = contractor.CompanyName,
                    FirstName = employee.FirstName + " " + employee.LastName,
                    WorkTitle = assignment.Title,
                    StartTime = chosenEntityData.StartTime.Value,
                    StopTime = chosenEntityData.StopTime.Value,
                    CountedHours = (chosenEntityData.StopTime.Value - chosenEntityData.StartTime.Value).TotalHours
                };

                return chosenEntityModel;

            }
           
            finally
            {
                entities.Dispose();
            }
            
        }*/

    }
}
