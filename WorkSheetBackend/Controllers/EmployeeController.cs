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
        public string[] GetAll()
        {
            string[] employees = null;
            WorksheetEntities entities = new WorksheetEntities();
            try
            {
                employees = (from e in entities.Employees where (e.Active == true) select e.FirstName + " " + e.LastName).ToArray();
            }
            finally
            {
                entities.Dispose();
            }

            return employees;
        }


        // GET: Getting model
        public WorkModel GetModel(string employeeName)
        {
            WorksheetEntities entities = new WorksheetEntities();

            try
            {
                 
                string chosenEmployee = employeeName;
                Employee employee = (from e in entities.Employees where (e.Active == true) && (e.FirstName + " " + e.LastName == chosenEmployee) select e).FirstOrDefault();

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
                    Employee newEntry = new Employee()
                    {
                        Id_Contractor = model.ContractorId,
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

                    Employee chosenEmployee = (from ce in entities.Employees where (ce.Active == true) && (ce.FirstName + " " + ce.LastName == model.FirstName + " " + model.LastName) select ce).FirstOrDefault();
                    if (chosenEmployee == null)
                    {
                        return false;
                    }
                    int employeeId = chosenEmployee.Id_Employee;
                    Employee existing = (from e in entities.Employees where (e.Id_Employee == employeeId) && (e.Active == true) select e).FirstOrDefault();
                    if (existing != null)
                    {
                        existing.FirstName = model.FirstName;
                        existing.LastName = model.LastName;
                        existing.PhoneNumber = model.PhoneNumber.ToString();
                        existing.EmailAddress = model.Email;
                        existing.LastModifiedAt = DateTime.Now;
                    }
                    else
                    {
                        return false;
                    }
                }

                // Delete chosen work
                else if (model.EmpOperation == "Delete")
                {
                    Employee chosenEmployee = (from ce in entities.Employees where (ce.FirstName + " " + ce.LastName == model.FirstName) select ce).FirstOrDefault();
                    if (chosenEmployee == null)
                    {
                        return false;
                    }
                    int employeeId = chosenEmployee.Id_Employee;
                    Employee existing = (from e in entities.Employees where (e.Id_Employee == employeeId) select e).FirstOrDefault();
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
