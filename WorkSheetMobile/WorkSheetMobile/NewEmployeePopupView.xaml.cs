using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WorkSheetMobile.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace WorkSheetMobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NewEmployeePopupView
	{
		public NewEmployeePopupView ()
		{
			InitializeComponent ();           
		}
       

        private async void SaveEmployeeButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                SHA512 sha512 = SHA512.Create();
                byte[] bytes = Encoding.UTF8.GetBytes(EmployeePasswordEntry.Text);
                byte[] hash = sha512.ComputeHash(bytes);
               

                WorkModel data = new WorkModel()
                {
                    EmpOperation = "Save",
                    ContractorName = ContractorPicker.SelectedItem.ToString(),
                    UserName = EmployeeUsernameEntry.Text, 
                    Password = hash,
                    FirstName = FirstNameEntry.Text,
                    LastName = LastNameEntry.Text,
                    PhoneNumber = int.Parse(PhoneNumberEntry.Text),
                    Email = EmailEntry.Text
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
                    await DisplayAlert ("Saved!", "A new employee has been added!", "Close");
                    await PopupNavigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Save Failed", "Sorry, could not save employee..", "Close");
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

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
            string json = await client.GetStringAsync("/api/contractor");
            string[] contractors = JsonConvert.DeserializeObject<string[]>(json);
            ContractorPicker.ItemsSource = contractors;
            
        }
    }
}