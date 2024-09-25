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
using Dashboard2.View.Fleet;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Dashboard2.ViewModel
{


    class FleetViewModel : IPage
    {
        //==============================================================
        //                   PROPERTIES AND FIELDS
        //==============================================================
        private CompanyDatabaseDbContext _companyDbContext;
        public ObservableCollection<Car>? AllActiveCarsList { get; set; }
        public ObservableCollection<Car>? AllNonActiveCarsList { get; set; }

        private ICollectionView? _dataGridCollectionActiveCars;
        public ICollectionView DataGridCollectionActiveCars
        {
            get
            {
                if (_dataGridCollectionActiveCars != null)
                    return _dataGridCollectionActiveCars;
                else
                    return null;
            }
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



        //FILTER SECTION
        //********************
        #region filter section

        //brand selectedCar filter
        private List<bool> AllCarsFilterStatus;
      
        private bool _brandCarFilterIsOn;

        private string _filterContentsToolTip;
        public string FilterContentsToolTip { get { return _filterContentsToolTip; } set { _filterContentsToolTip=value; OnPropertyChanged("FilterContentsToolTip"); } }

        private string _filterContents;
        public string FilterContents { get { return _filterContents; } set { _filterContents = value; OnPropertyChanged("FilterContents"); } }


        public bool BrandCarFilterIsOn { get { return _brandCarFilterIsOn; } set { _brandCarFilterIsOn = value; FilterCollection(); OnPropertyChanged("BrandCarFilterIsOn"); } }

        private bool _alfaRomeoFilterIsOn;
        public bool AlfaRomeoFilterIsOn { get { return _alfaRomeoFilterIsOn; } set { _alfaRomeoFilterIsOn = value; if (value == true) _addBrandCarFilter("Alfa Romeo"); else _deleteBrandCarFilter("Alfa Romeo"); FilterCollection(); OnPropertyChanged("AlfaRomeoFilterIsOn"); } }

        private bool _audiFilterIsOn;
        public bool AudiFilterIsOn { get { return _audiFilterIsOn; } set { _audiFilterIsOn = value; if (value == true) _addBrandCarFilter("Audi"); else _deleteBrandCarFilter("Audi"); FilterCollection(); OnPropertyChanged("AudiFilterIsOn"); } }

        private bool _bmwFilterIsOn;
        public bool BMWFilterIsOn { get { return _bmwFilterIsOn; } set { _bmwFilterIsOn = value; if (value == true) _addBrandCarFilter("BMW"); else _deleteBrandCarFilter("BMW"); FilterCollection(); OnPropertyChanged("BMWFilterIsOn"); } }

        private bool _chevroletFilterIsOn;
        public bool ChevroletFilterIsOn { get { return _chevroletFilterIsOn; } set { _chevroletFilterIsOn = value; if (value == true) _addBrandCarFilter("Chevrolet"); else _deleteBrandCarFilter("Chevrolet"); FilterCollection(); OnPropertyChanged("ChevroletFilterIsOn"); } }

        private bool _citroenFilterIsOn;
        public bool CitroenFilterIsOn { get { return _citroenFilterIsOn; } set { _citroenFilterIsOn = value; if (value == true) _addBrandCarFilter("Citroen"); else _deleteBrandCarFilter("Citroen"); FilterCollection(); OnPropertyChanged("CitroenFilterIsOn"); } }

        private bool _dafFilterIsOn;
        public bool DafFilterIsOn { get { return _dafFilterIsOn; } set { _dafFilterIsOn = value; if (value == true) _addBrandCarFilter("Daf"); else _deleteBrandCarFilter("Daf"); FilterCollection(); OnPropertyChanged("DafFilterIsOn"); } }
        private bool _fiatFilterIsOn;
         public bool FiatFilterIsOn { get { return _fiatFilterIsOn; } set { _fiatFilterIsOn = value; if (value == true) _addBrandCarFilter("Fiat"); else _deleteBrandCarFilter("Fiat"); FilterCollection(); OnPropertyChanged("FiatFilterIsOn"); } }
           private bool _fordFilterIsOn;
         public bool FordFilterIsOn { get { return _fordFilterIsOn;  } set { _fordFilterIsOn = value; if (value == true) _addBrandCarFilter("Ford"); else _deleteBrandCarFilter("Ford"); FilterCollection(); OnPropertyChanged("FordFilterIsOn"); } }
           private bool _hondaFilterIsOn;
         public bool HondaFilterIsOn { get { return _hondaFilterIsOn; } set { _hondaFilterIsOn  = value; if (value == true) _addBrandCarFilter("Honda"); else _deleteBrandCarFilter("Honda"); FilterCollection(); OnPropertyChanged("HondaFilterIsOn"); } }
           private bool _hyundaiFilterIsOn;
         public bool HyundaiFilterIsOn { get { return _hyundaiFilterIsOn; } set { _hyundaiFilterIsOn = value; if (value == true) _addBrandCarFilter("Hyundai"); else _deleteBrandCarFilter("Hyundai"); FilterCollection(); OnPropertyChanged("HyundaiFilterIsOn"); } }
        
        private bool _infinitiFilterIsOn;
         public bool InfinitiFilterIsOn { get { return _infinitiFilterIsOn; } set { _infinitiFilterIsOn = value; if (value == true) _addBrandCarFilter("Infiniti"); else _deleteBrandCarFilter("Infiniti"); FilterCollection(); OnPropertyChanged("InfinitiFilterIsOn"); } }
           private bool _ivecoFilterIsOn;
         public bool IvecoFilterIsOn { get { return _ivecoFilterIsOn; } set { _ivecoFilterIsOn = value; if (value == true) _addBrandCarFilter("Iveco"); else _deleteBrandCarFilter("Iveco"); FilterCollection(); OnPropertyChanged("IvecoFilterIsOn "); } }
         private bool _jeepFilterIsOn;
         public bool JeepFilterIsOn { get { return _jeepFilterIsOn; } set { _jeepFilterIsOn = value; if (value == true) _addBrandCarFilter("Jeep"); else _deleteBrandCarFilter("Jeep"); FilterCollection(); OnPropertyChanged("JeepFilterIsOn"); } }
         private bool _kiaFilterIsOn;
         public bool KiaFilterIsOn { get { return _kiaFilterIsOn; } set { _kiaFilterIsOn = value; if (value == true) _addBrandCarFilter("Kia"); else _deleteBrandCarFilter("Kia"); FilterCollection(); OnPropertyChanged("KiaFilterIsOn"); } }
          

         private bool _landRoverFilterIsOn;
         public bool LandRoverFilterIsOn { get { return _landRoverFilterIsOn; } set { _landRoverFilterIsOn = value; if (value == true) _addBrandCarFilter("Land Rover"); else _deleteBrandCarFilter("Land Rover"); FilterCollection(); OnPropertyChanged("LandRoverFilterIsOn"); } }
          

         private bool _lexusFilterIsOn;
         public bool LexusFilterIsOn { get { return _lexusFilterIsOn; } set { _lexusFilterIsOn = value; if (value == true) _addBrandCarFilter("Lexus"); else _deleteBrandCarFilter("Lexus"); FilterCollection(); OnPropertyChanged("LexusFilterIsOn"); } }
          

           private bool _manFilterIsOn;
         public bool ManFilterIsOn { get { return _manFilterIsOn; } set { _manFilterIsOn = value; if (value == true) _addBrandCarFilter("Man"); else _deleteBrandCarFilter("Man"); FilterCollection(); OnPropertyChanged("ManFilterIsOn"); } }
          private bool _mazdaFilterIsOn;
         public bool MazdaFilterIsOn { get { return _mazdaFilterIsOn; } set { _mazdaFilterIsOn = value; if (value == true) _addBrandCarFilter("Mazda"); else _deleteBrandCarFilter("Mazda"); FilterCollection(); OnPropertyChanged(" MazdaFilterIsOn"); } }
          private bool _mercedesFilterIsOn;
         public bool MercedesFilterIsOn { get { return _mercedesFilterIsOn; } set { _mercedesFilterIsOn = value; if (value == true) _addBrandCarFilter("Mercedes"); else _deleteBrandCarFilter("Mercedes"); FilterCollection(); OnPropertyChanged("MercedesFilterIsOn"); } }
          private bool _miniFilterIsOn;
         public bool MiniFilterIsOn { get { return _miniFilterIsOn; } set { _miniFilterIsOn = value; if (value == true) _addBrandCarFilter("Mini"); else _deleteBrandCarFilter("Mini"); FilterCollection(); OnPropertyChanged("MiniFilterIsOn"); } }
          

         private bool _mitsubishiFilterIsOn;
         public bool MitsubishiFilterIsOn { get { return _mitsubishiFilterIsOn; } set { _mitsubishiFilterIsOn = value; if (value == true) _addBrandCarFilter("Mitsubishi"); else _deleteBrandCarFilter("Mitsubishi"); FilterCollection(); OnPropertyChanged("MitsubishiFilterIsOn"); } }
          

           private bool _nissanFilterIsOn;
         public bool NissanFilterIsOn { get { return _nissanFilterIsOn; } set { _nissanFilterIsOn = value; FilterCollection(); OnPropertyChanged("NissanFilterIsOn"); } }

        private bool _opelFilterIsOn;
        public bool OpelFilterIsOn { get { return _opelFilterIsOn; } set { _opelFilterIsOn = value; if (value == true) _addBrandCarFilter("Opel"); else _deleteBrandCarFilter("Opel"); FilterCollection(); OnPropertyChanged("OpelFilterIsOn"); } }

        private bool _peugeotFilterIsOn;
         public bool PeugeotFilterIsOn { get { return _peugeotFilterIsOn; } set { _peugeotFilterIsOn = value; if (value == true) _addBrandCarFilter("Peugeot"); else _deleteBrandCarFilter("Peugeot"); FilterCollection(); OnPropertyChanged("PeugeotFilterIsOn"); } }
          

           private bool _renaultFilterIsOn;
         public bool RenaultFilterIsOn { get { return _renaultFilterIsOn; } set { _renaultFilterIsOn = value; if (value == true) _addBrandCarFilter("Renault"); else _deleteBrandCarFilter("Renault"); FilterCollection(); OnPropertyChanged("RenaultFilterIsOn"); } }
          

           private bool _seatFilterIsOn;
         public bool SeatFilterIsOn { get { return _seatFilterIsOn; } set {  _seatFilterIsOn = value; if (value == true) _addBrandCarFilter("Seat"); else _deleteBrandCarFilter("Seat"); FilterCollection(); OnPropertyChanged("SeatFilterIsOn"); } }
               private bool _skodaFilterIsOn;
         public bool SkodaFilterIsOn { get { return _skodaFilterIsOn; } set { _skodaFilterIsOn = value; if (value == true) _addBrandCarFilter("Skoda"); else _deleteBrandCarFilter("Skoda"); FilterCollection(); OnPropertyChanged("SkodaFilterIsOn"); } }
               private bool _starFilterIsOn;
         public bool StarFilterIsOn { get { return _starFilterIsOn; } set { _starFilterIsOn = value; if (value == true) _addBrandCarFilter("Star"); else _deleteBrandCarFilter("Star"); FilterCollection(); OnPropertyChanged("StarFilterIsOn"); } }
               private bool _toyotaFilterIsOn;
         public bool ToyotaFilterIsOn { get { return _toyotaFilterIsOn; } set { _toyotaFilterIsOn = value; if (value == true) _addBrandCarFilter("Toyota"); else _deleteBrandCarFilter("Toyota"); FilterCollection(); OnPropertyChanged("ToyotaFilterIsOn"); } }
           private bool _vwFilterIsOn;
         public bool VWFilterIsOn { get { return _vwFilterIsOn; } set { _vwFilterIsOn = value; if (value == true) _addBrandCarFilter("VW"); else _deleteBrandCarFilter("VW"); FilterCollection(); OnPropertyChanged("VWFilterIsOn"); } }
           private bool _volvoFilterIsOn;
         public bool VolvoFilterIsOn { get { return _volvoFilterIsOn; } set { _volvoFilterIsOn = value; if (value == true) _addBrandCarFilter("Volvo"); else _deleteBrandCarFilter("Volvo"); FilterCollection(); OnPropertyChanged("VolvoFilterIsOn"); } }
          



          /*   
           private bool _FilterIsOn;
         public bool FilterIsOn { get { return ; } set {  = value; FilterCollection(); OnPropertyChanged(""); } }
           private bool _FilterIsOn;
         public bool FilterIsOn { get { return ; } set {  = value; FilterCollection(); OnPropertyChanged(""); } }
           private bool _FilterIsOn;
         public bool FilterIsOn { get { return ; } set {  = value; FilterCollection(); OnPropertyChanged(""); } }

         */




        private bool _branchCompanyFilterIsOn;
        public bool BranchCompanyFilterIsOn { get { return _branchCompanyFilterIsOn; } set { _branchCompanyFilterIsOn = value; OnPropertyChanged("BranchCompanyFilterIsOn"); } }

        private bool _carTypeFilterIsOn;
        public bool CarTypeFilterIsOn { get { return _carTypeFilterIsOn; } set { _carTypeFilterIsOn = value; OnPropertyChanged("CarTypeFilterIsOn"); } }

        private bool _gpsStatusFilterIsOn;
        public bool GpsStatusFilterIsOn { get { return _gpsStatusFilterIsOn; } set { _gpsStatusFilterIsOn = value; OnPropertyChanged("CarTypeFilterIsOn"); } }

        private bool _fuelTypeFilterIsOn;
        public bool FuelTypeFilterIsOn { get { return _fuelTypeFilterIsOn; } set { _fuelTypeFilterIsOn = value; OnPropertyChanged("FuelTypeFilterIsOn"); } }

        private bool _ownFilterIsOn;
        public bool OwnFilterIsOn { get { return _fuelTypeFilterIsOn; } set { _fuelTypeFilterIsOn = value; OnPropertyChanged(" OwnFilterIsOn"); } }


        #endregion



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
        public bool IsActiveCarsBtnActive { get { return _isActiveCarsBtnActive; } set { _isActiveCarsBtnActive = value; OnPropertyChanged("IsActiveCarsBtnActive"); } }

        private bool _isGeneralTabBtnActive;
        public bool IsGeneralTabBtnActive { get { return _isGeneralTabBtnActive; } set { _isGeneralTabBtnActive = value; OnPropertyChanged("IsGeneralTabBtnActive"); } }

        private bool _isTankTabBtnActive;
        public bool IsTankTabBtnActive { get { return _isTankTabBtnActive; } set { _isTankTabBtnActive = value; OnPropertyChanged("IsTankTabBtnActive"); } }

        private bool _isServiceTabBtnActive;
        public bool IsServiceTabBtnActive { get { return _isServiceTabBtnActive; } set { _isServiceTabBtnActive = value; OnPropertyChanged("IsServiceTabBtnActive"); } }

        private bool _isBuySellTabBtnActive;
        public bool IsBuySellTabBtnActive { get { return _isBuySellTabBtnActive; } set { _isBuySellTabBtnActive = value; OnPropertyChanged("IsBuySellTabBtnActive"); } }


        //  private int _indexOfDataGrid;
        //  public int IndexOfDataGrid { get { return _indexOfDataGrid; } set { _indexOfDataGrid = value; OnPropertyChanged("IndexOfDataGrid"); } }//MessageBox.Show($"{DataGridCollectionActiveCars.CurrentItem}")



        private EditCarParamViewModel _editCarViewModel;
        public EditCarParamViewModel EditCarViewModel { get { return _editCarViewModel; } set { _editCarViewModel = value; OnPropertyChanged("EditCarViewModel"); } }

        public ObservableCollection<EditCarParamViewModel> EditCarParamVMList;

        public ObservableCollection<MyKeyValue> BrandCarListForFilter;


        static public event EventHandler<EventArgs> EditCarViewModelHasCreated;



        //=====================================================================
        //                   CONSTRUCTOR
        //=====================================================================
        public FleetViewModel() { }

        public FleetViewModel(CompanyDatabaseDbContext CompanyDbContext, ObservableCollection<Car> AllCarsListFromDb)
        {
            this.PageTitle = "Flota";
            this._companyDbContext = CompanyDbContext;
            List<Car> TempList1 = AllCarsListFromDb.Where(x => x.Akt == true).ToList();
            this.AllActiveCarsList = new ObservableCollection<Car>(TempList1);
            List<Car> TempList2 = AllCarsListFromDb.Where(x => x.Akt == false).ToList();
            this.AllNonActiveCarsList = new ObservableCollection<Car>(TempList2);
            this.EditCarParamVMList = new ObservableCollection<EditCarParamViewModel>();
            InitializeFilterSectionForDataGrid();

            InitializeTabSection();

            EditCarParamViewModel.CloseWindowRequest += CloseEditCarParamWindowAction;
            EditCarParamViewModel.SaveCarParamRequest += SaveCarParamAction;
        }

        private void CloseEditCarParamWindowAction(object sender, SaveParamCarEventArgs selectedCar)
        {
            // MessageBox.Show("wyzwolono zdarzenie closewindow");
            

            if (EditCarParamVMList.Any(x => x.SelectedCar.RegNum == selectedCar.Car.RegNum))
            {
                if (!EditCarParamVMList.Remove(EditCarParamVMList.FirstOrDefault(x => x.SelectedCar.RegNum == selectedCar.Car.RegNum)))
                {
                    MessageBox.Show($"Nie znaleziono obiektu dla auta o nr rej: {selectedCar.Car.RegNum}");
                }
              
                 //   MessageBox.Show($"usunieto VM dla {selectedCar.Car.RegNum}");

            }
        }

           private void SaveCarParamAction(object sender, SaveParamCarEventArgs e)
        {
         
            _companyDbContext.UpdateCarParamInDb(e.Car);
        }




        //======================================================
        //                   METHODS
        //======================================================

        //----------------------------------
        //COMMANDS SECTION For TAB BTNs
        //----------------------------------
        #region Commands for TabSection
        public ICommand? ShowTest { get; set; }
        public ICommand? EditCarParamClicked { get; set; }
        public ICommand? ActiveNonActiveToggleBtnClicked { get; set; }
        public ICommand? GeneralTabBtnClicked { get; set; }
        public ICommand? TankTabBtnClicked { get; set; }
        public ICommand? ServiceTabBtnClicked { get; set; }
        public ICommand? BuySellTabBtnClicked { get; set; }

        public ICommand? AddBrandCarFilter { get; set; }
        public ICommand? DeleteBrandCarFilter { get; set; }
        public ICommand? AddBranchCompanyFilter { get; set; }
        public ICommand? DeleteBranchCompanyFilter { get; set; }
        public ICommand? AddCarTypeFilter { get; set; }
        public ICommand? DeleteCarTypeFilter { get; set; }
        public ICommand? AddGpsStatusFilter { get; set; }
        public ICommand? DeleteGpsStatusFilter { get; set; }
        public ICommand? AddFuelTypeFilter { get; set; }
        public ICommand? DeleteFuelTypeFilter { get; set; }
        public ICommand? AddOwnFilter { get; set; }
        public ICommand? DeleteOwnFilter { get; set; }
        #endregion

        #region Actions for Commands to Execute()

      
        
        private void ShowMeTest(object value)
        {
            MessageBox.Show("test");
           
        }

        private void _editCarParam(object value)
        {
            Car selectedCar = (Car)DataGridCollectionActiveCars.CurrentItem;

            if (EditCarParamVMList.Any(x => x.SelectedCar.RegNum == selectedCar.RegNum))
            {
                MessageBox.Show("Okno do edycji paramterów tego auta, jest już otwarte.");
            }
            else
            {
                EditCarParamVMList.Add(new EditCarParamViewModel(this.DataGridCollectionActiveCars.CurrentItem as Car));
            }


            //MessageBox.Show("utworzono nowy EditCarViewModel\nnr rej: "+ this.EditCarViewModel.SelectedCar.RegNum);
            //  MessageBox.Show("utworzono nowy EditCarViewModel\nnr rej: "+ EditCarParamVMList[0].SelectedCar.RegNum);
            EditCarViewModelHasCreated.Invoke(this, new EventArgs());


        }



        private void _addBrandCarFilter(object value)
        {
            var filterCondition = value as string;
            // BrandCarListForFilter.Add(filterCondition, true);
            if (BrandCarListForFilter == null)
                BrandCarListForFilter = new();

            if (BrandCarListForFilter.Any(x => x.Key == filterCondition))
            {
                BrandCarListForFilter.FirstOrDefault(x => x.Key == filterCondition).Value = true;
            }
            else
            {
                BrandCarListForFilter.Add(new MyKeyValue(filterCondition, true));
            }

            DataGridCollectionActiveCars.Filter = new Predicate<object>(Filter);

            FilterContentsToolTip = "";
            FilterContents = "";
            foreach (var item in BrandCarListForFilter)
            {
               if(item.Value == true)
                { 
                    FilterContentsToolTip += $" {item.Key},";

                    if (FilterContents.Count() > 15)
                    {
                        continue;
                    }                      
                    else if((FilterContents.Count()+item.Key.Count())>15)
                    {                       
                        FilterContents += $" {item.Key},";
                        FilterContents = FilterContents.Insert(16, "....");
                    }
                    else
                        FilterContents += $" {item.Key},";
                }
            }

           

            if (BrandCarFilterIsOn == false)
                BrandCarFilterIsOn = true;
        }


        private void _deleteBrandCarFilter(object value)
        {
            var filterCondition = value as string;
            if (filterCondition == "All")
            {
                DataGridCollectionActiveCars.Filter = null;
                if(BrandCarListForFilter != null)
                BrandCarListForFilter.Where(x => x.Value == true).ToList().ForEach(x => x.Value = false);

                OpelFilterIsOn = false;
                BMWFilterIsOn = false;
                AlfaRomeoFilterIsOn = false;
                AudiFilterIsOn = false;
                BMWFilterIsOn = false;
                ChevroletFilterIsOn = false;
                CitroenFilterIsOn = false;
                DafFilterIsOn = false;
                FiatFilterIsOn = false;
              FordFilterIsOn = false;
              HondaFilterIsOn = false;
                HondaFilterIsOn = false;
              InfinitiFilterIsOn = false;
              IvecoFilterIsOn = false;
              JeepFilterIsOn = false;
              KiaFilterIsOn = false;
              LandRoverFilterIsOn = false;
                LexusFilterIsOn = false;
              ManFilterIsOn = false;
                MazdaFilterIsOn = false;
                MercedesFilterIsOn = false;
                MiniFilterIsOn = false;
                MitsubishiFilterIsOn = false;
                NissanFilterIsOn = false;
                OpelFilterIsOn = false;
                PeugeotFilterIsOn = false;
              RenaultFilterIsOn=false;
                SeatFilterIsOn = false;
                SkodaFilterIsOn = false;
                StarFilterIsOn = false;
                ToyotaFilterIsOn = false;
                VWFilterIsOn = false;
                VolvoFilterIsOn = false;

            

                // BrandCarListForFilter.Clear();
                BrandCarFilterIsOn = false;
            }
            else
            {
                if (BrandCarListForFilter.Any(x => x.Key == filterCondition))
                {
                    BrandCarListForFilter.FirstOrDefault(x => x.Key == filterCondition).Value = false;

                    if (BrandCarListForFilter.All(x => x.Value == false))
                    {
                        DataGridCollectionActiveCars.Filter = null;
                        BrandCarFilterIsOn = false;                      
                    }
                    else
                        DataGridCollectionActiveCars.Filter = new Predicate<object>(Filter);


                    FilterContentsToolTip = "";
                    FilterContents = "";
                    foreach (var item in BrandCarListForFilter)
                    {
                        if (item.Value == true)
                        {
                            FilterContentsToolTip += $" {item.Key},";

                            if (FilterContents.Count() > 15)
                            {
                                continue;
                            }
                            else if ((FilterContents.Count() + item.Key.Count()) > 15)
                            {
                                FilterContents += $" {item.Key},";
                                FilterContents = FilterContents.Insert(16, "....");
                            }
                            else
                                FilterContents += $" {item.Key},";
                        }
                    }
                }
            }
        }

    

        private void ChangeToggleBtnStatus(object value)
        {
            //MessageBox.Show("zmiana przycisku, stary status: "+this.IsActiveCarsBtnActive);


            if (this.IsActiveCarsBtnActive == true)
            {

                if (AllNonActiveCarsList == null || AllNonActiveCarsList.Count == 0)
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

            AddBrandCarFilter = new RelayCommand(_addBrandCarFilter, CanCommanTabBeExecutedForActiveCars);
            DeleteBrandCarFilter = new RelayCommand(_deleteBrandCarFilter, CanCommanTabBeExecutedForActiveCars);

            ShowTest = new RelayCommand(ShowMeTest, CanCommanTabBeExecutedForActiveCars);
            EditCarParamClicked = new RelayCommand(_editCarParam, CanCommanTabBeExecutedForActiveCars);
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
        private void InitializeFilterSectionForDataGrid()
        {
            if (AllActiveCarsList != null)
            {
                DataGridCollectionActiveCars = CollectionViewSource.GetDefaultView(AllActiveCarsList);
                DataGridCollectionActiveCars.Filter = null;
              //  DataGridCollectionActiveCars.Filter = new Predicate<object>(Filter);
             //  
                CountOfCars = DataGridCollectionActiveCars.Cast<Object>().Count().ToString();
            }

            _brandCarFilterIsOn = false;
            _branchCompanyFilterIsOn = false;
            _carTypeFilterIsOn = false;
            _gpsStatusFilterIsOn = false;
            _fuelTypeFilterIsOn = false;
            _ownFilterIsOn = false;

            
           /* 
            {
   AllCarsFilterStatus.Add(BMWFilterIsOn);
            AllCarsFilterStatus.Add(OpelFilterIsOn);
              AlfaRomeoFilterIsOn,
              AudiFilterIsOn,
              BMWFilterIsOn,
              ChevroletFilterIsOn,
              CitroenFilterIsOn,
              DafFilterIsOn,
              FiatFilterIsOn,
              FordFilterIsOn,
              HondaFilterIsOn,
              HondaFilterIsOn,
              InfinitiFilterIsOn,
              IvecoFilterIsOn,
              JeepFilterIsOn,
              KiaFilterIsOn,
              LandRoverFilterIsOn,
              LexusFilterIsOn,
              ManFilterIsOn,
              MazdaFilterIsOn,
              MercedesFilterIsOn,
              MiniFilterIsOn,
              MitsubishiFilterIsOn,
              NissanFilterIsOn,
              OpelFilterIsOn,
              PeugeotFilterIsOn,
              RenaultFilterIsOn,
              SeatFilterIsOn,
              SkodaFilterIsOn,
              StarFilterIsOn,
              ToyotaFilterIsOn,
              VWFilterIsOn,
              VolvoFilterIsOn,
             
          };
           */


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
          
                if(BrandCarFilterIsOn == true)
                {                    
                    if(BrandCarListForFilter.Count() > 0 )
                    {
                    
                         foreach (var el in BrandCarListForFilter)
                        {
                            if (el.Value == true && data.BrandName.Contains(el.Key))
                            {                             
                                return true;
                            }                           
                        }                        
                    }
                }
                if(BranchCompanyFilterIsOn == true)
                {

                }
                if(CarTypeFilterIsOn == true)
                {

                }
                if(GpsStatusFilterIsOn == true)
                {

                }
                if(FuelTypeFilterIsOn == true)
                {

                }
                if(OwnFilterIsOn == true)
                {

                }



                if (!string.IsNullOrEmpty(_filterString))
                {
                        return (data.RegNum.Contains(_filterString.ToUpper()) || (data.Driver != null ? data.Driver.Contains(_filterString) : false)
                            || ((_filterString.Length == 1 && data.Driver != null  ) ? data.Driver[0].ToString() == _filterString.ToUpper() : false))
                            || ((_filterString.Length >= 2 && data.Driver != null  ) ? data.Driver.Contains(_filterString.Substring(0, 1).ToUpper() + _filterString.Substring(1)) : false);
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
