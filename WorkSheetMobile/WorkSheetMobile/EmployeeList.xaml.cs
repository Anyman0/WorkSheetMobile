using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using WorkSheetMobile.Models;
using Rg.Plugins.Popup.Services;

namespace WorkSheetMobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EmployeeList : ContentPage
	{
        public EmployeeList()
        {
            InitializeComponent();

            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        latitudeLabel.Text = GpsLocationModel.Latitude.ToString("0.000");
                        longitudeLabel.Text = GpsLocationModel.Longitude.ToString("0.000");
                    });

                    await Task.Delay(5000);
                }
            });

            employeeList.ItemSelected += EmployeeList_ItemSelected;

            //Toolbaritems created here
            var GoToWorkList = new ToolbarItem()
            {
                Text = "Work List",
                
            };
            GoToWorkList.Clicked += GoToWorkList_Clicked;
            ToolbarItems.Add(GoToWorkList);

            var toCustomers = new ToolbarItem()
            {
                Text = "Customers"
            };
            ToolbarItems.Add(toCustomers);
            toCustomers.Clicked += ToCustomers_Clicked;

            var toContractors = new ToolbarItem()
            {
                Text = "Contractors"
            };
            ToolbarItems.Add(toContractors);
            toContractors.Clicked += ToContractors_Clicked;

            var toTimesheets = new ToolbarItem()
            {
                Text = "Timesheets"
            };
            ToolbarItems.Add(toTimesheets);
            toTimesheets.Clicked += ToTimesheets_Clicked;
		}

      
        private void EmployeeList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ModifyEmployeeButton.IsEnabled = true;
            DeleteEmployeeButton.IsEnabled = true;
        }

        //Toolbaritem OnClicks
        private async void ToContractors_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ContractorList());
        }
        private async void ToCustomers_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CustomerList());
        }

        private async void GoToWorkList_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new WorkList());
        }

        private async void ToTimesheets_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TimesheetList());
        }

        // Button OnClick-methods
        private async void NewEmployeeButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new NewEmployeePopupView());
        }

        private async void ModifyEmployeeButton_Clicked(object sender, EventArgs e)
        {
            string chosenEmployee = employeeList.SelectedItem?.ToString();
            await PopupNavigation.PushAsync(new ModifyEmployeePopupView(chosenEmployee));
        }

        private async void DeleteEmployeeButton_Clicked(object sender, EventArgs e)
        {
            string chosenEmployee = employeeList.SelectedItem?.ToString();
            await PopupNavigation.PushAsync(new DeleteEmployeePopupView(chosenEmployee));
        }

        protected override async void OnAppearing()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
            string json = await client.GetStringAsync("/api/employee");
            string[] employees = JsonConvert.DeserializeObject<string[]>(json);
            employeeList.ItemsSource = employees;

            base.OnAppearing();
        }
       
    }
}