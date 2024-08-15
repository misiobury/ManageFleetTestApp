using Dashboard2.Model.Domain;
using Dashboard2.Model.Infrastructure.DataAccessLayer.CompanyDbPhysicalLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard2.ViewModel
{
    class DriversViewModel : IPage
    {
        //============================================================
        //                    FIELDS
        //============================================================

        //DB CONTEXT
        //-----------------------------------------------------------
        private CompanyDatabaseDbContext CompanyDatabaseDbContext { get; set; }
        private ObservableCollection<Car> AllCarsList { get; set; }



        private string _pageTitle;
        public string PageTitle
        {
            get { return _pageTitle; }
            set { _pageTitle = value; OnPropertyChanged("PageTitle"); }
        }



        //==============================================================
        //                   CONSTRUCTOR
        //==============================================================
        public DriversViewModel(CompanyDatabaseDbContext CompanyDatabaseDbContext, ObservableCollection<Car> AllCarsList)
        {
            this.PageTitle = "Kierowcy";
            this.AllCarsList = AllCarsList;
            this.CompanyDatabaseDbContext = CompanyDatabaseDbContext;




        }



        //============================================================
        //                    COMMAND 
        //============================================================


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
