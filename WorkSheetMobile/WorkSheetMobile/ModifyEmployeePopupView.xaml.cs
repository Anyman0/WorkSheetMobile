using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.IO;
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
	public partial class ModifyEmployeePopupView
	{
        byte[] image;
        string employeeId;
		public ModifyEmployeePopupView (string chosenEmployee)
		{
			InitializeComponent ();
            employeeId = chosenEmployee;
		}

        private async void SaveChangesButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                WorkModel data = new WorkModel()
                {
                    EmpOperation = "Modify",
                    Picture = image,
                    FirstName = FirstNameEntry.Text,
                    LastName = LastNameEntry.Text,
                    PhoneNumber = int.Parse(PhoneNumberEntry.Text),
                    Email = EmailEntry.Text,
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
                    await DisplayAlert("Saved!", "Your changes have been saved!", "Close");
                    await PopupNavigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Save Failed", "Sorry, could not save changes..", "Close");
                }

            }
            catch
            {
                await DisplayAlert("Whoopsie", "Could not get data from database..", "OK");
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
                FirstNameEntry.Text = chosenWorkModel.FirstName;
                LastNameEntry.Text = chosenWorkModel.LastName;
                PhoneNumberEntry.Text = chosenWorkModel.PhoneNumber.ToString();
                EmailEntry.Text = chosenWorkModel.Email;

                if (chosenWorkModel.Picture != null)
                {
                    byte[] photo = chosenWorkModel.Picture;
                    Stream stream = new MemoryStream(photo);
                    ImageSource img = ImageSource.FromStream(() => stream);
                    EmployeePhoto.Source = img;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetType().Name + ": " + ex.Message;
                FirstNameEntry.Text = errorMessage;
            }
        }
        
    }
}