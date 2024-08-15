using Dashboard2.View.Viasat;
using Dashboard2.View.Start;
using Dashboard2.View.Fleet;
using Dashboard2.View.CommunicationWindow;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dashboard2.ViewModel;
using Dashboard2.Model.Domain;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Dashboard2.Model.Infrastructure.Repositories.ViasatApi;

namespace Dashboard2.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //===========================================
        //                  FIELDS 
        //===========================================

        private MenuButton _MenuButton;
        private IconList iconList { get; set; }
        MainViewModel mainViewModel;
      
      

        //===========================================
        //                   CONSTRUCTOR
        //===========================================
        public MainWindow()
        {
            InitializeComponent();
        }



        //=========================================================================================================================
        //                   METHODS
        //=========================================================================================================================

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
 
        }



        private void SetIconsForButtons(IconList iconList)
        {    
        }

      
    }



    public class MenuButton
    {
       
        public Label label { get; set; }
        public Border border { get; set; }

        public Image Icon { get; set; }
        public Image ActiveIcon { get; set; }
        public Image NonActiveIcon { get; set; }

        public MenuButton(Label LabelOfButton, Border BorderOfButton, Image IconOfButton, Image SourceImageForActiveIconButton, Image SourceImageForNonActiveIconButton)
        {            
            this.label = LabelOfButton;
            this.border = BorderOfButton;
            this.Icon = IconOfButton;
            this.ActiveIcon = SourceImageForActiveIconButton;
            this.NonActiveIcon = SourceImageForNonActiveIconButton;
            
     
        }

    

    }

    public class IconList
    {
        public Image ProgramLogoIcon {  get; set; }
        public Image HomeBtnActive {  get; set; }
        public Image HomeBtnNonActive {  get; set; }
        public Image ViasatBtnActive { get; set; }
        public Image ViasatBtnNonActive { get; set; }
        public Image FleetBtnActive { get; set; }
        public Image FleetBtnNonActive { get; set; }
        public Image DriversBtnActive { get; set; }
        public Image DriversBtnNonActive { get; set; }
        public Image StatisticsBtnActive { get; set; }
        public Image StatisticsBtnNonActive { get; set; }
        public Image SettingsBtnActive { get; set; }
        public Image SettingsBtnNonActive { get; set; }
        public Image ExitBtnActive { get; set; }
        public Image ExitBtnNonActive { get; set; }



        public IconList()
        {
            this.ProgramLogoIcon = new Image();
            this.ProgramLogoIcon.Source = new BitmapImage(new Uri(@".\..\..\..\View\Start\Resources\Main_Program_Logo.png", UriKind.Relative));

            this.HomeBtnActive = new Image();
            this.HomeBtnActive.Source = new BitmapImage(new Uri(@".\..\..\..\View\Start\Resources\Home_btn_Active.png", UriKind.Relative));
            this.HomeBtnNonActive = new Image();
            this.HomeBtnNonActive.Source = new BitmapImage(new Uri(@".\..\..\..\View\Start\Resources\Home_btn_NonActive.png", UriKind.Relative));

            this.ViasatBtnActive = new Image();
            this.ViasatBtnActive.Source = new BitmapImage(new Uri(@".\..\..\..\View\Start\Resources\Viasat_btn_Active.png", UriKind.Relative));
            this.ViasatBtnNonActive = new Image();
            this.ViasatBtnNonActive.Source = new BitmapImage(new Uri(@".\..\..\..\View\Start\Resources\Viasat_btn_NonActive.png", UriKind.Relative));

            this.FleetBtnActive = new Image();
            this.FleetBtnActive.Source = new BitmapImage(new Uri(@".\..\..\..\View\Start\Resources\Fleet_btn_Active.png", UriKind.Relative));
            this.FleetBtnNonActive = new Image();
            this.FleetBtnNonActive.Source = new BitmapImage(new Uri(@".\..\..\..\View\Start\Resources\Fleet_btn_NonActive.png", UriKind.Relative));

            this.DriversBtnActive = new Image();
            this.DriversBtnActive.Source = new BitmapImage(new Uri(@".\..\..\..\View\Start\Resources\Drivers_btn_Active.png", UriKind.Relative));
            this.DriversBtnNonActive = new Image();
            this.DriversBtnNonActive.Source = new BitmapImage(new Uri(@".\..\..\..\View\Start\Resources\Drivers_btn_NonActive.png", UriKind.Relative));

            this.StatisticsBtnActive = new Image();
            this.StatisticsBtnActive.Source = new BitmapImage(new Uri(@".\..\..\..\View\Start\Resources\Statistics_btn_Active.png", UriKind.Relative));
            this.StatisticsBtnNonActive = new Image();
            this.StatisticsBtnNonActive.Source = new BitmapImage(new Uri(@".\..\..\..\View\Start\Resources\Statistics_btn_NonActive.png", UriKind.Relative));

            this.SettingsBtnActive = new Image();
            this.SettingsBtnActive.Source = new BitmapImage(new Uri(@".\..\..\..\View\Start\Resources\Settings_btn_Active.png", UriKind.Relative));
            this.SettingsBtnNonActive = new Image();
            this.SettingsBtnNonActive.Source = new BitmapImage(new Uri(@".\..\..\..\View\Start\Resources\Settings_btn_NonActive.png", UriKind.Relative));

            this.ExitBtnActive = new Image();
            this.ExitBtnActive.Source = new BitmapImage(new Uri(@".\..\..\..\View\Start\Resources\Exit_btn_Active.png", UriKind.Relative));
            this.ExitBtnNonActive = new Image();
            this.ExitBtnNonActive.Source = new BitmapImage(new Uri(@".\..\..\..\View\Start\Resources\Exit_btn_NonActive.png", UriKind.Relative));

        }
    }
}