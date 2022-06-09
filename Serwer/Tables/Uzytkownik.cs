using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Serwer
{
    [Table("uzytkownicy")]
    public class Uzytkownik
    {
        [Key]
        public int id { get; set; }
        public string login { get; set; }
        public string haslo { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public string email { get; set; }
        public string data_ur { get; set; }
        public string uprawnienia { get; set; }
        [ForeignKey("id_u")]
        public virtual ICollection<Ogloszenie> OgloszeniaUzytkownika { get; set; }
        public override string ToString()
        {
            return "Uzytkownik - " + imie + " " + nazwisko;
        }
    }
}
