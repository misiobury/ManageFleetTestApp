using Dashboard2.Model.Domain;
using Dashboard2.Model.Infrastructure.DataAccessLayer.ViasatApiPhysicalLayer;
using Dashboard2.Model.Infrastructure.Repositories.ViasatApi;
using Dashboard2.View.Start;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Xml.Linq;


namespace Dashboard2.ViewModel
{
    public class ViasatViewModel : IPage
    {
        //==============================================================
        //                   PROPERTIES AND FIELDS
        //==============================================================

        //---------------------------------------------------------------
        // MAIN PROPER./FIELDS  SECTION 
        //---------------------------------------------------------------
        private string _pageTitle;
        public string PageTitle
        {
            get { return _pageTitle; }
            set { _pageTitle = value; OnPropertyChanged("PageTitle"); }
        }
        public ViasatDbContext viasatDbContext { get; set; }
        private ObservableCollection<Car> AllCarsFromDbAndApiMainList { get; set; }
        public GmapObject GmapObject { get; set; }


        //---------------------------------------------------------------
        // SELECT CAR  SECTION 
        //---------------------------------------------------------------
        private DTOForGetLocationsForCar _selectedCar;
        public DTOForGetLocationsForCar SelectedCar { get { return _selectedCar; } set { _selectedCar = value; OnPropertyChanged("SelectedCar"); } }
        public ObservableCollection<SimpleCarObjectForViasatPage> ListOfAvailableCars { get; set; }

        private int _listOfAvailableCarsSelectedIndex;
        public int ListOfAvailableCarsSelectedIndex { get { return _listOfAvailableCarsSelectedIndex; } set { _listOfAvailableCarsSelectedIndex = value; OnPropertyChanged("ListOfAvailableCarsSelectedIndex"); } }

        private int _countOfGpsCars;
        public int countOfGpsCars { get { return _countOfGpsCars; } set { _countOfGpsCars = value; OnPropertyChanged("countOfGpsCars"); } }



        //---------------------------------------------------------------
        // CHECKPOINTS PARAM. FORM SECTION  (for download from API)  
        //---------------------------------------------------------------
        public List<string> CarMinParkingTime { get; set; }
        public List<string> HourFromList { get; set; }
        public List<string> MinutesFromList { get; set; }
        public List<string> HourToList { get; set; }
        public List<string> MinutesToList { get; set; }

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                // MessageBox.Show($"'{value}'");
                _selectedDate = value;

                // MessageBox.Show(DateOnly.FromDateTime(_selectedDate).ToString());
                OnPropertyChanged("SelectedDate");
            }
        }

        private int _carMinParkTimeIndex;
        public int CarParkTimeIndex { get { return _carMinParkTimeIndex; } set { _carMinParkTimeIndex = value; OnPropertyChanged("CarMinParkingTime"); } }

        private int _hourFromSelectedIndex;
        public int HourFromSelectedIndex { get { return _hourFromSelectedIndex; } set { _hourFromSelectedIndex = value; OnPropertyChanged("HourFromSelectedIndex"); } }

        private int _minuteFromSelectedIndex;
        public int MinuteFromSelectedIndex { get { return _minuteFromSelectedIndex; } set { _minuteFromSelectedIndex = value; OnPropertyChanged("MinuteFromSelectedIndex"); } }

        private int _hourToSelectedIndex;
        public int HourToSelectedIndex { get { return _hourToSelectedIndex; } set { _hourToSelectedIndex = value; OnPropertyChanged("HourToSelectedIndex"); } }

        private int _minuteToSelectedIndex;
        public int MinuteToSelectedIndex { get { return _minuteToSelectedIndex; } set { _minuteToSelectedIndex = value; OnPropertyChanged("MinuteToSelectedIndex"); } }



        //---------------------------------------------------------------
        // RESULT CHECKPOINTS TABLE SECTION 
        //---------------------------------------------------------------
        private List<CheckPoint> _checkPoints;
        public List<CheckPoint> ListOfCheckPointsForSummaryOfResult { get { return _checkPoints; } set { _checkPoints = value; OnPropertyChanged("ListOfCheckPointsForSummaryOfResult"); } } 

        private ObservableCollection<ObservableCollection<string>> _listOfSummaryResultForSelectedCar;
        public ObservableCollection<ObservableCollection<string>> ListOfSummaryResultForSelectedCar { get { return _listOfSummaryResultForSelectedCar; } set { _listOfSummaryResultForSelectedCar = value; OnPropertyChanged("ListOfSummaryResultForSelectedCar"); } }

        private bool _isCommandProcesing;
        public bool IsCommandProcessing { get { return _isCommandProcesing; } set { _isCommandProcesing = value; OnPropertyChanged("IsCommandProcessing"); } }

        //---------------------------------------------------------------
        // RESULT MAP SECTION 
        //---------------------------------------------------------------
      


        //=====================================================================
        //                   CONSTRUCTOR
        //=====================================================================
        public ViasatViewModel(ViasatDbContext viasatDbContext, ObservableCollection<Car> AllCarsList)
        {
          //  MessageBox.Show("jestem w konstruktorze ViasatVM");
            this.PageTitle = "ViasatPage!";
            this.viasatDbContext = viasatDbContext;
            var list = AllCarsList.OrderBy(x => x.RegNum).ToList();
            this.AllCarsFromDbAndApiMainList = new ObservableCollection<Car>(list);
           this.GmapObject = new GmapObject();
            ListOfCheckPointsForSummaryOfResult = new List<CheckPoint>();


            InitialParameters();
            InitializeTableWithAvailablesCars();

            ShowSummaryForSelectedCar = new RelayCommand(_showSummaryForSelectedCar, CanCommandBeExecuted);

        }

        public ViasatViewModel() { }




        //============================================================
        //                    METHODS
        //============================================================

        //----------------------------------
        //COMMANDS SECTION 
        //----------------------------------
        #region Commands 

        public ICommand? ShowSummaryForSelectedCar { get; set; }

        #endregion

      

        #region Actions for Commands to Execute()
        private async void _showSummaryForSelectedCar(object value)
        {
            this.IsCommandProcessing = true;

            this.ListOfCheckPointsForSummaryOfResult = new List<CheckPoint>();
            this.ListOfSummaryResultForSelectedCar = new ObservableCollection<ObservableCollection<string>>();
            int HourFromTemp = int.Parse(this.HourFromList[HourFromSelectedIndex]);
            int HourToTemp = int.Parse(this.HourToList[this.HourToSelectedIndex]);
            int MinuteFromTemp = int.Parse(this.MinutesFromList[this.MinuteFromSelectedIndex]);
            int MinuteToTemp = int.Parse(this.MinutesToList[this.MinuteToSelectedIndex]);

            if ((HourFromTemp > HourToTemp) || (HourFromTemp==HourToTemp) && (MinuteFromTemp > MinuteToTemp) )
            {
               System.Windows.MessageBox.Show("Czas początkowy jest większy niż czas końcowy,\nproszę popraw zakres czasowy.");
            }
            else
            {
                DateTime dateTimeFrom = new DateTime(DateOnly.FromDateTime(SelectedDate), new TimeOnly(int.Parse(this.HourFromList[HourFromSelectedIndex]), int.Parse(this.MinutesFromList[MinuteFromSelectedIndex]), 0));
                DateTime dateTimeTo = new DateTime(DateOnly.FromDateTime(SelectedDate), new TimeOnly(int.Parse(this.HourToList[HourToSelectedIndex]), int.Parse(this.MinutesToList[MinuteToSelectedIndex]), 0));

                //MessageBox.Show(dateTimeFrom + "\n" + dateTimeTo);
               // MessageBox.Show($"rok: {dateTimeFrom.Year}\nmiesiac: {dateTimeFrom.Month}\ndzien:{dateTimeFrom.Day}\nhour: {dateTimeFrom.Hour}\nminute: {dateTimeFrom.Minute}");
               // MessageBox.Show($"rok: {dateTimeTo.Year}\nmiesiac: {dateTimeTo.Month}\ndzien:{dateTimeTo.Day}\nhour: {dateTimeTo.Hour}\nminute: {dateTimeTo.Minute}");

                if(this.ListOfAvailableCars!=null)
                {
                    this.SelectedCar = new DTOForGetLocationsForCar(this.ListOfAvailableCars[this.ListOfAvailableCarsSelectedIndex].Id, 
                                                                 this.ListOfAvailableCars[this.ListOfAvailableCarsSelectedIndex].RegNum,
                                                                 this.ListOfAvailableCars[this.ListOfAvailableCarsSelectedIndex].Owner,
                                                                 dateTimeFrom,
                                                                 dateTimeTo,
                                                                 int.Parse(this.CarMinParkingTime[this.CarParkTimeIndex])
                 );
                }

               // MessageBox.Show($"{this.SelectedCar.Id}\n{this.SelectedCar.RegNum}\n{this.SelectedCar.Name}\n{this.SelectedCar.DateFrom}\n{this.SelectedCar.DateTo}\n{this.SelectedCar.CarParkTime}");

                await Task.Run( () =>
                {
                  //  MessageBox.Show("zaczynamy");
                     this.ListOfCheckPointsForSummaryOfResult =  this.viasatDbContext.GetLocationsExNC(this.SelectedCar);                
                });
                Task.WaitAll();

                //   System.Windows.MessageBox.Show($"X0: {this.ListOfCheckPointsForSummaryOfResult[0].X}, Y0: {this.ListOfCheckPointsForSummaryOfResult[0].X}");


                await  Task.Run(() =>
                {
                     this.ListOfSummaryResultForSelectedCar =   this.GmapObject.InitializeCheckpointMarkersListAndRoutes(this.ListOfCheckPointsForSummaryOfResult, int.Parse(this.CarMinParkingTime[this.CarParkTimeIndex]));
                
                });
                Task.WaitAll();
                this.IsCommandProcessing = false;
                /*
                 * 
                 * new Action(delegate()
                 * 
                if (this.ListOfCheckPointsForSummaryOfResult != null || this.ListOfCheckPointsForSummaryOfResult.Count > 0)
                {
                  //  MessageBox.Show("wywoluje FinalTripTimeSummary");
                    this.ListOfSummaryResultForSelectedCar = FinalTripTimeSummary(ListOfCheckPointsForSummaryOfResult);
                  //    MessageBox.Show("zakonczylem FinalTripTimeSummary");
                }
                else
                {
                    System.Windows.MessageBox.Show("lista jest pusta lub null");
                }
                ;
                */
                //    this.GmapObject.InitialCheckpointForSelectedCarLayer(this.ListOfCheckPointsForSummaryOfResult);
                //   MessageBox.Show("skonczylem");
            }







            //MessageBox.Show("dziala!");


        }
        #endregion


        #region condition for commands
        private bool CanCommandBeExecuted(object value)
        {
            return true; 
        }

        #endregion





        //----------------------------------
        // OTHERS METHODS
        //----------------------------------
        private void InitialParameters()
        {
            this.countOfGpsCars = this.AllCarsFromDbAndApiMainList.Where(x => x.GpsDeviceId != null).Count();
            this.IsCommandProcessing = false;
            //SET HOURS
            this.HourFromList = new List<string>();
            this.HourToList = new List<string>();
            string hour = "";
            for (int i = 1; i < 25; i++)
            {
                if (i < 10)
                {
                    hour = $"0{i.ToString()}";
                }
                else
                    hour = $"{i.ToString()}";
                this.HourFromList.Add(hour);
                this.HourToList.Add(hour);
            }
            this.HourFromSelectedIndex = 6;
            this.HourToSelectedIndex = 15;


            //SET MINUTES
            this.MinutesFromList = new List<string>();
            this.MinutesToList = new List<string>();
            string minute = "";
            for (int i = 1; i < 60; i++)
            {
                if (i < 10)
                {
                    minute = $"0{i.ToString()}";
                }
                else
                    minute = $"{i.ToString()}";

                this.MinutesFromList.Add(minute);
                this.MinutesToList.Add(minute);
            }
            this.MinuteFromSelectedIndex = 29;
            this.MinuteToSelectedIndex = 29;


            //SET CAR MIN PARKING TIME
            this.CarMinParkingTime = new List<string>();
            for (int i = 5; i < 65; i += 5)
            {
                this.CarMinParkingTime.Add(i.ToString());
            }
            this.CarParkTimeIndex = 2;

            // SET  DATE
            this.SelectedDate = DateTime.Now.Date;
        }


        private void InitializeTableWithAvailablesCars()
        {
            this.ListOfAvailableCars = new ObservableCollection<SimpleCarObjectForViasatPage>();
            int index = 1;
            foreach(var car in this.AllCarsFromDbAndApiMainList.Where(x => x.GpsDeviceId !=null))
            {
                if (car.Driver == null || car.Driver == "")
                    this.ListOfAvailableCars.Add(new SimpleCarObjectForViasatPage(index.ToString(), car.RegNum, car.GpsDeviceId, car.BranchName, null));
                else
                    this.ListOfAvailableCars.Add(new SimpleCarObjectForViasatPage(index.ToString(), car.RegNum, car.GpsDeviceId, car.Driver, null));
                index++;
            }
        }
     


        //-------------------------------------------------
        //  METHODS FOR "RESULT CHECKPOINTS" TABLE SECTION 
        //-------------------------------------------------

      


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

        


        //-------------------------------
        //INotifyPropertyChanged
        //-------------------------------
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion



    }
}
