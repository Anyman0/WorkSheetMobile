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
            var AddNewEmployee = new ToolbarItem()
            {
                Text = "New",                
            };
            AddNewEmployee.Clicked += AddNewEmployee_Clicked;
            ToolbarItems.Add(AddNewEmployee);

            var ModifyEmployee = new ToolbarItem()
            {
                Text = "Modify"
            };
            ToolbarItems.Add(ModifyEmployee);
            ModifyEmployee.Clicked += ModifyEmployee_Clicked;

            var DeleteEmployee = new ToolbarItem()
            {
                Text = "Delete",               
            };
            ToolbarItems.Add(DeleteEmployee);
            DeleteEmployee.Clicked += DeleteEmployee_Clicked;
          
		}

        //Toolbaritem OnClicks
        private async void DeleteEmployee_Clicked(object sender, EventArgs e)
        {
            string chosen = employeeList.SelectedItem?.ToString();
            string[] employee = chosen.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            int empID = employee.Count();
            string chosenEmployee = employee[empID - 2];

            if (chosenEmployee == null)
            {
                await DisplayAlert("Whoopsie", "Choose an employee first.", "OK");
            }
            else
            {
                await PopupNavigation.PushAsync(new DeleteEmployeePopupView(chosenEmployee));
            }
        }

        private async void ModifyEmployee_Clicked(object sender, EventArgs e)
        {
            string chosen = employeeList.SelectedItem?.ToString();           
            string[] employee = chosen.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            int empID = employee.Count();
            string chosenEmployee = employee[empID - 2];          

            if (chosenEmployee == null)
            {
                await DisplayAlert("Whoopsie", "Choose an employee first.", "OK");
            }
            else
            {
                await PopupNavigation.PushAsync(new ModifyEmployeePopupView(chosenEmployee));
            }
        }

        private async void AddNewEmployee_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new NewEmployeePopupView());
        }
       

        private void EmployeeList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            
        }
               

        protected override async void OnAppearing()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
            string json = await client.GetStringAsync("/api/employee");
            List<string> employees = JsonConvert.DeserializeObject<List<string>>(json);
            employeeList.ItemsSource = employees;           

            base.OnAppearing();
        }
       
    }
}