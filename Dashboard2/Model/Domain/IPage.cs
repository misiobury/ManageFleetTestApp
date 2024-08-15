using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard2.Model.Domain
{
    interface IPage : INotifyPropertyChanged
    {
        string PageTitle { get; set; }
    }
}
