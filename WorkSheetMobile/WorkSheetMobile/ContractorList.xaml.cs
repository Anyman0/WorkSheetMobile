using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkSheetMobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ContractorList : ContentPage
	{
		public ContractorList ()
		{
			InitializeComponent ();
            contractorList.ItemSelected += ContractorList_ItemSelected;
		}

        private void ContractorList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ModifyContractorButton.IsEnabled = true;
            DeleteContractorButton.IsEnabled = true;
        }

        private async void NewContractorButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new NewContractorPopupView());
        }

        private async void ModifyContractorButton_Clicked(object sender, EventArgs e)
        {
            string ContractorId = contractorList.SelectedItem?.ToString();
            await PopupNavigation.PushAsync(new ModifyContractorPopupView(ContractorId));
        }

        private async void DeleteContractorButton_Clicked(object sender, EventArgs e)
        {
            string ContractorId = contractorList.SelectedItem?.ToString();
            await PopupNavigation.PushAsync(new DeleteContractorPopupView(ContractorId));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
            string json = await client.GetStringAsync("/api/contractor");
            string[] contractors = JsonConvert.DeserializeObject<string[]>(json);
            contractorList.ItemsSource = contractors;
        }
    }
}