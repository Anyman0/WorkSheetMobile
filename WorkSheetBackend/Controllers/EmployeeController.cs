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
    public class EmployeeController : ApiController
    {       
        public List<string> GetAll()
        {
            string[] employees = null;          
            WorksheetEntities entities = new WorksheetEntities();

            List<string> empList = new List<string>();
            employees = (from e in entities.Employees where (e.Active == true) select e.Id_Contractor + " " + e.Id_Employee).ToArray();
 
            try
            {


                foreach (var item in employees)
                {
                    string[] data = item.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string contractor = data[0];
                    string contr = (from c in entities.Contractors where (c.Id_Contractor.ToString() == contractor) select c.CompanyName).Single();
                    string employee = data[1];
                    string emp = (from e in entities.Employees where (e.Id_Employee.ToString() == employee) select e.FirstName + " " + e.LastName).Single();                  

                    empList.Add("Contractor: " + contr + " | Name: " + emp + " ( " + employee + " )" );
                }

            }
            finally
            {
                entities.Dispose();
            }

            return empList;
        }
      

        // GET: Getting model
        public WorkModel GetModel(string employeeName)
        {
            WorksheetEntities entities = new WorksheetEntities();

            try
            {
                 
                string chosenEmployee = employeeName;
                Employee employee = (from e in entities.Employees where (e.Active == true) && (e.Id_Employee.ToString() == chosenEmployee) select e).FirstOrDefault();

                WorkModel chosenEmployeeModel = new WorkModel()
                {
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    PhoneNumber = int.Parse(employee.PhoneNumber),
                    Email = employee.EmailAddress,
                };

                return chosenEmployeeModel;
            }

            finally
            {
                entities.Dispose();
            }

        }

        // POST New Employee Entry
        [HttpPost]
        public bool PostEmployee(WorkModel model)
        {
            WorksheetEntities entities = new WorksheetEntities();
            
            try
            {
                if (model.EmpOperation == "Save")
                {
                    Contractor contractor = (from c in entities.Contractors where (c.CompanyName == model.ContractorName) select c).FirstOrDefault();
                    Employee newEntry = new Employee()
                    {
                        Id_Contractor = contractor.Id_Contractor,
                        Username = model.UserName,
                        Password = model.Password,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber.ToString(),
                        EmailAddress = model.Email,
                        CreatedAt = DateTime.Now,
                        Active = true,
                    };

                    entities.Employees.Add(newEntry);
                }

                // Modify chosen employee
                else if (model.EmpOperation == "Modify")
                {

                    
                    Employee existing = (from e in entities.Employees where (e.Id_Employee == model.EmployeeId) && (e.Active == true) select e).FirstOrDefault();
                    if (existing != null)
                    {
                        existing.FirstName = model.FirstName;
                        existing.LastName = model.LastName;
                        existing.PhoneNumber = model.PhoneNumber.ToString();
                        existing.EmailAddress = model.Email;
                        existing.LastModifiedAt = DateTime.Now;
                        existing.EmployeePicture = model.Picture;
                    }
                    else
                    {
                        return false;
                    }
                }

                // Delete chosen work
                else if (model.EmpOperation == "Delete")
                {
                    
                    Employee existing = (from e in entities.Employees where (e.Id_Employee == model.EmployeeId) select e).FirstOrDefault();
                    if (existing != null)
                    {
                        entities.Employees.Remove(existing);
                    }
                    else
                    {
                        return false;
                    }
                }

                    entities.SaveChanges();
            }
            catch
            {
                return false;
            }
            finally
            {
                entities.Dispose();
            }

            return true;
        }

       
    }
}
