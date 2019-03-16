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
    public class WorkController : ApiController
    {

        public string[] GetAll()
        {
            string[] WorkAssignments = null;
            WorksheetEntities entities = new WorksheetEntities();

            try
            {
                WorkAssignments = (from wa in entities.WorkAssignments where (wa.Active == true) && (wa.InProgressAt == null) select wa.Title).ToArray();
            }
            finally
            {
                entities.Dispose();
            }

            return WorkAssignments;
        }


        // GET: Get works in progress
        public string[] GetWorksInProgress(int id)
        {
            WorksheetEntities entities = new WorksheetEntities();
            id = 5; 
            try
            {
                
                string[] chosenWorkData = (from cw in entities.WorkAssignments
                                           where (cw.Active == true) && (cw.InProgressAt != null) && (cw.Completed != true)
                                           select cw.Title + " | Started at:  " + cw.InProgressAt).ToArray();

                return chosenWorkData;
            }

            finally
            {
                entities.Dispose();
            }

            
        }

        // GET: Trying to return model
        public WorkModel GetModel(string workName)
        {
            WorksheetEntities entities = new WorksheetEntities();

            try
            {
                string chosenWorkId = workName;
                WorkAssignment chosenWorkData = (from cw in entities.WorkAssignments
                                           where (cw.Active == true) && (cw.Title == chosenWorkId)
                                           select cw).FirstOrDefault();

                WorkModel chosenWorkModel = new WorkModel()
                {
                    WorkTitle = chosenWorkData.Title,
                    Description = chosenWorkData.Description,
                    Deadline = chosenWorkData.Deadline.Value
                };

                return chosenWorkModel;
            }

            finally
            {
                entities.Dispose();
            }

        }

        // POST: New Work Entry
        [HttpPost]
        public bool PostWork(WorkModel model)
        {
            WorksheetEntities entities = new WorksheetEntities();

            Customer customer = (from cu in entities.Customers where (cu.Active == true) && (cu.CustomerName == model.CustomerName) select cu).FirstOrDefault();

            try
            {
                // Save chosen work
                if (model.Operation == "Save")
                {
                    WorkAssignment newEntry = new WorkAssignment()
                    {
                        Id_Customer = customer.Id_Customer,
                        Title = model.WorkTitle,
                        Description = model.Description,
                        Deadline = model.Deadline,
                        InProgress = true,
                        CreatedAt = DateTime.Now,
                        Active = true
                    };

                    entities.WorkAssignments.Add(newEntry);                   
                }
                // Modify chosen work
                else if (model.Operation == "Modify")
                {
                    WorkAssignment assignment = (from wa in entities.WorkAssignments where (wa.Active == true) && (wa.Title == model.WorkTitle) select wa).FirstOrDefault();
                    if (assignment == null)
                    {
                        return false;
                    }
                    int workId = assignment.Id_WorkAssignment;
                    WorkAssignment existing = (from wa in entities.WorkAssignments where (wa.Id_WorkAssignment == workId) && (wa.Active == true) select wa).FirstOrDefault();
                    if (existing != null)
                    {
                        existing.Title = model.WorkTitle;
                        existing.Description = model.Description;
                        existing.Deadline = model.Deadline;
                        existing.LastModifiedAt = DateTime.Now;
                    }
                    else
                    {
                        return false;
                    }
                   
                }
                // Delete chosen work
                else if (model.Operation == "Delete")
                {
                    WorkAssignment assignment = (from wa in entities.WorkAssignments where (wa.Title == model.WorkTitle) select wa).FirstOrDefault();
                    if (assignment == null)
                    {
                        return false;
                    }
                    int workId = assignment.Id_WorkAssignment;
                    WorkAssignment existing = (from wa in entities.WorkAssignments where (wa.Id_WorkAssignment == workId) select wa).FirstOrDefault();
                    if (existing != null)
                    {
                        entities.WorkAssignments.Remove(existing);
                    }
                    else
                    {
                        return false;
                    }
                    
                }
                // Assign chosen work to an employee
                else if(model.Operation == "Assign")
                {
                    
                    WorkAssignment assignment = (from wa in entities.WorkAssignments where (wa.Title == model.WorkTitle) && (wa.Active == true) && (wa.InProgress == true) select wa).FirstOrDefault();
                    if (assignment == null)
                    {
                        return false;
                    }

                    Employee emp = (from e in entities.Employees where (e.FirstName + " " + e.LastName == model.FirstName) select e).FirstOrDefault();
                    if (emp == null)
                    {
                        return false;
                    }

                    int workId = assignment.Id_WorkAssignment;
                    int customerId = assignment.Id_Customer.Value;                   

                    assignment.InProgressAt = DateTime.Now;

                    Timesheet newEntry = new Timesheet()
                    {
                        Id_Customer = customerId,
                        Id_Contractor = emp.Id_Contractor,
                        Id_Employee = emp.Id_Employee,
                        Id_WorkAssignment = workId,
                        StartTime = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        Active = true,
                        WorkComplete = false
                    };

                    entities.Timesheets.Add(newEntry);
                                       
                }

                else if(model.Operation == "MarkComplete")
                {
                    WorkAssignment assignment = (from wa in entities.WorkAssignments where (wa.Title + " | Started at:  " + wa.InProgressAt == model.WorkTitle) && (wa.Active == true) && (wa.InProgress == true) && (wa.InProgressAt != null) select wa).FirstOrDefault();
                    if (assignment == null)
                    {
                        return false;
                    }

                    int workId = assignment.Id_WorkAssignment;
                    int customerId = assignment.Id_Customer.Value;

                    assignment.CompletedAt = DateTime.Now;
                    assignment.Completed = true;
                    assignment.InProgress = false;

                    Timesheet existing = (from e in entities.Timesheets where (e.Id_WorkAssignment == workId) && (e.Id_Customer == customerId) select e).FirstOrDefault();
                    if (existing != null)
                    {
                        existing.WorkComplete = true;
                        existing.StopTime = DateTime.Now;
                        existing.LastModifiedAt = DateTime.Now;
                        existing.Comments = "Work set to complete by Admin";
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
