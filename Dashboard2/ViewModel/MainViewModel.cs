using Dashboard2.Model.Domain;
using Dashboard2.Model.Infrastructure.DataAccessLayer.CompanyDbPhysicalLayer;
using Dashboard2.Model.Infrastructure.DataAccessLayer.ViasatApiPhysicalLayer;
using Dashboard2.View.Drivers;
using Dashboard2.View.Fleet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Runtime.ConstrainedExecution;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dashboard2.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        //============================================================
        //                    FIELDS
        //============================================================
        
        //DB CONTEXT
        //-----------------------------------------------------------
        public ViasatDbContext? ViasatApiDbContext { get; set; }
        public CompanyDatabaseDbContext? CompanyDatabaseDbContext { get; set; }

        private ObservableCollection<Car> _allCarsFromDbAndApiMainList;
        public ObservableCollection<Car>? AllCarsFromDbAndApiMainList { get { return _allCarsFromDbAndApiMainList; } set { _allCarsFromDbAndApiMainList = value; OnPropertyChanged("AllCarsFromDbAndApiMainList"); } }
        public ObservableCollection<Car>? AllNonActiveCarsList { get; set; }

        private ObservableCollection<ViasatClientObject>? _carsIdAndRegnumListFromApi;
        public ObservableCollection<ViasatClientObject>? CarsIdAndRegnumListFromApi
        { get { return _carsIdAndRegnumListFromApi; } set { _carsIdAndRegnumListFromApi = value; OnPropertyChanged("CarsIdAndRegnumListFromApi"); } }

        private string _notificationString;
        public string NotificationString { get { return _notificationString; } set { _notificationString = value; OnPropertyChanged("NotificationString"); } }
        private bool _isNotificationActive;
        public bool IsNotificationActive { get { return _isNotificationActive; } set { _isNotificationActive = value; OnPropertyChanged("IsNotificationActive"); } }
        private bool _isAcceptIconActive;
        public bool IsAcceptIconActive { get { return _isAcceptIconActive; } set { _isAcceptIconActive = value; OnPropertyChanged("IsAcceptIconActive"); } }
        private bool _blockControl;


        //Navigate within Pages
        //----------------------------------------------------------
        private Dictionary<PageName, IPage>? Pages { get; set; }
        private PageName _selectedPageName { get; set; }
        public PageName SelectedPageName
        {
            get { return _selectedPageName; }
            set { _selectedPageName = value; OnPropertyChanged("SelectedPageName"); }
        }
        private IPage _selectedPage { get; set; }
        public IPage SelectedPage
        {
            get { return _selectedPage; }
            set { _selectedPage = value; OnPropertyChanged("SelectedPage"); }
        }

        

        //==============================================================
        //                   CONSTRUCTOR
        //==============================================================
        public MainViewModel()
        {
            this.IsNotificationActive = false;
            this.IsAcceptIconActive = false;
            this._blockControl = false;
            
            InitializeAllDbContexts(); 
            
            if (this.ViasatApiDbContext != null && this.ViasatApiDbContext.CarsIdAndRegnumListFromApi != null)
            {
                this.CarsIdAndRegnumListFromApi = this.ViasatApiDbContext.CarsIdAndRegnumListFromApi;
            
                InitializeViewModelsAndAllCarsFromDbAndApiMainList();
            }
            else
                MessageBox.Show("Inicjalizacja obiektu do połączenia do API zakończona niepowodzeniem.");



            //SET FIRST PAGE IN "PAGES" DICTIONARY AS START PAGE
            //---------------------------------------------------
            this.SelectedPage = this.Pages.First().Value;
            this.SelectedPageName = PageName.StartPage;

            
            SelectPageCommand = new RelayCommand(SelectPage, CanCommandBeExecuted);          
            ExitCommand = new RelayCommand(DisconnectConnections, CanCommandBeExecuted);
            

           
           // Task task1 = Task.Run(() => TestAction());
            
        }


        //============================================================
        //                    COMMAND 
        //============================================================

        //Commands
        public ICommand SelectPageCommand { get; set; }
        public ICommand ExitCommand { get; set; }


        //ACTIONS for commands
        private void SelectPage(object value)
        {
            if (value is PageName pageName && this.Pages.TryGetValue(pageName, out IPage selectedPage2))
            {
                this.SelectedPage = selectedPage2;
                this.SelectedPageName = pageName;
            }
            else
            {
                MessageBox.Show("Nie znaleziono sekcji dla tego przycisku.");
            }
        }
        private void DisconnectConnections(object value)
        { 
            DisconnectAllConnections();
         

            Task task2 = Task.Run(async () =>
            {
                await Task.Delay(3000);
                Application.Current.Dispatcher.Invoke(() => { Application.Current.Shutdown(); });
            });

        }
        private bool CanCommandBeExecuted(object value)
        {
            if (CompanyDatabaseDbContext != null)
            {
                return true;
            }
            else { return false; }
        }

        private async void TestAction()
        {           
            await Task.Delay(6000);
            this.IsNotificationActive = true;
            this.NotificationString = "Połączono z baząd danych";
            await Task.Delay(5000);           
            this.IsNotificationActive = false;
        }


        // INITIALIZE AND DISCONNECT DBCONTEXT && TAKE DATA THROUGH THIS CONTEXTS
        //--------------------------------------------------------------------------------
        private void InitializeAllDbContexts()
        {         
            Task task1 = Task.Run(async () =>
            {
                this.ViasatApiDbContext = new ViasatDbContext();
                ViasatApiDbContext.PropertyChanged += MyPropertyChanged;
                this.ViasatApiDbContext.LoginUser();
               

            });
           // Task.WaitAll(task1);
            Task task2 = Task.Run(() =>
            {
                this.CompanyDatabaseDbContext = new CompanyDatabaseDbContext();
                CompanyDatabaseDbContext.PropertyChanged += MyPropertyChanged;
                this.CompanyDatabaseDbContext.InitializeCompanyDbConnection();
            });
            Task.WaitAll(task1, task2);
           
        }

        private void WriteIdDevicesToAllCarList()
        {
            int count = 0;
            string summary = "";
            List<string> list = new List<string>();

            foreach (var item in this.CarsIdAndRegnumListFromApi)
            {
                if (item.Name[0] == ' ')
                   item.Name = item.Name.Remove(0, 1);
                if (item.Name[item.Name.Length - 1] == ' ')
                   item.Name = item.Name.Remove(item.Name.Length - 1, 1);
            }

            foreach (var el in this.CarsIdAndRegnumListFromApi)
            {
                for (int i = 0; i< this.AllCarsFromDbAndApiMainList.Count; i++)
                {
                    if (this.AllCarsFromDbAndApiMainList[i].RegNum.Contains(el.Name))
                    {
                        count += 1;
                        this.AllCarsFromDbAndApiMainList[i].GpsDeviceId = el.Id;
                        break;
                    }
                    if (i== AllCarsFromDbAndApiMainList.Count-1)
                    {
                        list.Add(el.Name);
                    }
                }
            }

            foreach (var item in list)
            {
                summary += item + "\n";
            }
            if(count != ViasatApiDbContext.CarsIdAndRegnumListFromApi.Count)
            MessageBox.Show($"wszystkie auta z gps: {ViasatApiDbContext.CarsIdAndRegnumListFromApi.Count}\n wszystkie znalezione auta: {count}\n lista brakujacych: {summary}");
          
           
        }
               
        public void DisconnectAllConnections()
        {
            Task[] tasks = new Task[2];
            tasks[0] = Task.Run(() => ViasatApiDbContext.LogOffSession());           
            tasks[1] = Task.Run(() => CompanyDatabaseDbContext.EndCompanyDbConnection());

            Task.WaitAll(tasks);
        }

        private void InitializeViewModelsAndAllCarsFromDbAndApiMainList()
        {
            if (CompanyDatabaseDbContext != null && ViasatApiDbContext != null)
            {
                AllCarsFromDbAndApiMainList = CompanyDatabaseDbContext.GetAllActiveCarListFromCompanyDb();
                // MessageBox.Show($"lista pobranych aut aktywnych: {AllCarsFromDbAndApiMainList.Count}" );

                if (AllCarsFromDbAndApiMainList != null && AllCarsFromDbAndApiMainList.Count != 0 && CarsIdAndRegnumListFromApi != null)
                {
                    WriteIdDevicesToAllCarList();

                    this.Pages = new Dictionary<PageName, IPage>()
                    {
                        {PageName.StartPage, new StartPageViewModel(ViasatApiDbContext, this.AllCarsFromDbAndApiMainList ) },
                        {PageName.ViasatPage, new ViasatViewModel(ViasatApiDbContext, this.AllCarsFromDbAndApiMainList) },
                        {PageName.FleetPage, new FleetViewModel(CompanyDatabaseDbContext, this.AllCarsFromDbAndApiMainList) },
                        {PageName.DriversPage, new DriversViewModel(CompanyDatabaseDbContext, this.AllCarsFromDbAndApiMainList) },
                        {PageName.StatisticsPage, new StatisticsViewModel(CompanyDatabaseDbContext, ViasatApiDbContext, this.AllCarsFromDbAndApiMainList) },
                        {PageName.SettingsPage, new SettingsViewModel() }
                    };
                }
                else if (CarsIdAndRegnumListFromApi.Count == 0 || CarsIdAndRegnumListFromApi.Count == null)
                {
                    MessageBox.Show("Nie pobrano listy ID urządzeń Viasat zamontowanych w pojazdach firmowych lub lista jest pusta.\nMoże jest problem z połączeniem z interfejsem Viasat. ");
                }
                else
                    MessageBox.Show("Pobrana z bazy firmowej - lista aut aktywnych w firmie jest pusta!");
            }
        }



        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void MyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "IsConnectionToApiActive")
            {
               // MessageBox.Show("zmiana status - IsConnectionToApiActive: " + this.ViasatApiDbContext.IsConnectionToApiActive);
               Task Task1= Task.Run(async () =>
                {
                    if (this._blockControl == true)
                    {
                        await Task.Delay(2000);
                    }
                    this._blockControl = true;
                  //  await Task.Delay(1000);
                    this.IsNotificationActive = true;
                    this.IsAcceptIconActive = true;
                    if(this.ViasatApiDbContext.IsConnectionToApiActive == true)
                    {
                        this.NotificationString = "Połączono z API Viasat.";
                        await Task.Delay(3000);
                    }
                    else
                    {
                        this.NotificationString = "Rozłączono z API Viasat.";
                        await Task.Delay(2000);
                    }                   
                   
                    this.IsNotificationActive = false;
                    this.IsAcceptIconActive = false;
                    this._blockControl = false;
                });
            //   Task.WaitAll(Task1);
            }
            

            if(e.PropertyName == "IsConnectionToCompanyDbActive")
            {
               // MessageBox.Show("zmiana statusu - IsConnectionToCompanyDbActive: "+this.CompanyDatabaseDbContext.IsConnectionToCompanyDbActive);
               Task task1 = Task.Run(async () =>
                {
                    if(this._blockControl == true)
                    {
                        await Task.Delay(2000);
                    }
                   
                        this._blockControl = true;
                       // await Task.Delay(1000);                       
                        this.IsNotificationActive = true;
                        this.IsAcceptIconActive = true;
                        if (this.CompanyDatabaseDbContext.IsConnectionToCompanyDbActive == true)
                        {
                            this.NotificationString = "Połączono z firmową bazą danych.";
                        await Task.Delay(3000);
                        }
                        else if (this.CompanyDatabaseDbContext.IsConnectionToCompanyDbActive == false)
                        {
                        this.NotificationString = "Rozłączono z firmową bazą danych.";
                        await Task.Delay(2000);
                        }
                       
                        this.IsNotificationActive = false;
                        this.IsAcceptIconActive = false;
                        this._blockControl = false;
                    
                  

                } );
               // Task.WaitAll(task1);
            }

            if (e.PropertyName == "CarsIdAndRegnumListFromApi")
            {
                if (this.ViasatApiDbContext != null)
                {
                    this.CarsIdAndRegnumListFromApi = this.ViasatApiDbContext.CarsIdAndRegnumListFromApi;
                }
                else
                    MessageBox.Show("Nie można zauktalizować wartości, ponieważ obiekt komunikacji z API jest pusty");
            }
        }
        #endregion

        #region ICommand RelayCommand object
        class RelayCommand : ICommand
        {
            Action<object> _execute;
            Func<object, bool> _canExecute;

            // public event EventHandler? CanExecuteChanged;

            public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
            {
                _execute = execute;
                _canExecute = canExecute;
            } 
       
            public bool CanExecute(object parameter)
            {
                if (_canExecute != null)
                { return _canExecute(parameter); }
                else
                { return false; }

            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }


            public void Execute(object parameter) { _execute(parameter); }

        }
        #endregion

    }
}