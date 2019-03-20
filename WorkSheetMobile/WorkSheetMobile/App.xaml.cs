using System;
using System.IO;
using WorkSheetMobile.Data;
using WorkSheetMobile.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace WorkSheetMobile
{
	public partial class App : Application
	{
        static LoginDatabase lDatabase;
		public App ()
		{
			InitializeComponent();

            MainPage = new NavigationPage(new LoginPage());                    
          
        }

        public static LoginDatabase LDatabase
        {
            get
            {
                if (lDatabase == null)
                {
                    lDatabase = new LoginDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LogInData.db"));
                }

                return lDatabase;
            }
        }
        
        protected override void OnStart ()
		{            
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
