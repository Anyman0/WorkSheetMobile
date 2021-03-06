﻿using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WorkSheetMobile.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkSheetMobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DeleteEmployeePopupView
	{
        string employeeId;
		public DeleteEmployeePopupView (string chosenEmployee)
		{
			InitializeComponent ();
            employeeId = chosenEmployee;
		}

        private async void DeleteEmployeeButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                WorkModel data = new WorkModel()
                {
                    EmpOperation = "Delete",
                    EmployeeId = int.Parse(employeeId)
                };

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
                string input = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");

                HttpResponseMessage message = await client.PostAsync("/api/employee", content);
                string reply = await message.Content.ReadAsStringAsync();
                bool success = JsonConvert.DeserializeObject<bool>(reply);

                if (success)
                {
                    await DisplayAlert("Delete complete", "Employee has been deleted", "Close");
                    await PopupNavigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Delete failed!", "Could not delete employee..", "Close");
                }
            }
            catch
            {
                await DisplayAlert("Delete failed!", "Sorry, could not get any data from database..", "OK");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
                string json = await client.GetStringAsync("/api/employee?employeeName=" + employeeId);
                WorkModel chosenWorkModel = JsonConvert.DeserializeObject<WorkModel>(json);
                EmployeeNameLabel.Text = chosenWorkModel.FirstName + " " + chosenWorkModel.LastName;
                EmployeeEmailLabel.Text = chosenWorkModel.Email;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetType().Name + ": " + ex.Message;
                EmployeeNameLabel.Text = errorMessage;
            }
        }
    }
}