using Dashboard2.Model.Domain;
using Dashboard2.ViewModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Dashboard2.View.Fleet
{
    /// <summary>
    /// Interaction logic for EditCarParamWindow.xaml
    /// </summary>
    public partial class EditCarParamWindow : Window
    {       
        public EditCarParamWindow()
        {           
            InitializeComponent();              
            EditCarParamViewModel.CloseWindowRequest += CloseWindowAction;       
        } 


        private void CloseWindowAction(object? sender, SaveParamCarEventArgs car  )
        {           
            var _datacontext = (EditCarParamViewModel)this.DataContext;
            if(car.Car.RegNum == _datacontext.SelectedCar.RegNum)
            this.Close();           
        }
        
   
    }
}
