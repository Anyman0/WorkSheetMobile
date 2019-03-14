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
    public class CustomerController : ApiController
    {
        public string[] GetAll()
        {
            string[] customers = null;
            WorksheetEntities entities = new WorksheetEntities();

            try
            {
                customers = (from c in entities.Customers where (c.Active == true) select c.CustomerName).ToArray();
            }

            finally
            {
                entities.Dispose();
            }

            return customers;
        }


        // GET: Getting model
        public WorkModel GetCustomerModel(string customerName)
        {
            WorksheetEntities entities = new WorksheetEntities();

            try
            {

                string chosenCustomer = customerName;
                Customer customer = (from c in entities.Customers where (c.Active == true) && (c.CustomerName == chosenCustomer) select c).FirstOrDefault();

                WorkModel chosenCustomerModel = new WorkModel()
                {
                    CustomerName = customer.CustomerName,
                    ContactPerson = customer.ContactPerson,
                    CustomerPhoneNumber = customer.PhoneNumber,
                    CustomerEmail = customer.EmailAddress
                };

                return chosenCustomerModel;
            }

            finally
            {
                entities.Dispose();
            }

        }


        [HttpPost]
        // POST: Customers
        public bool PostCustomer(WorkModel model)
        {
            WorksheetEntities entities = new WorksheetEntities();

            try
            {
                if (model.CustOperation == "Save")
                {
                    Customer newEntry = new Customer()
                    {
                        CustomerName = model.CustomerName,
                        ContactPerson = model.ContactPerson,
                        PhoneNumber = model.CustomerPhoneNumber,
                        EmailAddress = model.CustomerEmail,
                        CreatedAt = DateTime.Now,
                        Active = true
                    };

                    entities.Customers.Add(newEntry);
                }

                // Modify chosen customer
                else if (model.CustOperation == "Modify")
                {
                    Customer customer = (from c in entities.Customers where (c.Active == true) && (c.CustomerName == model.CustomerName) select c).FirstOrDefault();
                    if (customer == null)
                    {
                        return false;
                    }
                    int customerId = customer.Id_Customer;
                    Customer existing = (from c in entities.Customers where (c.Id_Customer == customerId) && (c.Active == true) select c).FirstOrDefault();
                    if (existing != null)
                    {
                        existing.CustomerName = model.CustomerName;
                        existing.ContactPerson = model.ContactPerson;
                        existing.PhoneNumber = model.CustomerPhoneNumber;
                        existing.EmailAddress = model.CustomerEmail;
                        existing.LastModifiedAt = DateTime.Now;
                    }
                    else
                    {
                        return false;
                    }

                }


                // Delete chosen customer
                else if (model.CustOperation == "Delete")
                {
                    Customer chosenCustomer = (from cc in entities.Customers where (cc.CustomerName == model.CustomerName) select cc).FirstOrDefault();
                    if (chosenCustomer == null)
                    {
                        return false;
                    }
                    int customerId = chosenCustomer.Id_Customer;
                    Customer existing = (from e in entities.Customers where (e.Id_Customer == customerId) select e).FirstOrDefault();
                    if (existing != null)
                    {
                        entities.Customers.Remove(existing);
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
