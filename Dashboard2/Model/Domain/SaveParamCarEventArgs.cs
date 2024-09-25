using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard2.Model.Domain
{
    public class SaveParamCarEventArgs : EventArgs
    {
        public SaveParamCarEventArgs(Car car)
        {
            Car = car;
        }

        public Car Car { get; set; }

    }
}
