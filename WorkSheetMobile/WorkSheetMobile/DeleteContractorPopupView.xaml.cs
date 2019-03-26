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
	public partial class DeleteContractorPopupView
	{
        public string contractorId;
        public int contID;
		public DeleteContractorPopupView (string ContractorId)
		{
			InitializeComponent ();
            contractorId = ContractorId;
		}

        private async void DeleteContractorButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                WorkModel data = new WorkModel()
                {
                    ContOperation = "Delete",
                    ContractorId = contID
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
                    await DisplayAlert("Delete complete", "Contractor has been deleted", "Close");
                    await PopupNavigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Delete failed!", "Could not delete contractor..", "Close");
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
                string json = await client.GetStringAsync("/api/contractor?contractorName=" + contractorId);
                WorkModel chosenContractorModel = JsonConvert.DeserializeObject<WorkModel>(json);
                ContractorNameLabel.Text = chosenContractorModel.ContractorName;
                ContactPersonLabel.Text = chosenContractorModel.ContractorContactPerson;
                contID = chosenContractorModel.ContractorId;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetType().Name + ": " + ex.Message;
                ContractorNameLabel.Text = errorMessage;
            }
        }
    }
}