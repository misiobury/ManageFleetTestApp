using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard2.Model.Domain
{
    public  class CompanyBranch
    {
        public CompanyBranch(string id, string name, string city, string address, double y, double x)
        {
            Id = id;
            Name = name;
            Address = address;
            City = city;
            X = x;
            Y = y;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}
