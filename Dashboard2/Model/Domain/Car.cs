using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard2.Model.Domain
{
  

    public class Car
    {
        //==============================================================
        //                 PROPERTIES AND FIELDS
        //==============================================================
        //General Section
        public string? Vin {  get; set; }
        public string? RegNum {  get; set; }
        public string?  Brand {  get; set; }
        private Driver? _driver { get; set; }

        public string? Driver 
        { get 
            { 
                if(_driver != null)
                return _driver.Surnname+" "+_driver.Name; 
                else 
                    return null;
            }  
        }
       // public string? Surname { get { return Driver.Surnname; } }
        //  public int? NumOfDriver { get; set; }

        public int? BranchID { get; set; }
        public string? BranchName {  get; set; }
        public bool? GpsMonitoring { get; set; }
        public string? GpsDeviceId { get; set; }
        public bool? CompanyOwn { get; set; }
        public string? CarType {  get; set; }


        //purachase/sale section
        public string? ProdDate {  get; set; }
        public string?  FirstRegDate {  get; set; }
        public string?   PurchaseDate{  get; set; }
        public string? PurchasePrice { get; set; }
        public bool? Akt { get; set; }
        public string? VehicleWithdrawalDate {  get; set; }
        public string? VehicleWithdrawalComment {  get; set; }


        //Tank/Fuel Section
        public string? FuelTankCapacity {  get; set; }
        public string? FuelConsumptionPer100 {  get; set; }
        public string? GasTankCapacity{  get; set; }
        public string? GasConsumptionPer100 {  get; set; }
        public string? FuelType {  get; set; }
        public string? OrlenTankCard {  get; set; }
        public string? BpTankCard {  get; set; }
        public string? PinCode {  get; set; }


        //Service Section
        public string? TechnicalInspectionDate {  get; set; }
        public string? NextTimingBeltReplacement {  get; set; }
        public string? NextOilChange {  get; set; }
        public string? TiresSize {  get; set; }
        public string? WinterTires {  get; set; }
        public string? TransportCost{  get; set; }
                            

        
     


        //=====================================================================
        //                   CONSTRUCTOR
        //=====================================================================
        public Car(CarDTOCompanyDb CarDTO)
        {
            Vin = CarDTO.NR_VIN;
            RegNum = CarDTO.NR_REJ;
            Brand = CarDTO.MARKA;
         //  int.TryParse(CarDTO.OSO_KOD, out int NumOfDriver);
           int.TryParse(CarDTO.FIR_KOD, out int BranchID);
            BranchName = CarDTO.FIR;
            GpsMonitoring = CarDTO.GPS=="T"? true : false;
            CompanyOwn = CarDTO.WLASNY=="T"? true : false;
            CarType = CarDTO.RODZAJ_SAMOCH;

            ProdDate = CarDTO.ROK_PROD;
            FirstRegDate = CarDTO.DAT_1_REJESTRACJI;
            PurchaseDate = CarDTO.DATA_ZAKUPU;
            PurchasePrice= CarDTO.CENA_ZAKUPU ;
            Akt = CarDTO.AKT == "T" ? true : false;
            VehicleWithdrawalDate = CarDTO.DAT_WYCOFANIA;
            VehicleWithdrawalComment = CarDTO.KOM_WYCOFANIA;

            FuelTankCapacity = CarDTO.BAK_PALIWO;
            FuelConsumptionPer100= CarDTO.ZUZ_PALIWO_100;
            GasTankCapacity = CarDTO.BAK_GAZ;
            GasConsumptionPer100 = CarDTO.ZUZ_GAZ_100;
            if(CarDTO.BENZYNA=="T")
            {
                FuelType = "Benzyna";
            }
            else if(CarDTO.DIESEL=="T")
            {
                FuelType = "Diesel";
            }
            else
            {
                FuelType = "Benzyna+Gaz";
            }

           OrlenTankCard = CarDTO.KARTA_ORLEN;
           BpTankCard = CarDTO.KARTA_BP;
           PinCode = CarDTO.KARTA_PIN;


            TechnicalInspectionDate = CarDTO.DATA_BADANIA_TECH;
            NextTimingBeltReplacement = CarDTO.NAST_WYM_ROZRZ;
            NextOilChange = CarDTO.NAST_WYM_OLEJU;
            TiresSize = CarDTO.OPONY_ROZMIAR;
            WinterTires = CarDTO.OPONY_ZIMOWE;
            TransportCost = CarDTO.KOSZT_TRANS;
            _driver = CarDTO.Driver;
        }


        



        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler? PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }

        }
        #endregion
    }
}
