using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard2.Model.Domain
{
    public class ViasatClientObject
    {
        public ViasatClientObject(string id, string name)
        {
            Id = id;
            Name = name;

        }

        public ViasatClientObject(string id, string name, string numOfKilometers)
        {
            Id = id;
            NumberOfKilometres = numOfKilometers;

        }

        public string NumberOfKilometres { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
