using Dashboard2.Model.Domain;
using Dashboard2.Model.Infrastructure.DataAccessLayer.ViasatApiPhysicalLayer;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Dashboard2.ViewModel
{
    class StartPageViewModel : IPage
    {

        //===================================================
        //               PROPERTIES AND FIELDS
        //===================================================
        public ObservableCollection<Car>? AllCarsList { get; set; }
       // public ObservableCollection<Car>? NearestTechnicalInspectionDateCarsList{ get; set; }
        public ObservableCollection<SimpleCarObjectForStartPage> NearestTechnicalInspectionDateCarsList { get; set; }
        public ObservableCollection<SimpleCarObjectForStartPage> LastMonthMileageForAllCars { get; set; }

        private ViasatDbContext ViasatApiDbContext;

        private string _pageTitle;       
        public string PageTitle
        {
            get { return _pageTitle; }
            set { _pageTitle = value; OnPropertyChanged("PageTitle"); }
        }

        private int _allCarsCount;
        public int AllCarsCount
        {
            get { return this._allCarsCount; }
            set { this._allCarsCount = value;
                if (_allCarsCount%10 > 1 && _allCarsCount%10 < 5)
                    AllCarsString ="auta";
                else
                    AllCarsString = "aut"; 
                OnPropertyChanged("AllCarscount"); }
        }

        private int _allCarsWithGpsCount;
        public int AllCarsWithGpsCount
        {
            get { return _allCarsWithGpsCount; }

            set { _allCarsWithGpsCount = value;

                if ((value%10) > 1 && (value%10) < 5)
                {
                    Console.WriteLine($"allgpscount: {AllCarsWithGpsCount}");
                    AllGpsCarsString = "auta";
                }                    
                else
                {Console.WriteLine($"allgpscount: {AllCarsWithGpsCount}");
                AllGpsCarsString = "aut";

                }
                    
                OnPropertyChanged("AllCarsWithGpsCount"); }
        }

        private int _allActiveDriverCount;
        public int AllActiveDriverCount
        {
            get { return _allActiveDriverCount; }
            set { _allActiveDriverCount = value;

                    if ((value % 10) > 1 && (value % 10) < 5)
                    {                      
                        DriverString = "osoby";
                    }
                    else
                    {                      
                        DriverString = "osób";
                    }

                    OnPropertyChanged("AllActiveDriverCount"); 
            }
        }

        private string _driverString;
        public string DriverString
        {
            get { return _driverString; }
            set { _driverString = value; OnPropertyChanged("DriverString"); }
        }
         
        private string _allCarsString;
        public string AllCarsString
        {
            get { return _allCarsString; }
            set { _allCarsString = value; OnPropertyChanged("AllCarsString"); }
        }

         private string _allGpsCarsString;
        public string AllGpsCarsString
        {
            get { return _allGpsCarsString; }
            set { _allGpsCarsString = value; OnPropertyChanged("AllGpsCarsString"); }
        }
        private string _lastMonthName;
        public string LastMonthName { get { return _lastMonthName; } set { _lastMonthName = value; OnPropertyChanged("LastMonthName"); } }



        //===================================================
        //               CONSTRUCTOR
        //===================================================
        public StartPageViewModel(ViasatDbContext viasatDbContext, ObservableCollection<Car> AllCarsListFromDb)
        {
            // MessageBox.Show("jestem w konstruktorze startpageVM");
            this.PageTitle = "Start";           
                     
            if(AllCarsListFromDb!=null && AllCarsListFromDb.Count>0 && viasatDbContext != null)
            {
                this.AllCarsList = AllCarsListFromDb;
                this.ViasatApiDbContext = viasatDbContext;

               Task Task3 = Task.Run(() => SetAllCountsOfValues());

                Task Task1 = Task.Run(() => SetTheNearestTechnicalInspectionDateCarsListOnStartPage());

                Task Task2 = Task.Run(() => SetLastMonthMileageForAllCarsTableOnStartPage());
                Task.WaitAll(Task1, Task2, Task3);
              //  MessageBox.Show("koniec StartPageViewModel");
            }
            else
            {
                this.AllCarsCount = 0;
                this.AllActiveDriverCount = 0;
                this.AllCarsWithGpsCount = 0;                
            }

         //  MessageBox.Show($"konstruktor StartPageViewModel;\n pierwszy obiekt: {this.LastMonthMileageForAllCars[0].RegNum} - {this.LastMonthMileageForAllCars[0].MileageOfKilometers}");
        }

        public StartPageViewModel()
        {
            this.PageTitle = "StartPage!";
        }



        //===================================================
        //               METHODS
        //===================================================
        private void SetAllCountsOfValues()
        {
            Task task1 = Task.Run(() =>
            {
                int y = AllCarsList.Where(x => x.Akt != false).Count();
                for (int i = 0; i < y+1; i++)
                {
                    this.AllCarsCount = i;
                    Thread.Sleep(20);
                }
            });

            Task task2 = Task.Run(() =>
            {
                int z = AllCarsList.Where(x => x.GpsDeviceId != null && x.GpsDeviceId != "").Count();
                for (int i = 0; i < z + 1; i++)
                {
                    this.AllCarsWithGpsCount = i;
                    Thread.Sleep(30);
                }
            });

            Task task3 = Task.Run(() =>
            {
                int a = AllCarsList.Count(x => x.Driver != null && x.Driver != "" && x.Akt == true);
                for (int i = 0; i < a + 1; i++)
                {
                    this.AllActiveDriverCount = i;
                    Thread.Sleep(40);
                }
            });
             
            DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1);            
            this.LastMonthName = $"({dateTime.ToString("MMMMMMMMMMMM")})";

        }
        private void SetTheNearestTechnicalInspectionDateCarsListOnStartPage()
        {
            List<Car> TempList = AllCarsList.Where(x => x.TechnicalInspectionDate != null && x.Akt == true && DateOnly.ParseExact(x.TechnicalInspectionDate, "yyyy.MM.dd") > DateOnly.FromDateTime(DateTime.Today))
                                           .OrderBy(x => (x.TechnicalInspectionDate, "yyyy.MM.dd")).ToList();

            this.NearestTechnicalInspectionDateCarsList = new ObservableCollection<SimpleCarObjectForStartPage>();
            foreach (Car car in TempList)
            {
                if (car.Driver != null && car.Driver != "")
                {
                    this.NearestTechnicalInspectionDateCarsList.Add(new SimpleCarObjectForStartPage(car.RegNum, car.Driver, car.TechnicalInspectionDate));
                }
                else
                {
                    this.NearestTechnicalInspectionDateCarsList.Add(new SimpleCarObjectForStartPage(car.RegNum, car.BranchName, car.TechnicalInspectionDate));
                }

            }
        }
        private void SetLastMonthMileageForAllCarsTableOnStartPage()
        {
         
            int PresentYear = DateTime.Now.Year;
            string DateFrom = "";
            string DateTo = "";
            if (DateTime.Now.Month-1<10)
            {
                DateFrom = $"{PresentYear}-0{DateTime.Now.Month - 1}-01";
                DateTo = $"{PresentYear}-0{DateTime.Now.Month - 1}-{DateTime.DaysInMonth(PresentYear, DateTime.Now.Month - 1)}";
            }
            else
            {
                DateFrom = $"{PresentYear}-{DateTime.Now.Month - 1}-01";
                DateTo = $"{PresentYear}-{DateTime.Now.Month - 1}-{DateTime.DaysInMonth(PresentYear, DateTime.Now.Month - 1)}";
            }

           // MessageBox.Show($"StartViewModel;\ndatefrom: {DateFrom};  dateto: {DateTo}");
            // MessageBox.Show($"datefrom: {DateFrom}, dateto: {DateTo}");
            var TempListMileageFromApi = new ObservableCollection<ViasatClientObject>();
            Task Task1 = Task.Run(() => { TempListMileageFromApi = new ObservableCollection<ViasatClientObject>(ViasatApiDbContext.GetDeviceStatisticFlow(DateFrom, DateTo).OrderByDescending(x => int.Parse(x.NumberOfKilometres)).ToList());});
            Task.WaitAll(Task1);

          // TempListMileageFromApi = TempListMileageFromApi.OrderBy(x => int.Parse( x.NumberOfKilometres));
            //MessageBox.Show($"\n lista z API - liczba el: {TempListMileageFromApi.Count}");

            // MessageBox.Show("zakonczylem wywolanie GetDeviceStatisticFlow()");
            this.LastMonthMileageForAllCars = new ObservableCollection<SimpleCarObjectForStartPage>();
            if(TempListMileageFromApi != null)
            {
                for(int i=0; i< TempListMileageFromApi.Count(); i++)
                {
                   int index = AllCarsList.IndexOf(AllCarsList.Where(x => x.RegNum == TempListMileageFromApi[i].Name).FirstOrDefault());
               
                    if(index >= 0)
                    {
                        if (AllCarsList[index].Driver != null && AllCarsList[index].Driver != "")
                        {
                            this.LastMonthMileageForAllCars.Add(new SimpleCarObjectForStartPage(TempListMileageFromApi[i].Name, AllCarsList[index].Driver, null, TempListMileageFromApi[i].NumberOfKilometres));
                        }
                        else
                        {
                            this.LastMonthMileageForAllCars.Add(new SimpleCarObjectForStartPage(TempListMileageFromApi[i].Name, AllCarsList[index].BranchName, null, TempListMileageFromApi[i].NumberOfKilometres));

                        }
                    }
                    else
                    {
                        this.LastMonthMileageForAllCars.Add(new SimpleCarObjectForStartPage(TempListMileageFromApi[i].Name, "brak info", null, TempListMileageFromApi[i].NumberOfKilometres));
                    }
                  
                }
            }
            else
            {
                MessageBox.Show("Lista pobranych przebiegów dla strony startowej jest pusta.");
            }
          
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
