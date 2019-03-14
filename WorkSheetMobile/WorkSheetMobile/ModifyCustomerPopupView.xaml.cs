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
	public partial class ModifyCustomerPopupView
	{
        public string customerID;
		public ModifyCustomerPopupView (string CustID)
		{
			InitializeComponent ();
            customerID = CustID;
		}

        private async void SaveChanges_Clicked(object sender, EventArgs e)
        {
            try
            {
                WorkModel data = new WorkModel()
                {
                    CustOperation = "Modify",
                    CustomerName = CustomerNameEntry.Text,
                    ContactPerson = ContactPersonEntry.Text,
                    CustomerPhoneNumber = PhoneNumberEntry.Text,
                    CustomerEmail = EmailEntry.Text
                };

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
                string input = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");

                HttpResponseMessage message = await client.PostAsync("/api/customer", content);
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
                string json = await client.GetStringAsync("/api/customer?customerName=" + customerID);
                WorkModel chosenCustomerModel = JsonConvert.DeserializeObject<WorkModel>(json);
                CustomerNameEntry.Text = chosenCustomerModel.CustomerName;
                ContactPersonEntry.Text = chosenCustomerModel.ContactPerson;
                PhoneNumberEntry.Text = chosenCustomerModel.CustomerPhoneNumber;
                EmailEntry.Text = chosenCustomerModel.CustomerEmail;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetType().Name + ": " + ex.Message;
                CustomerNameEntry.Text = errorMessage;
            }

        }
    }
}