using System;

namespace Klient
{
    /// <summary>
    /// Klasa pomocnicza, ktora rozszerza klase Kategoria o wlasnosc IloscOgloszen. Jest ona wykorzystywana jako typ do ObservableCollection aby w kontrolce
    /// ListView gdzie zostaja wypisane kategorie, mozna bylo rowniez wypisac ilosc ogloszen danej kategorii
    /// </summary>
    public class KategoriaModelWidoku
    {
        public int Id { get; set; }

        public string Nazwa { get; set; }

        public DateTime Data_utw { get; set; }


        public int UzytkownikId { get; set; }

        public int IloscOgloszen { get; set; }
    }
}
