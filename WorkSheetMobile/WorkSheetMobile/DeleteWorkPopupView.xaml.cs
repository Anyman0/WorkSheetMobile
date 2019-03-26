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
	public partial class DeleteWorkPopupView 
	{
        public string work;
		public DeleteWorkPopupView (string WorkID)
		{
			InitializeComponent ();
            work = WorkID;
		}

        private async void DeleteWorkButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                WorkModel data = new WorkModel()
                {
                    Operation = "Delete",
                    WorkID = int.Parse(work)
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
                    await DisplayAlert("Work deleted", "Work has been successfully deleted.", "Close");
                    await PopupNavigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Delete failed!", "Sorry, work has NOT been deleted..", "Close");
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
                string json = await client.GetStringAsync("/api/work?workName=" + work);
                WorkModel chosenWorkModel = JsonConvert.DeserializeObject<WorkModel>(json);
                WorkTitleLabel.Text = chosenWorkModel.WorkTitle;
                WorkDescriptionLabel.Text = chosenWorkModel.Description;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetType().Name + ": " + ex.Message;
                WorkTitleLabel.Text = errorMessage;
            }
        }
    }
}