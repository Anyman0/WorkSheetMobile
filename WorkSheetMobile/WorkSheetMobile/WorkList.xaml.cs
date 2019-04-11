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

            //Toolbaritems created here
            var AddNewWork = new ToolbarItem()
            {
                Text = "New",
            };
            AddNewWork.Clicked += AddNewWork_Clicked;
            ToolbarItems.Add(AddNewWork);

            var ModifyWork = new ToolbarItem()
            {
                Text = "Modify",                
            };
            ToolbarItems.Add(ModifyWork);
            ModifyWork.Clicked += ModifyWork_Clicked;

            var DeleteWork = new ToolbarItem()
            {
                Text = "Delete",
            };
            ToolbarItems.Add(DeleteWork);
            DeleteWork.Clicked += DeleteWork_Clicked;

        }

        private async void DeleteWork_Clicked(object sender, EventArgs e)
        {
            string WorkID = workList.SelectedItem?.ToString();

            if (WorkID == null)
            {
                await DisplayAlert("Whoopsie", "Choose a work first.", "OK");
            }
            else
            {
                string[] work = WorkID.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                int works = work.Count();
                string workid = work[works - 2];
                await PopupNavigation.PushAsync(new DeleteWorkPopupView(workid));
            }
           
        }

        private async void ModifyWork_Clicked(object sender, EventArgs e)
        {
            string WorkID = workList.SelectedItem?.ToString();
                                            
            if (WorkID == null)
            {
                await DisplayAlert("Whoopsie", "Choose a work first.", "OK");
            }

            else
            {
                string[] work = WorkID.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                int works = work.Count();
                string workid = work[works - 2];
                await PopupNavigation.PushAsync(new ModifyWorkPopupView(workid));
            }           
            
        }

        private async void AddNewWork_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new NewWorkPopupView());
        }

        // Enabling buttons
        private void WorkList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {            
            
            AssignWorkButton.IsEnabled = true;
            MarkCompleteButton.IsEnabled = true;
        }


        // Assign work      
        private async void AssignWorkButton_Clicked(object sender, EventArgs e)
        {
            string WorkID = workList.SelectedItem?.ToString();

            string[] work = WorkID.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            int works = work.Count();
            string workid = work[works - 2];

            await PopupNavigation.PushAsync(new AssignWorkPopupView(workid));
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
                AssignWorkButton.IsVisible = false;
                
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
                AssignWorkButton.IsVisible = true;
            }
            
        }
        // Markcompletebutton methods
        private async void MarkCompleteButton_Clicked(object sender, EventArgs e)
        {
            string WorkID = workList.SelectedItem?.ToString();

            string[] work = WorkID.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            int works = work.Count();
            string workid = work[works - 2];

            try
            {
                WorkModel data = new WorkModel()
                {
                    Operation = "MarkComplete",
                    WorkID = int.Parse(workid)                 
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

        protected override async void OnAppearing()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
            string json = await client.GetStringAsync("/api/work");
            string[] workTitles = JsonConvert.DeserializeObject<string[]>(json);
            workList.ItemsSource = workTitles;
            base.OnAppearing();
        }
    }
}