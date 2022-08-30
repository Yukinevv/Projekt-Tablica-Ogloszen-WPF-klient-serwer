using System;
using System.Windows;

namespace Klient
{
    /// <summary>
    /// Klasa pomocnicza, ktora rozszerza klase Komentarz o wlasnosci CzyZaznaczony, Login oraz CheckBoxWidocznosc. Jest ona wykorzystywana jako typ
    /// do ObservableCollection aby w ItemsControl gdzie zostaja wypisane komentarze, mozna bylo wypisac login wlasciciela komentarza oraz zaznaczyc
    /// wybrane komentarze do usuniecia czy edycji
    /// </summary>
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
