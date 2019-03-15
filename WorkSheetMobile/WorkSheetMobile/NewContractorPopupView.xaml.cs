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
	public partial class NewContractorPopupView
	{
		public NewContractorPopupView ()
		{
			InitializeComponent ();
            HourlyRateEntry.Keyboard = Keyboard.Numeric;
		}

        private async void SaveContractorButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                WorkModel data = new WorkModel()
                {
                    ContOperation = "Save",
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
                    await DisplayAlert("Saved!", "New contractor has been added!", "Close");
                    await PopupNavigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Save Failed", "Sorry, could not save contractor..", "Close");
                }

            }
            catch
            {
                await DisplayAlert("Save Failed!", "Sorry, couldn't get any data from database..", "OK");
            }
        }
    }
}