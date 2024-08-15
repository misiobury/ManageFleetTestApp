using Dashboard2.Model.Domain;
using Dashboard2.Model.Infrastructure.DataAccessLayer.CompanyDbPhysicalLayer;
using Dashboard2.Model.Infrastructure.DataAccessLayer.ViasatApiPhysicalLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard2.ViewModel 
{
    class StatisticsViewModel : IPage
    {
        //============================================================
        //                    FIELDS
        //============================================================
        private ViasatDbContext? ViasatApiDbContext { get; set; }
        private CompanyDatabaseDbContext? CompanyDatabaseDbContext { get; set; }
        private ObservableCollection<Car>? AllCarsList { get; set; }




        //PAGE TITLE
        private string? _pageTitle;
        public string PageTitle
        {
            get { return _pageTitle ?? null; }
            set { _pageTitle = value; OnPropertyChanged("PageTitle"); }
        }




        //==============================================================
        //                   CONSTRUCTOR
        //==============================================================
        public StatisticsViewModel(CompanyDatabaseDbContext companyDatabaseDbContext, ViasatDbContext? VViasatApiDbContext , ObservableCollection<Car> AllCarsList)
        {
            this.PageTitle = "Statystyki";
            this.ViasatApiDbContext = ViasatApiDbContext;
            this.CompanyDatabaseDbContext = companyDatabaseDbContext;
            this.AllCarsList = AllCarsList;

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
