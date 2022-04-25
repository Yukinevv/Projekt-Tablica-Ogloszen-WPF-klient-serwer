using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt
{
    public class Ogloszenia
    {
        public int Id_o { get; set; }
        public int Id_u { get; set; }
        public string Tytul { get; set; }
        public string Kategoria { get; set; }
        public string Tresc { get; set; }
        //public DateTime Data_utw { get; set; }
        //public DateTime Data_ed { get; set; }
        public string Data_utw { get; set; }
        public string Data_ed { get; set; }

        public string Opis
        {
            get
            {
                return Id_o.ToString() + " " + Id_u.ToString() + Tytul + " " + Kategoria + " " + Tresc + " " +
                    Data_utw.ToString() + " " + Data_ed.ToString();
            }
        }
    }
}
