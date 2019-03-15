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
	public partial class CustomerList : ContentPage
	{
		public CustomerList ()
		{
			InitializeComponent ();
            customerList.ItemSelected += CustomerList_ItemSelected;
		}

        private void CustomerList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ModifyCustomerButton.IsEnabled = true;
            DeleteCustomerButton.IsEnabled = true;
        }

        private async void NewCustomerButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new NewCustomerPopupView());
        }

        private async void ModifyCustomerButton_Clicked(object sender, EventArgs e)
        {
            string CustID = customerList.SelectedItem?.ToString();
            await PopupNavigation.PushAsync(new ModifyCustomerPopupView(CustID));
        }

        private async void DeleteCustomerButton_Clicked(object sender, EventArgs e)
        {
            string CustID = customerList.SelectedItem?.ToString();
            await PopupNavigation.PushAsync(new DeleteCustomerPopupView(CustID));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
            string json = await client.GetStringAsync("/api/customer");
            string[] customers = JsonConvert.DeserializeObject<string[]>(json);
            customerList.ItemsSource = customers;
        }
    }
}