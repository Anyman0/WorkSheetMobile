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

            //Toolbaritems created here
            var AddNewContractor = new ToolbarItem()
            {
                Text = "New",
            };
            AddNewContractor.Clicked += AddNewContractor_Clicked;
            ToolbarItems.Add(AddNewContractor);

            var ModifyContractor = new ToolbarItem()
            {
                Text = "Modify"
            };
            ToolbarItems.Add(ModifyContractor);
            ModifyContractor.Clicked += ModifyContractor_Clicked;

            var DeleteContractor = new ToolbarItem()
            {
                Text = "Delete",
            };
            ToolbarItems.Add(DeleteContractor);
            DeleteContractor.Clicked += DeleteContractor_Clicked;
        }

        private async void DeleteContractor_Clicked(object sender, EventArgs e)
        {
            string ContractorId = contractorList.SelectedItem?.ToString();
            if (ContractorId == null)
            {
                await DisplayAlert("Whoopsie", "Choose a contractor first.", "OK");
            }
            else
            {
                await PopupNavigation.PushAsync(new DeleteContractorPopupView(ContractorId));
            }
        }

        private async void ModifyContractor_Clicked(object sender, EventArgs e)
        {
            string ContractorId = contractorList.SelectedItem?.ToString();
            if (ContractorId == null)
            {
                await DisplayAlert("Whoopsie", "Choose a contractor first.", "OK");
            }
            else
            {
                await PopupNavigation.PushAsync(new ModifyContractorPopupView(ContractorId));
            }
        }

        private async void AddNewContractor_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new NewContractorPopupView());
        }

        private void ContractorList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            
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