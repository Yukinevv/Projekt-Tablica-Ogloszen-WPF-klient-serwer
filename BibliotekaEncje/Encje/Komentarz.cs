using System;
using System.Collections.Generic;

namespace BibliotekaEncje.Encje
{
    public class Komentarz
    {
        public int Id { get; set; }

        public string Tresc { get; set; }


        public int UzytkownikId { get; set; }

        public virtual Uzytkownik Uzytkownik { get; set; }


        public int OgloszenieId { get; set; }

        public virtual Ogloszenie Ogloszenie { get; set; }
    }
}
