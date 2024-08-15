using Dashboard2.Model.Domain;
using Dashboard2.Model.Infrastructure.DataAccessLayer.ViasatApiPhysicalLayer;
using Dashboard2.Model.Infrastructure.Repositories.ViasatApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dashboard2.ViewModel
{
    public class ViasatViewModel : IPage
    {
        //==============================================================
        //                   PROPERTIES AND FIELDS
        //==============================================================
        public ObservableCollection<ViasatClientObject> GetClientObjectsCarList { get; set; }
        public ViasatDbContext viasatDbContext { get; set; }

        public ObservableCollection<Car> AllCarsList { get; set; }

        private string _pageTitle;
        public string PageTitle
        {
            get { return _pageTitle; }
            set { _pageTitle = value; OnPropertyChanged("PageTitle"); }
        }




        //=====================================================================
        //                   CONSTRUCTOR
        //=====================================================================
        public ViasatViewModel()
        { }

        public ViasatViewModel(ViasatDbContext viasatDbContext, ObservableCollection<Car> AllCarsList)
        {
          //  MessageBox.Show("jestem w konstruktorze ViasatVM");
            this.PageTitle = "ViasatPage!";
            this.AllCarsList = AllCarsList;

            Task<ObservableCollection<ViasatClientObject>> TaskGetClient = Task.Run(  () =>  viasatDbContext.GetClientObjects());
            GetClientObjectsCarList =  TaskGetClient.Result;


         //   MessageBox.Show("wychodze z konstruktora ViasatVM");

        }

        public void przypisz()
        {
            //GetClientObjectsRepository getClientObjectsRepository = new GetClientObjectsRepository(viasatDbContext);
            MessageBox.Show("po przypisaniu repository");
          //  GetClientObjectsCarList = getClientObjectsRepository.GetAllCars();
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
