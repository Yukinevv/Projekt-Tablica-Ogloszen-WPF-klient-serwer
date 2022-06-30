using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Serwer
{
    [Table("kattoogl")]
    public class Kattoogl
    {
        [Key, Column(Order = 1)]
        public int id_o { get; set; }
        [Key, Column(Order = 2)]
        public int id_k { get; set; }
        public virtual Ogloszenie Ogloszenia { get; set; }
        public virtual Kategoria Kategorie { get; set; }
    }
}
