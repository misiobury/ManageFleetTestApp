using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard2.Model.Domain
{
    public class SimpleCarObjectForStartPage
    {
        public string? RegNum { get; set; }
        public string? Owner {  get; set; }
        public string? TechnicalInspectionDate { get; set; }

        public string? MileageOfKilometers { get; set; }

        public SimpleCarObjectForStartPage(string reg,  string owner, string techinspdate)
        {
            RegNum = reg;
            Owner = owner;
            TechnicalInspectionDate = techinspdate;
        } 
        public SimpleCarObjectForStartPage(string reg,  string owner, string techinspdate, string mileage)
        {
            RegNum = reg;
            Owner = owner;
            TechnicalInspectionDate = techinspdate;
            MileageOfKilometers = mileage;
        }
    }
}
