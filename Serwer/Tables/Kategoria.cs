using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Serwer
{
    [Table("kategoria")]
    public class Kategoria
    {
        [Key]
        public int id_k { get; set; }
        public string nazwa { get; set; }
        public int id_u { get; set; }
        public string data_utw { get; set; }
        [ForeignKey("id_u")]
        public virtual Uzytkownik Uzytkownik { get; set; }
        public override string ToString()
        {
            return "Kategoria: \"" + nazwa + "\"" + " utworzona - " + data_utw;
        }

    }
}
