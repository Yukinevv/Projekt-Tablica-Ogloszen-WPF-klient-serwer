using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Serwer
{
    [Table("ogloszenia")]
    public class Ogloszenie
    {
        [Key]
        public int id_o { get; set; }
        public int id_u { get; set; }
        public string tytul { get; set; }
        public string data_utw { get; set; }
        public string data_ed { get; set; }
        public string tresc { get; set; }
        [ForeignKey("id_u")]
        public virtual Uzytkownik Uzytkownik { get; set; }
        public virtual IList<Kattoogl> KategorieOgloszenia { get; set; }
        public override string ToString()
        {
            return "Ogloszenie \"" + tytul + "\"";
        }
    }
}
