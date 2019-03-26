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
	public partial class TSList : ContentPage
	{
        ListView lista = new ListView();
        Button searchButton = new Button();       
        Picker EntityPicker = new Picker();
		public TSList ()
		{
			InitializeComponent ();


            searchButton.Text = "Search";
            searchButton.Clicked += SearchButton_Clicked;

            var detailToolBarItem = new ToolbarItem()
            {
                Text = "Details"
            };
            ToolbarItems.Add(detailToolBarItem);
            detailToolBarItem.Clicked += DetailToolBarItem_Clicked;

            EntityPicker.Title = "Choose";

            var page = new StackLayout();
            page.Children.Add(lista);
            page.Children.Add(EntityPicker);
            page.Children.Add(searchButton);
            Content = page;          
		}

        private async void DetailToolBarItem_Clicked(object sender, EventArgs e)
        {
            string chosen = lista.SelectedItem?.ToString();           
            await PopupNavigation.PushAsync(new SheetDetailPopupView(chosen));
        }

        private async void SearchButton_Clicked(object sender, EventArgs e)
        {
            string Entity = EntityPicker.SelectedItem?.ToString();

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
                string json = await client.GetStringAsync("/api/timesheet?Entity=" + Entity);
                List<string> chosenList = JsonConvert.DeserializeObject<List<string>>(json);
                lista.ItemsSource = chosenList;
            }
            catch
            {
                await DisplayAlert("Whoops!", "Sorry, couldnt get any data... ", "OK");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
            string json = await client.GetStringAsync("/api/timesheet?tsID=2");
            List<string> sheetList = JsonConvert.DeserializeObject<List<string>>(json);
            lista.ItemsSource = sheetList;

            HttpClient cli = new HttpClient();
            client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
            string jsonn = await client.GetStringAsync("/api/timesheet?picker=1");
            List<string[]> pickerDataModel = JsonConvert.DeserializeObject<List<string[]>>(jsonn);
            List<string> picks = new List<string>();
            foreach (var item in pickerDataModel)
            {
                foreach (var pick in item)
                {
                    picks.Add(pick);
                }
            }
            EntityPicker.ItemsSource = picks;

        }
	}
}