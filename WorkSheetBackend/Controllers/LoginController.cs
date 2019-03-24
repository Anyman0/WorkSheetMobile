using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using WorkSheetBackend.DataAccess;
using WorkSheetMobile.Models;

namespace WorkSheetBackend.Controllers
{
    public class LoginController : ApiController
    {
        [HttpGet]
        [HttpPost]
        
        public bool GetLogin(string LogData)
        {
            WorksheetEntities entities = new WorksheetEntities();

            string[] logDataParts = LogData.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string usrname = logDataParts[0];
            string pw = logDataParts[1];

            SHA512 sha512 = SHA512.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(pw);
            byte[] hash = sha512.ComputeHash(bytes);

            Employee emp = (from e in entities.Employees where (e.Username == usrname) select e).FirstOrDefault();

            StringBuilder dbPW = new StringBuilder();
            for (int i = 0; i < emp.Password.Length; i++)
            {
                dbPW.Append(emp.Password[i].ToString("X2"));
            }

            StringBuilder logDataPW = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                logDataPW.Append(hash[i].ToString("X2"));
            }

            string DBPass = dbPW.ToString();
            string logDataPass = logDataPW.ToString();

            if (emp.Username + DBPass == usrname + logDataPass)
            {
                return true;
            }

            return false;

        }

        [HttpGet]
        public WorkModel GetProfileInfo(string userName)
        {
            
            WorksheetEntities entities = new WorksheetEntities();

            try
            {
               
                Employee profile = (from p in entities.Employees where (p.Active == true) && (p.Username == userName) select p).FirstOrDefault();
                string cont = (from c in entities.Contractors where (c.Id_Contractor == profile.Id_Contractor) select c.CompanyName).Single();


                WorkModel loggedProfile = new WorkModel()
                {
                    ContractorName = cont,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    PhoneNumber = int.Parse(profile.PhoneNumber),
                    Email = profile.EmailAddress,
                    Picture = profile.EmployeePicture,
                    UserName = profile.Username
                };

                return loggedProfile;
            }

            finally
            {
                entities.Dispose();
            }
        }


        [HttpPost]
        public bool PostChanges(WorkModel model)
        {
            WorksheetEntities entities = new WorksheetEntities();

            try
            {
                Employee MyProfile = (from ce in entities.Employees where (ce.Active == true) && (ce.FirstName + " " + ce.LastName == model.FirstName + " " + model.LastName) select ce).FirstOrDefault();
                if (MyProfile == null)
                {
                    return false;
                }
                int employeeId = MyProfile.Id_Employee;
                Employee existing = (from e in entities.Employees where (e.Id_Employee == employeeId) && (e.Active == true) select e).FirstOrDefault();
                if (existing != null && model.Picture != null && model.Operation == "Save")
                {
                    existing.FirstName = model.FirstName;
                    existing.LastName = model.LastName;
                    existing.PhoneNumber = model.PhoneNumber.ToString();
                    existing.EmailAddress = model.Email;
                    existing.LastModifiedAt = DateTime.Now;                  
                    existing.EmployeePicture = model.Picture;                   
                }
                else if (existing != null && model.Picture == null && model.Operation == "Save")
                {
                    existing.FirstName = model.FirstName;
                    existing.LastName = model.LastName;
                    existing.PhoneNumber = model.PhoneNumber.ToString();
                    existing.EmailAddress = model.Email;
                    existing.LastModifiedAt = DateTime.Now;
                }
                else if (existing != null && model.Operation == "SavePW")
                {
                    existing.Password = model.Password;
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
