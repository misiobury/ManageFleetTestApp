using Dashboard2.Model.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;
using Dashboard2.Model.Infrastructure.DataAccessLayer.CompanyDbPhysicalLayer;

namespace Dashboard2.ViewModel
{
   public class EditCarParamViewModel : INotifyPropertyChanged
    {
        //==============================================================
        //                   PROPERTIES AND FIELDS
        //==============================================================
        
        //Main
        private Car _selectedCar;
        public Car SelectedCar { get { return _selectedCar; } set { _selectedCar = value; OnPropertyChanged("SelectedCar"); } }
        public CompanyDatabaseDbContext DbContext { get; set; }


        //CAR PARAM
        private ObservableCollection<string> _carTypeList; 
        public ObservableCollection<string> CarTypeList { get { return _carTypeList; } set { _carTypeList = value; OnPropertyChanged("CarTypeList"); } }

        private int _carTypeListSelectedIndex;
        public int CarTypeListSelectedIndex { get { return _carTypeListSelectedIndex; } set { _carTypeListSelectedIndex = value; OnPropertyChanged("CarTypeListSelectedIndex"); } }

        private ObservableCollection<string> _fuelTypeList;
        public ObservableCollection<string> FuelTypeList { get { return _fuelTypeList; } set { _fuelTypeList = value; OnPropertyChanged("FuelTypeList"); } }

        private int _fuelTypeListSelectedIndex;
        public int FuelTypeListSelectedIndex { get { return _fuelTypeListSelectedIndex;} set { _fuelTypeListSelectedIndex = value; OnPropertyChanged("FuelTypeListSelectedIndex"); } }

        private ObservableCollection<string> _brandTypeList;
        public ObservableCollection<string> BrandTypeList { get { return _brandTypeList; } set { _brandTypeList = value; OnPropertyChanged("BrandTypeList"); } }

        private int _brandTypeListSelectedIndex;
        public int BrandTypeListSelectedIndex { get { return _brandTypeListSelectedIndex; } set { _brandTypeListSelectedIndex = value; BrandName = BrandTypeList[BrandTypeListSelectedIndex]; OnPropertyChanged("BrandTypeListSelectedIndex");  } }

        private string _brandName;
        public string BrandName { get { return _brandName; } set { _brandName = value; OnPropertyChanged("BrandName"); } }

        private string _modelName;
        public string ModelName { get { return _modelName; } set { _modelName = value; OnPropertyChanged("ModelName"); } }

        private ObservableCollection<string> _productionYearList;
        public ObservableCollection<string> ProductionYearList { get { return _productionYearList; } set { _productionYearList = value; OnPropertyChanged("ProductionYearList"); } }
        
        private int _productionYearListSelectedIndex;
        public int ProductionYearListSelectedIndex { get { return _productionYearListSelectedIndex; } set { _productionYearListSelectedIndex = value; OnPropertyChanged("ProductionYearListSelectedIndex"); } }

        private DateTime _technicalInspectionDate;
       // public string TechnicalInspectionDate2 { get { return $"{_technicalInspectionDate.Month}.{_technicalInspectionDate.Day}.{_technicalInspectionDate.Year}"; } set { _technicalInspectionDate = DateOnly.ParseExact(value, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture); OnPropertyChanged("TechnicalInspectionDate2"); } }
       
        public DateTime  TechnicalInspectionDate { get { return _technicalInspectionDate; } set { _technicalInspectionDate = value; OnPropertyChanged("TechnicalInspectionDate"); } }

        private DateTime _buyDate;
        public DateTime BuyDate { get { return _buyDate; } set { _buyDate = value; OnPropertyChanged("BuyDate"); } }

        private DateTime _firstRegDate;

        public DateTime FirstRegDate { get { return _firstRegDate; } set {  _firstRegDate = value; OnPropertyChanged("FirstRegDate"); } }

        private DateTime _withdrawalDate;
        public DateTime WithdrawalDate { get { return _withdrawalDate; } set { _withdrawalDate = value; OnPropertyChanged("WithdrawalDate"); } }


        private string _buyPrice;
        public string BuyPrice { get { return _buyPrice; } set { _buyPrice = value; OnPropertyChanged("BuyPrice"); } }

        //status Window
        private bool _needClose;
        public bool NeedClose { get { return _needClose; } set { _needClose = value; OnPropertyChanged("NeedClose"); } }



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



        static public event EventHandler<EventArgs> HasCreated;
        static public event EventHandler<SaveParamCarEventArgs> CloseWindowRequest;
        static public event EventHandler<SaveParamCarEventArgs> SaveCarParamRequest;


        //=============================================================
        //                   CONSTRUCTOR
        //=============================================================     

        public EditCarParamViewModel()
        {
            InitializeTabSection();

            HasCreated?.Invoke(this, EventArgs.Empty);

        }

        public EditCarParamViewModel(Car selectedCar)
        {
            this.SelectedCar = selectedCar;

            InitializeTabSection();
            InitializeCarParametersAndRestCommands();

            HasCreated?.Invoke(this, EventArgs.Empty);
        }




        //=============================================================
        //                   METHODS
        //=============================================================     


        #region InitializeCarParametersAndRestCommands
        private void InitializeCarParametersAndRestCommands()
        {
            NeedClose = false;


            if (DateTime.TryParseExact(SelectedCar.PurchaseDate, "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dateTimeTemp))
            {
                this.BuyDate = dateTimeTemp;
            }
            else
            {
                // this.TechnicalInspectionDate = DateTime.Now;
            }
             

            if (DateTime.TryParseExact(SelectedCar.FirstRegDate, "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dateTimeTemp2))
            {
                this.FirstRegDate = dateTimeTemp2;
            }
       

            if (DateTime.TryParseExact(SelectedCar.VehicleWithdrawalDate, "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dateTimeTemp3))
            {
                this.WithdrawalDate = dateTimeTemp3;
            
            }
  

            //  this.TechnicalInspectionDate = DateOnly.ParseExact(SelectedCar.TechnicalInspectionDate, "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);
           // MessageBox.Show(SelectedCar.TechnicalInspectionDate);
           if( DateTime.TryParseExact(SelectedCar.TechnicalInspectionDate, "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dateTimeTemp4 ))
            {
                this.TechnicalInspectionDate = dateTimeTemp4;
            }
           else
            {
            }

           
          

// MessageBox.Show($"{TechnicalInspectionDate.Year}-{TechnicalInspectionDate.Month}-{TechnicalInspectionDate.Day}");
            FuelTypeList = new ObservableCollection<string> { "Benzyna", "Diesel", "Benzyna+Gaz", "Brak" };
            switch(SelectedCar.FuelType)
            {
                case "Benzyna":
                    FuelTypeListSelectedIndex = 0;
                    break;
                case "Diesel":
                    FuelTypeListSelectedIndex = 1;
                    break;
                case "Benzyna+Gaz":
                    FuelTypeListSelectedIndex = 2;
                    break;
                default:
                    FuelTypeListSelectedIndex = 3;
                    break;
            }

            CarTypeList = new ObservableCollection<string> { "Osobowe", "Ciężarowe", "Combivan", "Dostawcze", "Inne" };
            switch(SelectedCar.CarType)
            {
                case "Osobowe":
                    CarTypeListSelectedIndex = 0;
                    break;
                case "Ciężarowe":
                    CarTypeListSelectedIndex = 1;
                    break;
                case "Combivan":
                    CarTypeListSelectedIndex = 2;
                    break;
                case "Dostawcze":
                    CarTypeListSelectedIndex = 3;
                    break;
                case "Inne":
                    CarTypeListSelectedIndex = 4;
                    break;
                default:
                    CarTypeListSelectedIndex = 4;
                    break;
            }

            /*
            BrandTypeList = new ObservableCollection<string> { 
                "Audi",
                "Alfa Romeo",
                "BMW",
                "Chevrolet",
                "Citroen",
                "Daf",
                "Fiat",
                "Ford",
                "Honda",
                "Hyundai",
                "Infiniti",
                "Iveco",
                "Jeep",
                "Kia",
                "Land Rover",
                "Lexus",
                "Man",
                "Mazda",
                "Mercedes",
                "Mini",
                "Mitsubishi",
                "Nissan",
                "Opel",
                "Peugeot",
                "Renault",
                "Seat",
                "Skoda",
                "Star",
                "Toyota",
                "VW",
                "Volvo",
                "Inny"
            };
            */

            BrandTypeList = BrandCarName.BrandList;

            BrandTypeListSelectedIndex = BrandTypeList.IndexOf(BrandTypeList.FirstOrDefault(x => SelectedCar.BrandName.Contains(x)));
            
            BrandName = BrandTypeList[BrandTypeListSelectedIndex];


          //  MessageBox.Show($"'{BrandName}'");
            //ModelName = SelectedCar.BrandModel.Substring(SelectedCar.BrandModel.IndexOf(" ",0)+1);
            ModelName = SelectedCar.BrandModel;

            ProductionYearList = new ObservableCollection<string>();
            ProductionYearList.Add("brak");
            for (int i = 1990; i<= DateTime.Now.Year; i++)
            {
                ProductionYearList.Add($"{i}");
            }
           
                if(ProductionYearList.FirstOrDefault(x=> x== SelectedCar.ProdDate )!=null )
            {
                // ProductionYearListSelectedIndex = 0; System.InvalidOperationException
                ProductionYearListSelectedIndex = ProductionYearList.IndexOf(ProductionYearList.First(x => x == SelectedCar.ProdDate));
            }
        
            else
            {
                ProductionYearListSelectedIndex = 0;
               // ProductionYearListSelectedIndex = ProductionYearList.IndexOf(ProductionYearList.First(x => x == SelectedCar.ProdDate));
            }                                  
            

            CloseWindow = new RelayCommand(closeWindow, CanCommanTabBeExecuted);
            SaveChanges = new RelayCommand(saveChanges, CanCommanTabBeExecuted);

        }

        #endregion



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
            System.Windows.MessageBox.Show("test");

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
        private bool CanCommanTabBeExecuted(object value)
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


        //----------------------------------
        //COMMANDS SECTION For WINDOW MAIN BUTTON
        //----------------------------------
        #region Commands for Window BTN
        public ICommand? CloseWindow { get; set; }
        public ICommand? SaveChanges { get; set; }
        #endregion

        #region Actions for Commands to Execute()
        private void closeWindow(object value)
        {
            CloseWindowRequest.Invoke(this, new SaveParamCarEventArgs(SelectedCar));
        }
        private void saveChanges(object value)
        {
            SaveCarParamRequest.Invoke(this, new SaveParamCarEventArgs(SelectedCar));
            CloseWindowRequest.Invoke(this, new SaveParamCarEventArgs(SelectedCar));
        }

        #endregion

        #region InitializeTabSection
        private void InitializeTabSection()
        {
            IsGeneralTabBtnActive = true;
            IsTankTabBtnActive = false;
            IsServiceTabBtnActive = false;
            IsBuySellTabBtnActive = false;

            GeneralTabBtnClicked = new RelayCommand(ShowGeneralSection, CanCommanTabBeExecuted);
            TankTabBtnClicked = new RelayCommand(ShowTankSection, CanCommanTabBeExecuted);
            ServiceTabBtnClicked = new RelayCommand(ShowServiceSection, CanCommanTabBeExecuted);
            BuySellTabBtnClicked = new RelayCommand(ShowBuySellSection, CanCommanTabBeExecuted);
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
