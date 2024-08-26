using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard2.Model.Domain
{
    public class DTOForGetLocationsForCar
    {
        public DTOForGetLocationsForCar(string id, string regnum, string name, DateTime datefrom, DateTime dateto, int carparktime)
        {
            Id = id;
            Name = name;
            RegNum = regnum;
            DateFrom = datefrom;
            DateTo = dateto;
            CarParkTime = carparktime;

        }

     
        public string? NumberOfKilometres { get; set; }
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? RegNum { get; set; }

        public int CarParkTime { get; set; }
      public  DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
