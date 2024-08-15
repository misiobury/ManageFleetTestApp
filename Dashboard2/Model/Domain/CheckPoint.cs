using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard2.Model.Domain
{
    public class CheckPoint : INotifyPropertyChanged
    {
        private readonly string _IdDevice;
        private readonly string _RegNum;

        private CheckPoint() { }
        public CheckPoint(string IdDevice, string RegNum)
        {
            _IdDevice = IdDevice;
            _RegNum = RegNum;
        }

        public string IdDevice
        {
            get { return _IdDevice; }
        }

        public string RegNum
        {
            get { return _RegNum; }
        }

        public double X { get; set; }
        public double Y { get; set; }

        public string LocalizationDescription { get; set; }

        public int Speed { get; set; }
        public int Odometer { get; set; }

        public TimeOnly DateTimeReading { get; set; }


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
