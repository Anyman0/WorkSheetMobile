using Newtonsoft.Json;
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
	public partial class AssignWorkPopupView
	{
        public string workId;
		public AssignWorkPopupView (string WorkID)
		{
			InitializeComponent ();
            workId = WorkID;
		}

        private async void AssignWorkButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                WorkModel data = new WorkModel()
                {
                    Operation = "Assign",
                    WorkTitle = TitleLabel.Text,
                    EmployeeId = EmployeePicker.SelectedIndex
                };

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
                string input = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");

                HttpResponseMessage message = await client.PostAsync("/api/work", content);
                string reply = await message.Content.ReadAsStringAsync();
                bool success = JsonConvert.DeserializeObject<bool>(reply);

                if (success)
                {
                    await DisplayAlert("Saved!", "Work has been assigned!", "Close");
                    await PopupNavigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Save Failed", "Sorry, could not assign work...", "Close");
                }

            }
            catch
            {
                await DisplayAlert("Save Failed!", "Sorry, couldn't get any data from database..", "OK");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
                string json = await client.GetStringAsync("/api/work?workName=" + workId);
                WorkModel chosenWorkModel = JsonConvert.DeserializeObject<WorkModel>(json);
                TitleLabel.Text = chosenWorkModel.WorkTitle;
                DescriptionLabel.Text = chosenWorkModel.Description;
                DeadlineLabel.Text = chosenWorkModel.Deadline.ToString();
             
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetType().Name + ": " + ex.Message;
                TitleLabel.Text = errorMessage;
            }

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
                string json = await client.GetStringAsync("/api/employee");
                string[] employees = JsonConvert.DeserializeObject<string[]>(json);
                EmployeePicker.ItemsSource = employees;
            }
            catch
            {

            }
        }
    }
}