using System.Configuration;
using System.Data;
using System.Windows;
using Dashboard2.ViewModel;
using Dashboard2.View;

namespace Dashboard2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application 
    {
        
       protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new MainWindow();
            //MessageBox.Show("1");
           MainWindow.DataContext= new MainViewModel();
         MainWindow.Show();

        }
      
    }

}
