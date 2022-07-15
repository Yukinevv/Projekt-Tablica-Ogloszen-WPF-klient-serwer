using System;
using System.Collections.Generic;

namespace BibliotekaEncje.Encje
{
    public class Kategoria
    {
        public int Id { get; set; }

        public string Nazwa { get; set; }

        public DateTime Data_utw { get; set; }


        public int UzytkownikId { get; set; }

        public virtual Uzytkownik Uzytkownik { get; set; }

        public virtual List<OgloszenieKategoria> Ogloszenia { get; set; }
    }
}
