using System;

namespace Klient
{
    public class KategoriaModelWidoku
    {
        public int Id { get; set; }

        public string Nazwa { get; set; }

        public DateTime Data_utw { get; set; }


        public int UzytkownikId { get; set; }

        public int IloscOgloszen { get; set; }
    }
}
