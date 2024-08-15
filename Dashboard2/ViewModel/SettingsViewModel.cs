using Dashboard2.Model.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard2.ViewModel
{
    


    class SettingsViewModel : IPage
    {
        //============================================================
        //                    FIELDS
        //============================================================


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
        public SettingsViewModel(  )
        {
            this.PageTitle = "Ustawienia";
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
