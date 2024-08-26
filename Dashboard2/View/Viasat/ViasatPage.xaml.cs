﻿using Dashboard2.Model.Domain;
using Dashboard2.Model.Infrastructure.DataAccessLayer.ViasatApiPhysicalLayer;
using Dashboard2.Model.Infrastructure.Repositories.ViasatApi;
using Dashboard2.ViewModel;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace Dashboard2.View.Viasat
{
    public partial class ViasatPage : Page
    {


        //  public ViasatViewModel _ViasatViewModel { get; set; }
        //  ViasatDbContext _viasatDbContext { get; set; }

      //  ViasatViewModel x; 

        //=========================================================================================================================
        //                   CONSTRUCTOR
        //=========================================================================================================================        
        public ViasatPage()
        {             
            InitializeComponent();      
        }


        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {        
            var CurrentDataContext = (ViasatViewModel)this.DataContext;
            Panel2.Controls.Add(CurrentDataContext.GmapObject.MapObject); 
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        
        }


        /*
        public ViasatPage(ViasatViewModel viasatViewModel)
        {
            InitializeComponent();
            /*
            this._ViasatViewModel = viasatViewModel;
            this._ViasatViewModel.przypisz();
            this.DataContext = _ViasatViewModel;
            
        }

   
          //=========================================================================================================================
          //                   API VIASAT Main Methods
          //=========================================================================================================================        

          public async Task GetClientObjectTest()
          {
              //Task < ObservableCollection < ViasatClientObject >> taskresult =  this._viasatDbContext.GetClientObjects();

             // await Task.Run(() => { this.MojaListaAut =  taskresult.Result;
                 // MessageBox.Show("liczba elementow w liscie: " + this.MojaListaAut.Count);
              });
          }


          public async Task GetLocationsExNCTest(ViasatClientObject viasatClientObject2)
          {
              //CheckPoint CheckPoint = new CheckPoint("-21443", "CT1001V");
              //CheckPoint CheckPoint = new CheckPoint("-28660", "CT020AJ");

              DateTime StartDate = new DateTime(2024, 06, 28, 7, 30, 0);
              DateTime EndDate = new DateTime(2024, 06, 28, 16, 30, 0);
              // ViasatClientObject viasatClientObject = new ViasatClientObject("-28660", "CT020AJ");
              ViasatClientObject viasatClientObject = viasatClientObject2;

              if (this.ListOfCheckPointsForSummaryOfResult != null || this.ListOfCheckPointsForSummaryOfResult.Count > 0) { this.ListOfCheckPointsForSummaryOfResult.Clear(); }
               this.ListCheckpointOwn.UpdateLayout();

              Task.WaitAll(Task.Run(async () => {
                  this.ListOfCheckPointsForSummaryOfResult
                  = await this._viasatDbContext.GetLocationsExNC(viasatClientObject, StartDate, EndDate);
              }));


              if (this.ListOfCheckPointsForSummaryOfResult != null || this.ListOfCheckPointsForSummaryOfResult.Count > 0)
              {
                  MessageBox.Show("wywoluje FinalTripTimeSummary");
                  this.ListOfSummaryResultForSelectedCar = FinalTripTimeSummary(ListOfCheckPointsForSummaryOfResult);

                  this.ListCheckpointOwn.ItemsSource = this.ListOfSummaryResultForSelectedCar;

                  MessageBox.Show("zakonczylem FinalTripTimeSummary");
              }
              else
              {
                  MessageBox.Show("lista jest pusta lub null");
              }
              //  FinalTripTimeSummary(ListOfCheckPointsForSummaryOfResult, SelectedCar);
          }


          //=========================================================================================================================
          //                 Helper Fun for  API VIASAT Main Methods
          //=========================================================================================================================        
          public ObservableCollection<ObservableCollection<string>> FinalTripTimeSummary(List<CheckPoint> ListOfCheckPointsForSummaryOfResult)
          {
              ObservableCollection<ObservableCollection<string>> SummaryList = new ObservableCollection<ObservableCollection<string>>();

              for (int i = 0; i < ListOfCheckPointsForSummaryOfResult.Count - 1; i++)
              {
                  if (ViasatDbContext.CheckIsAddressSameAsPrevious(ListOfCheckPointsForSummaryOfResult[i].LocalizationDescription, ListOfCheckPointsForSummaryOfResult[i + 1].LocalizationDescription))
                  {
                      ObservableCollection<string> CheckpointInfo = new ObservableCollection<string>();

                      CheckpointInfo.Add($"{ListOfCheckPointsForSummaryOfResult[i].DateTimeReading} - {ListOfCheckPointsForSummaryOfResult[i + 1].DateTimeReading}");
                      CheckpointInfo.Add("POSTÓJ");
                      CheckpointInfo.Add("0");
                      CheckpointInfo.Add($"{CorrectTheAddress(ListOfCheckPointsForSummaryOfResult[i].LocalizationDescription)}");

                      SummaryList.Add(CheckpointInfo);
                  }
                  else if (!ViasatDbContext.CheckIsAddressSameAsPrevious(ListOfCheckPointsForSummaryOfResult[i].LocalizationDescription, ListOfCheckPointsForSummaryOfResult[i + 1].LocalizationDescription))
                  {
                      ObservableCollection<string> CheckpointInfo = new ObservableCollection<string>();

                      CheckpointInfo.Add($"{ListOfCheckPointsForSummaryOfResult[i].DateTimeReading} - {ListOfCheckPointsForSummaryOfResult[i + 1].DateTimeReading}");
                      CheckpointInfo.Add("PRZEJAZD");

                      if (ListOfCheckPointsForSummaryOfResult[i].Odometer != 0)
                      {
                          // message += $"{ListOfCheckPointsForSummaryOfResult[i + 1].Odometer - ListOfCheckPointsForSummaryOfResult[i].Odometer}\t";
                          CheckpointInfo.Add($"{ListOfCheckPointsForSummaryOfResult[i + 1].Odometer - ListOfCheckPointsForSummaryOfResult[i].Odometer}");
                      }
                      else
                      {
                          CheckpointInfo.Add("0");
                      }
                      CheckpointInfo.Add($"{CorrectTheAddress(ListOfCheckPointsForSummaryOfResult[i].LocalizationDescription)}  <->  {CorrectTheAddress(ListOfCheckPointsForSummaryOfResult[i + 1].LocalizationDescription)}");
                      SummaryList.Add(CheckpointInfo);
                  }

              }

              return SummaryList;
          }


          public static string CorrectTheAddress(string address)
          {
              address = address.Replace(" / ", ",");
              return address;
          }






  */





    }
}
