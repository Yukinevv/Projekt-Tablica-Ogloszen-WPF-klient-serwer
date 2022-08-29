using System;
using System.Windows;

namespace Klient
{
    public class KomentarzModelWidoku
    {
        public int Id { get; set; }

        public string Tresc { get; set; }

        public int UzytkownikId { get; set; }

        public int OgloszenieId { get; set; }

        public bool CzyZaznaczony { get; set; }

        public string Login { get; set; }

        public Visibility CheckBoxWidocznosc { get; set; }
    }
}
