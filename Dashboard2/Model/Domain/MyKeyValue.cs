using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard2.Model.Domain
{
    class MyKeyValue : INotifyPropertyChanged
    {

        public MyKeyValue(string key, bool value) 
        {
            _key = key;
            _value = value;
        }

        private string _key;
        public string Key { get { return _key; } set { _key=value; OnPropertyChanged("Key"); } }
       
        private bool _value;
        public bool Value { get { return _value; }  set { _value = value; OnPropertyChanged("Value"); } }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
