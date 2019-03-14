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
	public partial class DeleteCustomerPopupView
	{
        public string customerID;
		public DeleteCustomerPopupView (string CustID)
		{
			InitializeComponent ();
            customerID = CustID;
		}

        private async void DeleteCustomerButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                WorkModel data = new WorkModel()
                {
                    CustOperation = "Delete",
                    CustomerName = CustomerNameLabel.Text
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
                    await DisplayAlert("Delete complete", "Customer has been deleted", "Close");
                    await PopupNavigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Delete failed!", "Could not delete customer..", "Close");
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
                string json = await client.GetStringAsync("/api/customer?customerName=" + customerID);
                WorkModel chosenCustomerModel = JsonConvert.DeserializeObject<WorkModel>(json);
                CustomerNameLabel.Text = chosenCustomerModel.CustomerName;
                ContactPersonLabel.Text = chosenCustomerModel.ContactPerson;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetType().Name + ": " + ex.Message;
                CustomerNameLabel.Text = errorMessage;
            }
        }
    }
}