using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard2.Model.Domain
{
    public class ObjectOfRowInSummary
    {
        public ObjectOfRowInSummary() 
        {
            TimeInterval = "";
            StatusOfTimePoint = "";
            KilometersTraveled = "";
            Localization = "";
        }
       public string TimeInterval {  get; set; }
       public string StatusOfTimePoint { get; set; }
        public string KilometersTraveled { get; set; }
        public string Localization { get; set; }
    }
}
