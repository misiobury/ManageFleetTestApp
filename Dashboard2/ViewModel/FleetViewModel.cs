using Dashboard2.Model.Domain;
using Dashboard2.Model.Infrastructure.DataAccessLayer.CompanyDbPhysicalLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Configuration;

namespace Dashboard2.ViewModel
{


     class FleetViewModel : IPage
    {
        //==============================================================
        //                   PROPERTIES AND FIELDS
        //==============================================================
        public ObservableCollection<Car>? AllActiveCarsList { get; set; }
        public ObservableCollection<Car>? AllNonActiveCarsList { get; set; }

        private ICollectionView? _dataGridCollectionActiveCars;
        public ICollectionView DataGridCollectionActiveCars
        {
            get { if (_dataGridCollectionActiveCars != null)
                    return _dataGridCollectionActiveCars;
                else
                    return null;  }
            set { _dataGridCollectionActiveCars = value; OnPropertyChanged("DataGridCollectionActiveCars"); }
        }

        private ICollectionView? _dataGridCollectionNonActiveCars;
        public ICollectionView DataGridCollectionNonActiveCars
        {
            get
            {
                if (_dataGridCollectionNonActiveCars != null)
                    return _dataGridCollectionNonActiveCars;
                else
                    return null;
            }
            set { _dataGridCollectionNonActiveCars = value; OnPropertyChanged("DataGridCollectionNonActiveCars"); }
        }

        private string? _filterString;
        public string FilterString
        {
            get { return _filterString; }
            set
            {
                _filterString = value;
                OnPropertyChanged("FilterString");
                FilterCollection();
            }
        }


        private string? _countofcars;
        public string CountOfCars 
        {
            get { return _countofcars; } 
            set { _countofcars = value; OnPropertyChanged("CountOfCars"); }
        }


        //PAGE TITLE
        private string _pageTitle;
        public string PageTitle
        {
            get { return _pageTitle; }
            set { _pageTitle = value; OnPropertyChanged("PageTitle"); }
        }



        //Check is TAB BTN Active
        private bool _isActiveCarsBtnActive;
        public bool IsActiveCarsBtnActive { get { return _isActiveCarsBtnActive; } set { _isActiveCarsBtnActive = value; OnPropertyChanged("IsActiveCarsBtnActive"); }}

        private bool _isGeneralTabBtnActive;
        public bool IsGeneralTabBtnActive { get { return _isGeneralTabBtnActive; } set { _isGeneralTabBtnActive = value; OnPropertyChanged("IsGeneralTabBtnActive"); } }
        
        private bool _isTankTabBtnActive;
        public bool IsTankTabBtnActive { get { return _isTankTabBtnActive; } set { _isTankTabBtnActive = value; OnPropertyChanged("IsTankTabBtnActive"); } }

        private bool _isServiceTabBtnActive;
        public bool IsServiceTabBtnActive { get { return _isServiceTabBtnActive; } set { _isServiceTabBtnActive = value; OnPropertyChanged("IsServiceTabBtnActive"); } }

        private bool _isBuySellTabBtnActive;
        public bool IsBuySellTabBtnActive { get { return _isBuySellTabBtnActive; } set { _isBuySellTabBtnActive = value; OnPropertyChanged("IsBuySellTabBtnActive"); } }


       

        //=====================================================================
        //                   CONSTRUCTOR
        //=====================================================================
        public FleetViewModel() { }

        public FleetViewModel(CompanyDatabaseDbContext CompanyDbContext, ObservableCollection<Car> AllCarsListFromDb)
        {
            this.PageTitle = "Flota";
            List<Car> TempList1 = AllCarsListFromDb.Where(x => x.Akt == true).ToList();
            this.AllActiveCarsList = new ObservableCollection<Car>(TempList1) ;
            List<Car> TempList2 = AllCarsListFromDb.Where(x => x.Akt == false).ToList();
            this.AllNonActiveCarsList = new ObservableCollection<Car>(TempList2);

            InitializeDataGridForFilter();           
            InitializeTabSection();


        }



        //======================================================
        //                   METHODS
        //======================================================

        //----------------------------------
        //COMMANDS SECTION For TAB BTNs
        //----------------------------------
        #region Commands for TabSection
        public ICommand? ShowTest { get; set; }
        public ICommand? ActiveNonActiveToggleBtnClicked { get; set; }
        public ICommand? GeneralTabBtnClicked { get; set; }       
        public ICommand? TankTabBtnClicked { get; set; }
        public ICommand? ServiceTabBtnClicked { get; set; }
        public ICommand? BuySellTabBtnClicked { get; set; }
        #endregion

        #region Actions for Commands to Execute()
        private void ShowMeTest(object value)
        {
            MessageBox.Show("kliknales cos");
        }
        private void ChangeToggleBtnStatus(object value)
        {
            //MessageBox.Show("zmiana przycisku, stary status: "+this.IsActiveCarsBtnActive);
          

            if (this.IsActiveCarsBtnActive == true)
            { 
                
                if(AllNonActiveCarsList == null || AllNonActiveCarsList.Count == 0)
                {
                    MessageBox.Show("Lista aut, pobranych z firmowej bazy (których firma nie ma na stanie), jest pusta");
           
                }
                    
                    this.IsActiveCarsBtnActive = false;
               // MessageBox.Show("obecny status: " + this.IsActiveCarsBtnActive);
            }
            else if (this.IsActiveCarsBtnActive == false)
            {  
                if (AllActiveCarsList == null || AllActiveCarsList.Count == 0)
                {
                    MessageBox.Show("Lista aut, pobranych z firmowej bazy (aktualnie na stanie), jest pusta");
                }
                    
                this.IsActiveCarsBtnActive = true;
               // MessageBox.Show("obecny status: " + this.IsActiveCarsBtnActive);
            }
       


            /*
             if(value.ToString() == "true")
             {

                 this.IsActiveCarsBtnActive = true;
                // MessageBox.Show($"zmienilem wartosc przycisku na {this.IsActiveCarsBtnActive}");
             }
             else if(value.ToString() == "false")
             {
                 this.IsActiveCarsBtnActive= false;
                // MessageBox.Show($"zmienilem wartosc przycisku na {this.IsActiveCarsBtnActive}");
             }
            */
        }
        
        private void ShowGeneralSection(object value)
        {
           // MessageBox.Show("pokaz general section");
            IsGeneralTabBtnActive = true;
            IsTankTabBtnActive = false;
            IsServiceTabBtnActive = false;
            IsBuySellTabBtnActive = false;           
        }
         private void ShowTankSection(object value)
        {           
            IsGeneralTabBtnActive = false;
            IsTankTabBtnActive = true;
            IsServiceTabBtnActive = false;
            IsBuySellTabBtnActive = false;
           
        }
         private void ShowServiceSection(object value)
        {
            IsGeneralTabBtnActive = false;
            IsTankTabBtnActive = false;
            IsServiceTabBtnActive = true;
            IsBuySellTabBtnActive = false;
        }
         private void ShowBuySellSection(object value)
        {
            IsGeneralTabBtnActive = false;
            IsTankTabBtnActive = false;
            IsServiceTabBtnActive = false;
            IsBuySellTabBtnActive = true;
        }
        #endregion

        #region condition for commands
        private bool CanCommanTabBeExecutedForActiveCars(object value)
        {
            return true;
            /*
           // MessageBox.Show("sprawdzam warunek");
            if (IsActiveCarsBtnActive == true)
            {
                if (DataGridCollectionNonActiveCars != null)
                    return true;
                else
                {
                   // MessageBox.Show("Lista aut, pobranych z firmowej bazy (których firma nie ma na stanie), jest pusta");
                    return false;
                }
            }
            if (IsActiveCarsBtnActive == false)
            {
                if (DataGridCollectionActiveCars != null)
                    return false;
                else
                {
                  //  MessageBox.Show("Lista aut, pobranych z firmowej bazy (aktualnie na stanie), jest pusta");
                    return false;
                }
            }
            else
                return false;
            */
        }               
        #endregion


        #region InitializeTabSection
        private void InitializeTabSection()
        {
            IsActiveCarsBtnActive = true;
            IsGeneralTabBtnActive = true;
            IsTankTabBtnActive = false;
            IsServiceTabBtnActive = false;
            IsBuySellTabBtnActive = false;

            GeneralTabBtnClicked = new RelayCommand(ShowGeneralSection, CanCommanTabBeExecutedForActiveCars);
            TankTabBtnClicked = new RelayCommand(ShowTankSection, CanCommanTabBeExecutedForActiveCars);
            ServiceTabBtnClicked = new RelayCommand(ShowServiceSection, CanCommanTabBeExecutedForActiveCars);
            BuySellTabBtnClicked = new RelayCommand(ShowBuySellSection, CanCommanTabBeExecutedForActiveCars);
            ActiveNonActiveToggleBtnClicked = new RelayCommand(ChangeToggleBtnStatus, CanCommanTabBeExecutedForActiveCars);
        }
        #endregion


        //----------------------------------
        //Initialize DataGrid for Filter
        //----------------------------------
        #region InitializeDataGridForFilter
        private void InitializeDataGridForFilter()
        {
            if(AllActiveCarsList != null)
            {
                DataGridCollectionActiveCars = CollectionViewSource.GetDefaultView(AllActiveCarsList);
                DataGridCollectionActiveCars.Filter = new Predicate<object>(Filter);

                CountOfCars = DataGridCollectionActiveCars.Cast<Object>().Count().ToString();
            }
        }
        #endregion



        //FILTER METHODS
        //----------------------------------------------------------------------------
        #region Filter Methods
        private void FilterCollection()
        {
            if (_dataGridCollectionActiveCars != null)
            {

                _dataGridCollectionActiveCars.Refresh();
                CountOfCars = DataGridCollectionActiveCars.Cast<Object>().Count().ToString();
            }
        }

        public bool Filter(object obj)
        {
            var data = obj as Car;
            if (data != null)
            {
                if (!string.IsNullOrEmpty(_filterString))
                {
                    return data.RegNum.Contains(_filterString.ToUpper());// || data.Brand.Contains(_filterString);
                }
                return true;
            }
            return false;
        }
        #endregion



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
