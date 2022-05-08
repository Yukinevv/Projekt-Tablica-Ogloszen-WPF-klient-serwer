using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt
{
    public class Uzytkownicy
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Haslo { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Email { get; set; }
        //public DateTime Data_ur { get; set; }
        public string Data_ur { get; set; }
        public string Uprawnienia { get; set; }
    }
}
