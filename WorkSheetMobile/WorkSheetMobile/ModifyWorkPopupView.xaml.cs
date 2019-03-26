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
	public partial class ModifyWorkPopupView
	{
        public string work;
		public ModifyWorkPopupView (string WorkID)
		{
			InitializeComponent ();
            work = WorkID;
                        
		}

        private void DeadLinePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TitleEntry.Text))
                ModifyWorkButton.IsEnabled = true;
            else if (string.IsNullOrEmpty(TitleEntry.Text))
                TitleEntry.IsEnabled = false;
        }

        private async void ModifyWorkButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                WorkModel data = new WorkModel()
                {
                    Operation = "Modify",
                    WorkTitle = TitleEntry.Text,
                    Description = DescriptionEntry.Text,
                    Deadline = DeadLinePicker.Date,
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
                string json = await client.GetStringAsync("/api/work?workName=" + work);
                WorkModel chosenWorkModel = JsonConvert.DeserializeObject<WorkModel>(json);
                TitleEntry.Text = chosenWorkModel.WorkTitle;
                DescriptionEntry.Text = chosenWorkModel.Description;
                DeadLinePicker.Date = chosenWorkModel.Deadline;               
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetType().Name + ": " + ex.Message;
                TitleEntry.Text = errorMessage;
            }
            
        }
    }
}