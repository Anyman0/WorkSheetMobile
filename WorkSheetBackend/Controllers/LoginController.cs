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
    public class LoginController : ApiController
    {

        public bool GetLogin(WorkModel logData)
        {
            
            WorksheetEntities entities = new WorksheetEntities();

            try
            {

                
            }

            finally
            {
                entities.Dispose();
            }
            
            return true;
        }

    }
}
