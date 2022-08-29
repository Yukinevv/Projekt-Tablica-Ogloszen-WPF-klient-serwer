using System;
using System.Collections.Generic;

namespace BibliotekaEncje.Encje
{
    public class Ogloszenie
    {
        public int Id { get; set; }

        public string Tytul { get; set; }

        public DateTime Data_utw { get; set; }

        public DateTime Data_ed { get; set; }

        public string Tresc { get; set; }


        public int UzytkownikId { get; set; }

        public virtual Uzytkownik Uzytkownik { get; set; }

        public virtual List<OgloszenieKategoria> Kategorie { get; set; }

        public virtual List<Komentarz> Komentarze { get; set; } = new List<Komentarz>();
    }
}