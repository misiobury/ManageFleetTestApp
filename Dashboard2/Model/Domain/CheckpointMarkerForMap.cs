using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard2.Model.Domain
{
    public class CheckpointMarkerForMap
    {
        public CheckpointMarkerForMap(int id, TimeOnly startReadingTime, TimeOnly endReadingTime, string address, double x, double y)
        {
            Id = id;
            StartReadingTime = startReadingTime;
            EndReadingTime = endReadingTime;
            Address = address;
            X = x;
            Y = y;
        }

        public int Id { get; set; }
        public TimeOnly StartReadingTime { get; set; }
        public TimeOnly EndReadingTime { get; set; }
        public string Address { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

    }
}
