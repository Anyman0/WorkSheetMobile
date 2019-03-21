using Newtonsoft.Json;
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
	public partial class TSList : ContentPage
	{
        ListView lista = new ListView();       
		public TSList ()
		{
			InitializeComponent ();           
            var page = new StackLayout();
            page.Children.Add(lista);
            Content = page;            
		}
        
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
            string json = await client.GetStringAsync("/api/timesheet?tsID=2");
            List<string> sheetList = JsonConvert.DeserializeObject<List<string>>(json);
            lista.ItemsSource = sheetList;
            
        }
	}
}