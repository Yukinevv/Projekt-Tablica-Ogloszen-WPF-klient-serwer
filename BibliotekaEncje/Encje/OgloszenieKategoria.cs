using System;
using System.Collections.Generic;

namespace BibliotekaEncje.Encje
{
    public class OgloszenieKategoria
    {
        public int OgloszenieId { get; set; }

        public virtual Ogloszenie Ogloszenie { get; set; }


        public int KategoriaId { get; set; }

        public virtual Kategoria Kategoria { get; set; }
    }
}
