using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using WorkSheetMobile.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkSheetMobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NewWorkPopupView 
	{
		public NewWorkPopupView ()
		{
			InitializeComponent ();
		}

        public async void SaveWorkButton_Clicked(object sender, EventArgs e)
        {
            
            try
            {
                WorkModel data = new WorkModel()
                {
                    Operation = "Save",
                    CustomerName = CustomerEntry.SelectedItem.ToString(),
                    WorkTitle = TitleEntry.Text,
                    Description = DescriptionEntry.Text,
                    Deadline = DeadLinePicker.Date,
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
                    await DisplayAlert("Saved!", "A new work has been added!", "Close");                                     
                    await PopupNavigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Save Failed", "Sorry, could not save work..", "Close");
                }

            }
            catch
            {
                await DisplayAlert("Save Failed!", "Sorry, couldn't get any data from database..", "OK");
            }
        }

        private void DeadLinePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TitleEntry.Text))
                SaveWorkButton.IsEnabled = true;
            else if (string.IsNullOrEmpty(TitleEntry.Text))
                TitleEntry.IsEnabled = false;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
            string json = await client.GetStringAsync("/api/customer");
            string[] customers = JsonConvert.DeserializeObject<string[]>(json);
            CustomerEntry.ItemsSource = customers;
        }
    }
}