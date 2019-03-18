﻿using Newtonsoft.Json;
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

            string logData = UserNameEntry.Text + " " + PasswordEntry.Text;

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
                string json = await client.GetStringAsync("/api/login?LogData=" + logData);
                bool state = JsonConvert.DeserializeObject<bool>(json);

                if (state)
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