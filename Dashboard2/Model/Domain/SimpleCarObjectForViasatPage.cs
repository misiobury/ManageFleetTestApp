using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard2.Model.Domain
{
    public class SimpleCarObjectForViasatPage
    {
        public string RowNum { get; set; }
        public string? RegNum { get; set; }
        public string? Owner { get; set; }
        public string? Id { get; set; }
      

        public string? MileageOfKilometers { get; set; }

        public SimpleCarObjectForViasatPage(string rownum, string reg,string id, string owner, string mileageOfKilometers)
        {
            RowNum = rownum;
            RegNum = reg;
            Id = id;
            Owner = owner;
            MileageOfKilometers = mileageOfKilometers;
        }
     
    }
}
