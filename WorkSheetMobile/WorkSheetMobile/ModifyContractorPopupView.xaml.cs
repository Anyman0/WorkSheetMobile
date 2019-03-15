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
	public partial class ModifyContractorPopupView
	{
        public string contractorID;
		public ModifyContractorPopupView (string ContractorId)
		{
			InitializeComponent ();
            contractorID = ContractorId;
		}

        private async void SaveChangesButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                WorkModel data = new WorkModel()
                {
                    ContOperation = "Modify",
                    ContractorName = ContractorNameEntry.Text,
                    ContractorContactPerson = ContactPersonEntry.Text,
                    ContractorPhoneNumber = PhoneNumberEntry.Text,
                    ContractorEmail = EmailEntry.Text,
                    VatId = VatIDEntry.Text,
                    HourlyRate = HourlyRateEntry.Text
                };

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
                string input = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");

                HttpResponseMessage message = await client.PostAsync("/api/contractor", content);
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
                string json = await client.GetStringAsync("/api/contractor?contractorName=" + contractorID);
                WorkModel chosenCustomerModel = JsonConvert.DeserializeObject<WorkModel>(json);
                ContractorNameEntry.Text = chosenCustomerModel.ContractorName;
                ContactPersonEntry.Text = chosenCustomerModel.ContractorContactPerson;
                PhoneNumberEntry.Text = chosenCustomerModel.ContractorPhoneNumber;
                EmailEntry.Text = chosenCustomerModel.ContractorEmail;
                VatIDEntry.Text = chosenCustomerModel.VatId;
                HourlyRateEntry.Text = chosenCustomerModel.HourlyRate;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetType().Name + ": " + ex.Message;
                ContractorNameEntry.Text = errorMessage;
            }
        }
    }
}