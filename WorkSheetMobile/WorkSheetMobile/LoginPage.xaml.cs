using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WorkSheetMobile.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkSheetMobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
		public LoginPage ()
		{
			InitializeComponent ();
		}

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {

            
            byte[] logdata = new byte[PasswordEntry.Text.Length];
            byte[] result;
            SHA512 shaM = new SHA512Managed();
            result = shaM.ComputeHash(logdata);

            byte[] HashedPW = result;

            WorkModel logData = new WorkModel()
            {
                UserName = UserNameEntry.Text,
                Password = result             
            };

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
                string input = JsonConvert.SerializeObject(logData);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");

                HttpResponseMessage message = await client.PostAsync("/api/work", content);
                string reply = await message.Content.ReadAsStringAsync();
                bool success = JsonConvert.DeserializeObject<bool>(reply);
                

                if (success)
                {
                    await Navigation.PushAsync(new EmployeeList());
                }
                else
                {
                    await DisplayAlert("Login failed", "Username or password is incorrect..", "OK");
                }
            }
            catch
            {
                await DisplayAlert("Login failed", "Couldnt retrieve data from database...", "OK");
            }

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}