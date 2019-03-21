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

            //Toolbaritems created here
            var AddNewCustomer = new ToolbarItem()
            {
                Text = "New",
            };
            AddNewCustomer.Clicked += AddNewCustomer_Clicked;
            ToolbarItems.Add(AddNewCustomer);

            var ModifyCustomer = new ToolbarItem()
            {
                Text = "Modify"
            };
            ToolbarItems.Add(ModifyCustomer);
            ModifyCustomer.Clicked += ModifyCustomer_Clicked;

            var DeleteCustomer = new ToolbarItem()
            {
                Text = "Delete",
            };
            ToolbarItems.Add(DeleteCustomer);
            DeleteCustomer.Clicked += DeleteCustomer_Clicked;
        }

        private async void DeleteCustomer_Clicked(object sender, EventArgs e)
        {
            string CustID = customerList.SelectedItem?.ToString();
            if (CustID == null)
            {
                await DisplayAlert("Whoopsie", "Choose a customer first.", "OK");
            }
            else
            {
                await PopupNavigation.PushAsync(new DeleteCustomerPopupView(CustID));
            }
        }

        private async void ModifyCustomer_Clicked(object sender, EventArgs e)
        {
            string CustID = customerList.SelectedItem?.ToString();
            if (CustID == null)
            {
                await DisplayAlert("Whoopsie", "Choose a customer first.", "OK");
            }
            else
            {
                await PopupNavigation.PushAsync(new ModifyCustomerPopupView(CustID));
            }
        }

        private async void AddNewCustomer_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new NewCustomerPopupView());
        }

        private void CustomerList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            
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