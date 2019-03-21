
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WorkSheetMobile.Models;
using Xamarin.Forms;

namespace WorkSheetMobile
{
	public partial class MainPage : ContentPage
	{
       
        public string Name;        
        public MainPage(string usrName)
        {
            InitializeComponent();
            Name = usrName;          
            GetUsersName();
        }

        private async void ToEmployeesButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EmployeeList());
        }

        private async void ToCustomersButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CustomerList());
        }

        private async void ToContractorsButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ContractorList());
        }

        private async void ToWorkassignmentsButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new WorkList());
        }

        private async void ToTimesheetsButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TSList());
        }

        private async void LogOutButton_Clicked(object sender, EventArgs e)
        {
            LoginDBModel data = new LoginDBModel();
            await App.LDatabase.DeleteItemAsync(data);
            await Navigation.PushAsync(new LoginPage());
        }

        public async void GetUsersName()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
            string json = await client.GetStringAsync("/api/login?userName=" + Name);
            WorkModel ProfileInfo = JsonConvert.DeserializeObject<WorkModel>(json);
            WelcomeUserLabel.Text = "Welcome " +ProfileInfo.FirstName + " " + ProfileInfo.LastName + "!";
        }

        protected override async void OnAppearing()
        {
            
        }
    }
}
