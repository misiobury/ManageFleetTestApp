using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard2.Model.Domain
{
    public class Driver
    {
        private string _idperson;
        private string _name;
        private string _surname;
        public Driver(string idperson, string name, string surname)
        {
            _idperson = idperson;
            _name = name;
            _surname = surname;
        }

        public string IdPerson
        {
            get { return _idperson; }
        }

        public string Name
        {

            get { return _name; }
        } 
        public string Surnname
        {

            get { return _surname; }
        }
    }
}
