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
                WorkAssignments = (from wa in entities.WorkAssignments where (wa.Active == true) select wa.Title).ToArray();
            }
            finally
            {
                entities.Dispose();
            }

            return WorkAssignments;
        }


        // GET: Get chosen information of the chosen work
        public string[] GetChosenWork(string workId)
        {
            WorksheetEntities entities = new WorksheetEntities();

            try
            {
                string chosenWorkId = workId;
                string[] chosenWorkData = (from cw in entities.WorkAssignments
                                           where (cw.Active == true) && (cw.Title == chosenWorkId)
                                           select cw.Title + " " + cw.Description + " " + cw.Deadline).ToArray();

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

            try
            {
                // Save chosen work
                if (model.Operation == "Save")
                {
                    WorkAssignment newEntry = new WorkAssignment()
                    {
                        Id_Customer = model.CustomerId,
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
