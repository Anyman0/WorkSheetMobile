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
	public partial class TimesheetList : ContentPage
	{
        public string chosenEntity;
        public WorkModel chosenData;       
		public TimesheetList ()
		{
			InitializeComponent ();
            
		}

        private async void EmployeeInfoButton_Clicked(object sender, EventArgs e)
        {

            chosenEntity = ChooseEmployeePicker.SelectedItem?.ToString();

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
                string json = await client.GetStringAsync("/api/timesheet?chosenEntity=" + chosenEntity);
                WorkModel chosenWorkModel = JsonConvert.DeserializeObject<WorkModel>(json);
                chosenData = chosenWorkModel;

                ChosenEntityLabel.Text = chosenData.FirstName;
                ChosenEntityLabel1.Text = chosenData.WorkTitle;
                ChosenEntityLabel2.Text = chosenData.CountedHours.ToString();
            }
            catch
            {
                await DisplayAlert("Failed!", "Sorry, this employee doesnt have any completed work..", "OK");
            }
        }

        private async void SearchWorkDataButton_Clicked(object sender, EventArgs e)
        {

            chosenEntity = ChooseWorkPicker.SelectedItem?.ToString();
            
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
                string json = await client.GetStringAsync("/api/timesheet?chosenEntity=" + chosenEntity);
                WorkModel chosenWorkModel = JsonConvert.DeserializeObject<WorkModel>(json);
                chosenData = chosenWorkModel;

                ChosenEntityLabel.Text = chosenData.WorkTitle;
                ChosenEntityLabel1.Text = chosenData.StartTime.ToString();
                ChosenEntityLabel2.Text = chosenData.StopTime.ToString();
                ChosenEntityLabel3.Text = chosenData.CountedHours.ToString();
            }
            catch
            {
                await DisplayAlert("Failed!", "Sorry, this work is not yet completed..", "OK");
            }
        }

        private async void SearchContractorDataButton_Clicked(object sender, EventArgs e)
        {
            chosenEntity = ChooseContractorPicker.SelectedItem?.ToString();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
            string json = await client.GetStringAsync("/api/timesheet?chosenEntity=" + chosenEntity);
            string [] chosenWorkModel = JsonConvert.DeserializeObject<string[]>(json);
            testList.ItemsSource = chosenWorkModel;
            
            /*try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
                string json = await client.GetStringAsync("/api/timesheet?chosenEntity=" + chosenEntity);
                WorkModel chosenWorkModel = JsonConvert.DeserializeObject<WorkModel>(json);
                chosenData = chosenWorkModel;
                testList.ItemsSource = chosenData.ContractorPickerData;
                ChosenEntityLabel.Text = chosenData.ContractorName;
                ChosenEntityLabel1.Text = chosenData.WorkTitle;
                ChosenEntityLabel2.Text = chosenData.FirstName;
                ChosenEntityLabel3.Text = chosenData.CountedHours.ToString();
            }
            catch
            {
                await DisplayAlert("Failed!", "Sorry, this contractor's employees doesnt have any completed work..", "OK");
            }*/
        }

        private void SearchButton_Clicked(object sender, EventArgs e)
        {

        }

        protected override async void OnAppearing()
        {       
            
            base.OnAppearing();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://worksheet.azurewebsites.net");
            string json = await client.GetStringAsync("/api/timesheet/1");
            WorkModel pickerDataModel = JsonConvert.DeserializeObject<WorkModel>(json);
            ChooseWorkPicker.ItemsSource = pickerDataModel.WorkPickerData;
            ChooseEmployeePicker.ItemsSource = pickerDataModel.EmployeePickerData;
            ChooseContractorPicker.ItemsSource = pickerDataModel.ContractorPickerData;
        }
    }
}