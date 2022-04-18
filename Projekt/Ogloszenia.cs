using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt
{
    internal class Ogloszenia
    {
        public int Id_o { get; set; }
        public int Id_u { get; set; }
        public string Tytul { get; set; }
        public string Kategotia { get; set; }
        public string Tresc { get; set; }
        public DateTime Data_utw { get; set; }
        public DateTime Data_ed { get; set; }
    }
}
