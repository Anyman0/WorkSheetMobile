using SQLite;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WorkSheetMobile.Models;

namespace WorkSheetMobile.Data
{
   public class LoginDatabase
   {

        readonly SQLiteAsyncConnection lDatabase;

        public LoginDatabase(string dbPath)
        {
            lDatabase = new SQLiteAsyncConnection(dbPath);
            lDatabase.CreateTableAsync<LoginDBModel>().Wait();
        }

        public Task<List<LoginDBModel>> GetData()
        {
            
            return lDatabase.Table<LoginDBModel>().ToListAsync();
            
        }
       
                 
        public Task<int> SaveItemAsync(LoginDBModel model)
        {           

            return lDatabase.InsertAsync(model);
          
        }

        public Task<int> DeleteItemAsync(LoginDBModel model)
        {
            //return lDatabase.DropTableAsync<LoginDBModel>();
            return lDatabase.DeleteAllAsync<LoginDBModel>();           
        }



    }
}
