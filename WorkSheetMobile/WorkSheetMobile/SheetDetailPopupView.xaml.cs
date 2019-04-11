using Newtonsoft.Json;
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
	public partial class SheetDetailPopupView
	{
        public string chosenSheet;
		public SheetDetailPopupView (string chosen)
		{
			InitializeComponent ();
            chosenSheet = chosen;
		}

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
            string json = await client.GetStringAsync("/api/timesheet?Details=" + chosenSheet);
            WorkModel sheetData = JsonConvert.DeserializeObject<WorkModel>(json);
            CustomerLabel.Text = sheetData.CustomerName;
            ContractorLabel.Text = sheetData.ContractorName;
            EmployeeLabel.Text = sheetData.FirstName;
            WorkLabel.Text = sheetData.WorkTitle;
            TimeLabel.Text = " " + sheetData.CountedHours.ToString() + " hours.";
            CommentLabel.Text = sheetData.Comments;
        }
	}
}