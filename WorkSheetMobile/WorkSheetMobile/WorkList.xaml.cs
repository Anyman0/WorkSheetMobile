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
	public partial class WorkList : ContentPage
	{
        
		public WorkList ()
		{
			InitializeComponent ();
           
            workList.ItemSelected += WorkList_ItemSelected;
            
		}

        // Enabling buttons
        private void WorkList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {            
            ModifyWorkButton.IsEnabled = true;
            DeleteWorkButton.IsEnabled = true;
            AssignWorkButton.IsEnabled = true;
            MarkCompleteButton.IsEnabled = true;
        }

        // Button Clicked-Actions
        private async void NewWorkButton_Clicked(object sender, EventArgs e)
        {
             await PopupNavigation.PushAsync(new NewWorkPopupView());           
        }

        private async void ModifyWorkButton_Clicked(object sender, EventArgs e)
        {
            string WorkID = workList.SelectedItem?.ToString();
            await PopupNavigation.PushAsync(new ModifyWorkPopupView(WorkID));
        }

        public async void DeleteWorkButton_Clicked(object sender, EventArgs e)
        {
            string WorkID = workList.SelectedItem?.ToString();
            await PopupNavigation.PushAsync(new DeleteWorkPopupView(WorkID));
        }

        private async void AssignWorkButton_Clicked(object sender, EventArgs e)
        {
            string WorkID = workList.SelectedItem?.ToString();
            await PopupNavigation.PushAsync(new AssignWorkPopupView(WorkID));
        }

        protected override async void OnAppearing()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
            string json = await client.GetStringAsync("/api/work");
            string[] workTitles = JsonConvert.DeserializeObject<string[]>(json);
            workList.ItemsSource = workTitles;
            base.OnAppearing();
        }

        // ViewListButton methods
        private async void ViewListButton_Clicked(object sender, EventArgs e)
        {
            
            if (ViewListButton.Text == "Show works in progress")
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
                string json = await client.GetStringAsync("/api/work/5");
                string[] workTitles = JsonConvert.DeserializeObject<string[]>(json);
                workList.ItemsSource = workTitles;
                ViewListButton.Text = "Show unassigned works";

                MarkCompleteButton.IsVisible = true;

            }
            else if (ViewListButton.Text == "Show unassigned works")
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
                string json = await client.GetStringAsync("/api/work");
                string[] workTitles = JsonConvert.DeserializeObject<string[]>(json);
                workList.ItemsSource = workTitles;
                ViewListButton.Text = "Show works in progress";

                MarkCompleteButton.IsVisible = false;
            }
            
        }

        private async void MarkCompleteButton_Clicked(object sender, EventArgs e)
        {
            string WorkID = workList.SelectedItem?.ToString();            
            try
            {
                WorkModel data = new WorkModel()
                {
                    Operation = "MarkComplete",
                    WorkTitle = WorkID                  
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
                    await DisplayAlert("Saved!", "Work has been set to Completed!", "Close");
                    
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
    }
}