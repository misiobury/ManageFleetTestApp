using Dashboard2.Model.Domain;
using Dashboard2.Model.Infrastructure.DataAccessLayer.CompanyDbPhysicalLayer;
using Dashboard2.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Globalization;

namespace Dashboard2.View.Fleet
{
    public partial class FleetPage : Page
    {

        FleetViewModel _currentDataContext;
     

        //=========================================================
        //                   CONSTRUCTOR
        //=========================================================
        public FleetPage()
        {
            InitializeComponent();
            FleetViewModel.EditCarViewModelHasCreated += ShowEditCarWindow;
        }




        //=========================================================
        //                   METHODS
        //=========================================================


        private void ShowEditCarWindow(object? sender, EventArgs e)
        {
            _currentDataContext = (FleetViewModel)this.DataContext;
            EditCarParamWindow editCarWindow = new EditCarParamWindow();

            editCarWindow.DataContext = _currentDataContext.EditCarParamVMList[_currentDataContext.EditCarParamVMList.Count-1];

            editCarWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            editCarWindow.Show();
        }



        

        private void EditCarWindowViewModelHasCreatedEvent(object sender, EventArgs e)
        {
            _currentDataContext = (FleetViewModel)this.DataContext;
            EditCarParamWindow editCarWindow = new EditCarParamWindow();

            while (_currentDataContext.EditCarViewModel==null)
            {
                Task.Delay(1000);
            }


            editCarWindow.DataContext = _currentDataContext.EditCarViewModel;

            editCarWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        }

      

    }

}