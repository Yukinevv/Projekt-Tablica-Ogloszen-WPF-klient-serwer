using System;
using System.Collections.Generic;

namespace BibliotekaEncje.Encje
{
    public class Uzytkownik
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string Haslo { get; set; }

        public string Imie { get; set; }

        public string Nazwisko { get; set; }

        public string Email { get; set; }

        public DateTime Data_ur { get; set; }

        public string Uprawnienia { get; set; }


        public virtual List<Ogloszenie> Ogloszenia { get; set; } = new List<Ogloszenie>();

        public virtual List<Kategoria> Kategorie { get; set; } = new List<Kategoria>();

        public virtual List<Komentarz> Komentarze { get; set; } = new List<Komentarz>();
    }
}
