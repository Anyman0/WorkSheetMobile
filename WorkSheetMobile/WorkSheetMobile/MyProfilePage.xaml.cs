using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
	public partial class MyProfilePage : ContentPage
	{
        string MyName;
        byte[] image;
		public MyProfilePage (string Name)
		{
			InitializeComponent ();
            MyName = Name;
		}

        private async void SaveChangesButton_Clicked(object sender, EventArgs e)
        {
            WorkModel data = new WorkModel();
            try
            {
                if (image != null)
                {
                    data = new WorkModel()
                    {
                        Operation = "Save",
                        Picture = image,
                        FirstName = FirstNameEntry.Text,
                        LastName = LastNameEntry.Text,
                        PhoneNumber = int.Parse(PhoheNumberEntry.Text),
                        Email = EmailEntry.Text
                    };
                }
                else
                {
                    data = new WorkModel()
                    {
                        Operation = "Save",
                        FirstName = FirstNameEntry.Text,
                        LastName = LastNameEntry.Text,
                        PhoneNumber = int.Parse(PhoheNumberEntry.Text),
                        Email = EmailEntry.Text
                    };
                }

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
                string input = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");

                HttpResponseMessage message = await client.PostAsync("/api/login", content);
                string reply = await message.Content.ReadAsStringAsync();
                bool success = JsonConvert.DeserializeObject<bool>(reply);

                if (success)
                {
                    await DisplayAlert("Saved!", "Your changes have been saved!", "Close");                    
                }
                else
                {
                    await DisplayAlert("Save Failed", "Sorry, could not save changes..", "Close");
                }

            }
            catch
            {
                await DisplayAlert("Whoopsie", "Could not get data from database..", "OK");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
            string json = await client.GetStringAsync("/api/login?userName=" + MyName);
            WorkModel ProfileInfo = JsonConvert.DeserializeObject<WorkModel>(json);

            if (ProfileInfo.Picture != null && image == null)
            {
                byte[] photo = ProfileInfo.Picture;
                Stream stream = new MemoryStream(photo);
                ImageSource img = ImageSource.FromStream(() => stream);
                MyPicture.Source = img;
            }

            

            FirstNameEntry.Text = ProfileInfo.FirstName;
            LastNameEntry.Text = ProfileInfo.LastName;
            PhoheNumberEntry.Text = ProfileInfo.PhoneNumber.ToString();
            EmailEntry.Text = ProfileInfo.Email;
            MyUserName.Text = "Username:  " + ProfileInfo.UserName;
        }

        private async void TakePictureButton_Clicked(object sender, EventArgs e)
        {
            var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });

            if (photo != null)
                MyPicture.Source = ImageSource.FromStream(() => { return photo.GetStream(); });

            Stream s = photo.GetStream();

            if (s.Length > int.MaxValue)
            {
                throw new Exception("This stream is larger than the conversion algorithm can currently handle.");
            }

            using (var br = new BinaryReader(s))
            {
                image = br.ReadBytes((int)s.Length);
            }
        }

        private void MyPWEntry_Focused(object sender, FocusEventArgs e)
        {
            MyPWEntry.Placeholder = "Old password";
            NewPWEntry.IsVisible = true;
            NewPWSaveButton.IsEnabled = true;
        }

        private async void NewPWSaveButton_Clicked(object sender, EventArgs e)
        {
            string[] logDataParts = MyUserName.Text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string usrname = logDataParts[1];
            string confirm = usrname + " " + MyPWEntry.Text;
            WorkModel pw = new WorkModel();

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
                string json = await client.GetStringAsync("/api/login?LogData=" + confirm);
                bool state = JsonConvert.DeserializeObject<bool>(json);

                if (state && NewPWEntry.Text != null)
                {
                    SHA512 sha512 = SHA512.Create();
                    byte[] bytes = Encoding.UTF8.GetBytes(NewPWEntry.Text);
                    byte[] hash = sha512.ComputeHash(bytes);

                    pw = new WorkModel()
                    {
                        Operation = "SavePW",
                        FirstName = FirstNameEntry.Text,
                        LastName = LastNameEntry.Text,
                        Password = hash
                    };

                    HttpClient cli = new HttpClient();
                    cli.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
                    string input = JsonConvert.SerializeObject(pw);
                    StringContent content = new StringContent(input, Encoding.UTF8, "application/json");

                    HttpResponseMessage message = await cli.PostAsync("/api/login", content);
                    string reply = await message.Content.ReadAsStringAsync();
                    bool success = JsonConvert.DeserializeObject<bool>(reply);

                    if (success)
                    {
                        await DisplayAlert("Saved!", "Your changes have been saved!", "Close");
                    }
                    else
                    {
                        await DisplayAlert("Save Failed", "Sorry, could not save changes..", "Close");
                    }
                }
                else
                {
                    await DisplayAlert("Whoops!", "Seems like your old password is incorrect...", "OK");
                }
            }
            catch
            {
                await DisplayAlert("Whoops!", "Couldnt retrieve data from database...", "OK");
            }
        }
    }
}