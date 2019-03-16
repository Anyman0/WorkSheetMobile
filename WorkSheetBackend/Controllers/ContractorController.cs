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
    public class ContractorController : ApiController
    {
        public string[] GetAll()
        {
            string[] contractors = null;
            WorksheetEntities entities = new WorksheetEntities();

            try
            {
                contractors = (from c in entities.Contractors where (c.Active == true) select c.CompanyName).ToArray();
            }

            finally
            {
                entities.Dispose();
            }
            
            return contractors;
        }


        // GET: Getting model
        public WorkModel GetContractorModel(string contractorName)
        {
            WorksheetEntities entities = new WorksheetEntities();

            try
            {

                string chosenContractor = contractorName;
                Contractor contractor = (from co in entities.Contractors where (co.Active == true) && (co.CompanyName == chosenContractor) select co).FirstOrDefault();

                WorkModel chosenContractorModel = new WorkModel()
                {
                    ContractorId = contractor.Id_Contractor,
                    ContractorName = contractor.CompanyName,
                    ContractorContactPerson = contractor.ContactPerson,
                    ContractorPhoneNumber = contractor.PhoneNumber,
                    ContractorEmail = contractor.EmailAddress,
                    VatId = contractor.VatId,
                    HourlyRate = contractor.HourlyRate.ToString()
                };

                return chosenContractorModel;
            }

            finally
            {
                entities.Dispose();
            }

        }

        [HttpPost]
        // POST: Contractors
        public bool PostContractor(WorkModel model)
        {
            WorksheetEntities entities = new WorksheetEntities();
             
            try
            {
                if (model.ContOperation == "Save")
                {
                    Contractor newEntry = new Contractor()
                    {                       
                        CompanyName = model.ContractorName,
                        ContactPerson = model.ContractorContactPerson,
                        PhoneNumber = model.ContractorPhoneNumber,
                        EmailAddress = model.ContractorEmail,
                        VatId = model.VatId,
                        HourlyRate = int.Parse(model.HourlyRate),
                        Active = true,
                        CreatedAt = DateTime.Now
                    };

                    entities.Contractors.Add(newEntry);
                }

                // Modify chosen contractor
                else if (model.ContOperation == "Modify")
                {
                    Contractor contractor = (from co in entities.Contractors where (co.Active == true) && (co.CompanyName == model.ContractorName) select co).FirstOrDefault();
                    if (contractor == null)
                    {
                        return false;
                    }
                    int contractorId = contractor.Id_Contractor;
                    Contractor existing = (from co in entities.Contractors where (co.Id_Contractor == contractorId) && (co.Active == true) select co).FirstOrDefault();
                    if (existing != null)
                    {
                        existing.CompanyName = model.ContractorName;
                        existing.ContactPerson = model.ContractorContactPerson;
                        existing.PhoneNumber = model.ContractorPhoneNumber;
                        existing.EmailAddress = model.ContractorEmail;
                        existing.VatId = model.VatId;
                        existing.HourlyRate = int.Parse(model.HourlyRate);
                        existing.LastModifiedAt = DateTime.Now;
                    }
                    else
                    {
                        return false;
                    }

                }


                // Delete chosen contractor
                else if (model.ContOperation == "Delete")
                {
                    Contractor chosenContractor = (from cco in entities.Contractors where (cco.CompanyName == model.ContractorName) select cco).FirstOrDefault();
                    if (chosenContractor == null)
                    {
                        return false;
                    }
                    int contractorId = chosenContractor.Id_Contractor;
                    Contractor existing = (from e in entities.Contractors where (e.Id_Contractor == contractorId) select e).FirstOrDefault();
                    if (existing != null)
                    {
                        entities.Contractors.Remove(existing);
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
