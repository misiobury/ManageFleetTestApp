using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using Dashboard2.Model.Domain;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Dashboard2.Model.Infrastructure.DataAccessLayer.CompanyDbPhysicalLayer
{
    public class CompanyDatabaseDbContext : INotifyPropertyChanged
    {     
        private string _ConnectionString = AppGeneralConfigStaticClass.OracleConnectionString;
        private OracleConnection _OracleConnection = new OracleConnection();
        private ObservableCollection<CarDTOCompanyDb> TempAllCarsFromCompanyDb = new ObservableCollection<CarDTOCompanyDb>();
        private ObservableCollection<Car> AllActiveCarListFromCompanyDb = new ObservableCollection<Car>();
        private ObservableCollection<Car> AllNonActiveCarListFromCompanyDb = new ObservableCollection<Car>();
        private bool _isConnectionToCompanyDbActive;
        public bool IsConnectionToCompanyDbActive { get { return _isConnectionToCompanyDbActive; } set { _isConnectionToCompanyDbActive = value; OnPropertyChanged("IsConnectionToCompanyDbActive"); } }


        //=========================================================================================================================
        //                   CONSTRUCTOR
        //=========================================================================================================================
        public CompanyDatabaseDbContext()
        {
            this.IsConnectionToCompanyDbActive = false;
          //  InitializeCompanyDbConnection();           
         //  InitializeCarListFromCompanyDb();


           


        }


        //=========================================================================================================================
        //                   METHODS
        //=========================================================================================================================

  
        //Connecting/disconnecting Methods to Database 
        public void InitializeCompanyDbConnection()
        {
          //  OracleConfiguration.SqlNetAllowedLogonVersionClient = OracleAllowedLogonVersionClient.Version11;            
            OracleConfiguration.SqlNetAllowedLogonVersionClient = AppGeneralConfigStaticClass.OracleClientVersionForLoginToDatabase;            
            this._OracleConnection.ConnectionString = this._ConnectionString;
            try
            {
                this._OracleConnection.Open();
               // MessageBox.Show("Połączono z firmową bazą danych");
               this.IsConnectionToCompanyDbActive=true;
            }
            catch(Exception ex) 
            {
                MessageBox.Show("Nieudana próba połączenia z firmową bazą aut\n"+ex.Message);
            }

            InitializeCarListFromCompanyDb();
        }

       public void EndCompanyDbConnection()
        {
            try
            {
                _OracleConnection.Close();
                this.IsConnectionToCompanyDbActive=false;
              //  MessageBox.Show("Zakończono połączenie z firmową bazą danych");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Nieudana próba zakończenia połączenie z firmową bazą danych\n"+ex.Message);
            }

        }



        //Initialize Car List from Db
        private void InitializeCarListFromCompanyDb()
        { 
          
           // command1.ExecuteNonQuery();
           
          
          

            OracleCommand SelectAllActiveCarCommand = _OracleConnection.CreateCommand();
            SelectAllActiveCarCommand.CommandText = SqlCommandsStaticClass.SelectAllActiveCarsCommand;

            OracleCommand SelectAllActiveDriversCommand = _OracleConnection.CreateCommand();
            SelectAllActiveDriversCommand.CommandText = SqlCommandsStaticClass.SelectAllActiveDriversCommand;

            OracleCommand SelectAllNonActiveCarsCommand = _OracleConnection.CreateCommand();
            SelectAllNonActiveCarsCommand.CommandText = SqlCommandsStaticClass.SelectAllNonActiveCarsCommand;

           


            OracleDataReader ActiveDriversReader = SelectAllActiveDriversCommand.ExecuteReader();
            List<string>? SingleDriverParameters = new List<string>();
            Dictionary<string, Driver> TempDriversList = new Dictionary<string, Driver>();

            while (ActiveDriversReader.Read())
            {
                SingleDriverParameters.Clear();

                for (int i = 0; i < ActiveDriversReader.FieldCount; i++)
                {
                    if (!(ActiveDriversReader.IsDBNull(i)))
                    {
                        SingleDriverParameters.Add(ActiveDriversReader.GetString(i));
                    }
                    else
                    {
                        SingleDriverParameters.Add(null);
                    }
                }

                TempDriversList.Add(SingleDriverParameters[0], new Driver(SingleDriverParameters[1], SingleDriverParameters[3], SingleDriverParameters[2]));
            }


            OracleDataReader AllActiveCarsReader = SelectAllActiveCarCommand.ExecuteReader();
            List<string?> ActiveCarListParameters = new List<string?>();

            while (AllActiveCarsReader.Read())
            {
                ActiveCarListParameters.Clear();
                for (int i = 0; i < AllActiveCarsReader.FieldCount; i++)
                {
                    if (!(AllActiveCarsReader.IsDBNull(i)))
                    {
                        ActiveCarListParameters.Add(AllActiveCarsReader.GetString(i));
                    }
                    else
                    {
                        ActiveCarListParameters.Add(null);
                    }
                }


                if (TempDriversList.TryGetValue(ActiveCarListParameters[1], out Driver TempDriver))
                {
                    AllActiveCarListFromCompanyDb.Add(new Car(new CarDTOCompanyDb()
                    {
                        NR_VIN = ActiveCarListParameters[0],
                        NR_REJ = ActiveCarListParameters[1],
                        MARKA = ActiveCarListParameters[2],
                        OSO_KOD = ActiveCarListParameters[3],
                        FIR_KOD = ActiveCarListParameters[4],
                        FIR = ActiveCarListParameters[5],
                        GPS = ActiveCarListParameters[6],
                        WLASNY = ActiveCarListParameters[7],
                        RODZAJ_SAMOCH = ActiveCarListParameters[8],
                        ROK_PROD = ActiveCarListParameters[9],
                        DAT_1_REJESTRACJI = ActiveCarListParameters[10],
                        DATA_ZAKUPU = ActiveCarListParameters[11],
                        CENA_ZAKUPU = ActiveCarListParameters[12],
                        AKT = ActiveCarListParameters[13],
                        DAT_WYCOFANIA = ActiveCarListParameters[14],
                        KOM_WYCOFANIA = ActiveCarListParameters[15],
                        BAK_PALIWO = ActiveCarListParameters[16],
                        ZUZ_PALIWO_100 = ActiveCarListParameters[17],
                        BAK_GAZ = ActiveCarListParameters[18],
                        ZUZ_GAZ_100 = ActiveCarListParameters[19],
                        BENZYNA = ActiveCarListParameters[20],
                        DIESEL = ActiveCarListParameters[21],
                        GAZ = ActiveCarListParameters[22],
                        KARTA_ORLEN = ActiveCarListParameters[23],
                        KARTA_BP = ActiveCarListParameters[24],
                        KARTA_PIN = ActiveCarListParameters[25],
                        DATA_BADANIA_TECH = ActiveCarListParameters[26],
                        NAST_WYM_ROZRZ = ActiveCarListParameters[27],
                        NAST_WYM_OLEJU = ActiveCarListParameters[28],
                        OPONY_ROZMIAR = ActiveCarListParameters[29],
                        OPONY_ZIMOWE = ActiveCarListParameters[30],
                        KOSZT_TRANS = ActiveCarListParameters[31],
                        Driver = new Driver(TempDriver.IdPerson, TempDriver.Name, TempDriver.Surnname)
                    }));
                }
                else
                {
                    AllActiveCarListFromCompanyDb.Add(new Car(new CarDTOCompanyDb()
                    {
                        NR_VIN = ActiveCarListParameters[0],
                        NR_REJ = ActiveCarListParameters[1],
                        MARKA = ActiveCarListParameters[2],
                        OSO_KOD = ActiveCarListParameters[3],
                        FIR_KOD = ActiveCarListParameters[4],
                        FIR = ActiveCarListParameters[5],
                        GPS = ActiveCarListParameters[6],
                        WLASNY = ActiveCarListParameters[7],
                        RODZAJ_SAMOCH = ActiveCarListParameters[8],
                        ROK_PROD = ActiveCarListParameters[9],
                        DAT_1_REJESTRACJI = ActiveCarListParameters[10],
                        DATA_ZAKUPU = ActiveCarListParameters[11],
                        CENA_ZAKUPU = ActiveCarListParameters[12],
                        AKT = ActiveCarListParameters[13],
                        DAT_WYCOFANIA = ActiveCarListParameters[14],
                        KOM_WYCOFANIA = ActiveCarListParameters[15],
                        BAK_PALIWO = ActiveCarListParameters[16],
                        ZUZ_PALIWO_100 = ActiveCarListParameters[17],
                        BAK_GAZ = ActiveCarListParameters[18],
                        ZUZ_GAZ_100 = ActiveCarListParameters[19],
                        BENZYNA = ActiveCarListParameters[20],
                        DIESEL = ActiveCarListParameters[21],
                        GAZ = ActiveCarListParameters[22],
                        KARTA_ORLEN = ActiveCarListParameters[23],
                        KARTA_BP = ActiveCarListParameters[24],
                        KARTA_PIN = ActiveCarListParameters[25],
                        DATA_BADANIA_TECH = ActiveCarListParameters[26],
                        NAST_WYM_ROZRZ = ActiveCarListParameters[27],
                        NAST_WYM_OLEJU = ActiveCarListParameters[28],
                        OPONY_ROZMIAR = ActiveCarListParameters[29],
                        OPONY_ZIMOWE = ActiveCarListParameters[30],
                        KOSZT_TRANS = ActiveCarListParameters[31],
                        Driver = null
                    }));

                }

            }

            OracleDataReader AllNonActiveCarsReader = SelectAllNonActiveCarsCommand.ExecuteReader();
            List<string?> NonActiveCarListParameters = new List<string?>();
            while (AllNonActiveCarsReader.Read())
            {
                NonActiveCarListParameters.Clear();
                for (int i = 0; i < AllNonActiveCarsReader.FieldCount; i++)
                {
                    if (!(AllNonActiveCarsReader.IsDBNull(i)))
                    {
                        NonActiveCarListParameters.Add(AllNonActiveCarsReader.GetString(i));
                    }
                    else
                    {
                        NonActiveCarListParameters.Add(null);
                    }
                }

                
                AllNonActiveCarListFromCompanyDb.Add(new Car(new CarDTOCompanyDb()
                {                   
                    NR_REJ = NonActiveCarListParameters[0],
                    NR_VIN = NonActiveCarListParameters[1],
                    MARKA = NonActiveCarListParameters[2],
                    RODZAJ_SAMOCH = NonActiveCarListParameters[3],
                    FIR_KOD = NonActiveCarListParameters[4],
                    FIR = NonActiveCarListParameters[5],
                    ROK_PROD = NonActiveCarListParameters[6],
                    DATA_ZAKUPU = NonActiveCarListParameters[7],
                   DAT_1_REJESTRACJI = NonActiveCarListParameters[8],
                    CENA_ZAKUPU = NonActiveCarListParameters[9],
                    WLASNY = NonActiveCarListParameters[10],
                    AKT = NonActiveCarListParameters[11],
                    DAT_WYCOFANIA = NonActiveCarListParameters[12],
                    KOM_WYCOFANIA = NonActiveCarListParameters[13],                   
                    BENZYNA = ActiveCarListParameters[14],
                    DIESEL = ActiveCarListParameters[15],
                    GAZ = ActiveCarListParameters[16],
                    Driver = null
                }));

            

                


            }


        }

        public void UpdateCarParamInDb(Car selectedCar)
        {
            //MessageBox.Show("companydbcontext - wywolano metode\n"+selectedCar.RegNum);
            MessageBox.Show("companydbcontext - wywolano metode, update do bazy:\n"+ SqlCommandsStaticClass.GetUpdateCarParamUpdateCommand(selectedCar));

            /*
            if (selectedCar != null)
            {
                try
                {
                    OracleCommand command1 = _OracleConnection.CreateCommand();
                   // command1.CommandText = $@"UPDATE ANDRZEJ.AT_SAMOCH_BAZA SET oso_kod=null WHERE nr_rej='CT8066T'";
                    command1.CommandText = SqlCommandsStaticClass.GetUpdateCarParamUpdateCommand(selectedCar);
                   // MessageBox.Show(command1.CommandText);
                    command1.ExecuteNonQuery();
                    MessageBox.Show("Zaktualizowano dane");
                }

                catch (OracleException ex)
                {
                    MessageBox.Show("Record is not inserted into the database table.\nsqlstate: " + ex.Errors + "\nnumber: " + ex.Number);
                    MessageBox.Show("Exception Message: " + ex.Message);
                    // MessageBox.Show("Exception Source: " + ex.Source);


                }

            }
            */
}


        public ObservableCollection<Car> GetAllActiveCarListFromCompanyDb()
        {
            return AllActiveCarListFromCompanyDb;
        }
          public ObservableCollection<Car> GetAllNonActiveCarListFromCompanyDb()
        {
            return AllNonActiveCarListFromCompanyDb;
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
        #endregion
    }


}


