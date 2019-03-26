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

               
        // Gets all the data for the picker
        public List<string[]> GetPickerData(int picker)
        {
            WorksheetEntities entities = new WorksheetEntities();

            List<string[]> pickerData = new List<string[]>();
            string[] emp = { "Employees:---------- " };
            string[] work = { "Workassignments:---------- " };
            string[] cont = { "Contractors:---------- " };           

            try
            {
                string[] workAssignments = (from wa in entities.WorkAssignments where (wa.Active == true) select wa.Title).ToArray();
                string [] employees = (from e in entities.Employees where (e.Active == true) select e.FirstName + " " + e.LastName).ToArray();
                string[] contractors = (from co in entities.Contractors where (co.Active == true) select co.CompanyName).ToArray();

                pickerData.Add(work);
                pickerData.Add(workAssignments);
                pickerData.Add(emp);
                pickerData.Add(employees);
                pickerData.Add(cont);
                pickerData.Add(contractors);
              
            }
            finally
            {
                entities.Dispose();
            }

            return pickerData;
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
                    
                    realList.Add(contr + " | " + emp + " | " + work + " " + "( " + workid + " )");
                }

            }            

            finally
            {
                entities.Dispose();
            }
           
            return realList;
        }

        // Method to get same data as in timesheet mainpage, but for the chosen entity only
        public List<string> GetChosenEntity (string Entity)
        {
            WorksheetEntities entities = new WorksheetEntities();
            string[] sheet = null;
                        

            string[] workAssignments = (from wa in entities.WorkAssignments where (wa.Active == true) select wa.Title).ToArray();
            string[] employees = (from e in entities.Employees where (e.Active == true) select e.FirstName + " " + e.LastName).ToArray();
            string[] contractors = (from cont in entities.Contractors where (cont.Active == true) select cont.CompanyName).ToArray();
            List<string[]> empData = new List<string[]>();
            List<string[]> contData = new List<string[]>();
            List<string[]> WorkData = new List<string[]>();
            WorkData.Add(workAssignments);
            empData.Add(employees);
            contData.Add(contractors);

            foreach (var item in empData)
            {
                foreach (var i in item)
                {
                    if (i.ToString() == Entity)
                    {
                        string emplo = (from e in entities.Employees where (e.FirstName + " " + e.LastName == Entity) select e.Id_Employee.ToString()).Single();
                        sheet = (from ts in entities.Timesheets where (ts.Id_Employee.ToString() == emplo) select ts.Id_Contractor.ToString() + " " + ts.Id_Employee.ToString() + " " + ts.Id_WorkAssignment.ToString()).ToArray();
                    }
                }
            }
            foreach (var itemm in WorkData)
            {
                foreach (var it in itemm)
                {
                    if (it.ToString() == Entity)
                    {
                        string worka = (from w in entities.WorkAssignments where (w.Title == Entity) select w.Id_WorkAssignment.ToString()).Single();
                        sheet = (from ts in entities.Timesheets where (ts.Id_WorkAssignment.ToString() == worka) select ts.Id_Contractor.ToString() + " " + ts.Id_Employee.ToString() + " " + ts.Id_WorkAssignment.ToString()).ToArray();
                    }
                }
            }
            foreach (var iten in contData)
            {
                foreach (var ite in iten)
                {
                    if (ite.ToString() == Entity)
                    {
                        string contra = (from c in entities.Contractors where (c.CompanyName == Entity) select c.Id_Contractor.ToString()).Single();
                        sheet = (from ts in entities.Timesheets where (ts.Id_Contractor.ToString() == contra) select ts.Id_Contractor.ToString() + " " + ts.Id_Employee.ToString() + " " + ts.Id_WorkAssignment.ToString()).ToArray();
                    }
                }
            }
            

            List<string> entityList = new List<string>();          

            try
            {


                foreach (var item in sheet)
                {
                    string[] datas = item.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string contractor = datas[0];
                    string contr = (from c in entities.Contractors where (c.Id_Contractor.ToString() == contractor) select c.CompanyName).Single();
                    string employee = datas[1];
                    string emp = (from e in entities.Employees where (e.Id_Employee.ToString() == employee) select e.FirstName + " " + e.LastName).Single();
                    string workid = datas[2];
                    string work = (from w in entities.WorkAssignments where (w.Id_WorkAssignment.ToString() == workid) select w.Title).Single();

                    entityList.Add(contr + " | " + emp + " | " + work + " " + "( " + workid + " )");
                }

            }

            finally
            {
                entities.Dispose();
            }

            return entityList;
        }


        public WorkModel GetDetailModel(string Details)
        {
            WorksheetEntities entities = new WorksheetEntities();

            string[] Sheet = Details.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);                     
            int count = Sheet.Count();
            string workid = Sheet[count - 2];
           
            Timesheet sheet = (from ts in entities.Timesheets where (ts.Id_WorkAssignment.ToString() == workid) select ts).FirstOrDefault();

            string cont = (from con in entities.Contractors where (con.Id_Contractor == sheet.Id_Contractor) select con.CompanyName).Single();
            string cust = (from c in entities.Customers where (c.Id_Customer == sheet.Id_Customer) select c.CustomerName).Single();
            string work = (from co in entities.WorkAssignments where (co.Id_WorkAssignment == sheet.Id_WorkAssignment) select co.Title).Single();
            string emp = (from e in entities.Employees where (e.Id_Employee == sheet.Id_Employee) select e.FirstName + " " + e.LastName).Single();
            double workTime = (sheet.StopTime.Value - sheet.StartTime.Value).TotalHours;
            string comments = (from ct in entities.Timesheets where (ct.Id_Timesheet == sheet.Id_Timesheet) select ct.Comments).Single();

            WorkModel details = new WorkModel()
            {
                CustomerName = cust,
                ContractorName = cont,
                FirstName = emp,
                WorkTitle = work,
                CountedHours = workTime,
                Comments = comments
            };

            return details;
        }
    }
}
