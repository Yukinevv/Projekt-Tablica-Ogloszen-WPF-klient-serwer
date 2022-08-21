using BibliotekaEncje.Encje;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace Klient
{
    public class PanelAdminaMenuModelWidoku : BaseViewModel
    {
        public ObservableCollection<Uzytkownik> UzytkownicyLista { get; set; } = new ObservableCollection<Uzytkownik>();

        private List<Uzytkownik> UzytkownicyKopia;

        public static Uzytkownik WybranyUzytkownik { get; set; }

        public ICommand WyborUzytkownikaKomenda { get; set; }
        public ICommand SortujKomenda { get; set; }
        public ICommand FiltrujKomenda { get; set; }
   
        private bool posortowano = false;     

        public PanelAdminaMenuModelWidoku()
        {
            WyborUzytkownikaKomenda = new RelayCommand(WyborUzytkownika);
            SortujKomenda = new RelayCommand(Sortuj);
            FiltrujKomenda = new RelayCommand(Filtruj);

            OperacjeKlient.Wyslij("UZYTKOWNICY");
            OperacjeKlient.Wyslij(LogowanieModelWidoku.TextBoxLoginTextModelWidoku);
            string uzytkownicySerialized = OperacjeKlient.Odbierz();
            var uzytkownicy = JsonConvert.DeserializeObject<List<Uzytkownik>>(uzytkownicySerialized);
            uzytkownicy = uzytkownicy.OrderBy(u => u.Id).ToList();
            foreach (var uzytkownik in uzytkownicy)
            {
                UzytkownicyLista.Add(uzytkownik);
            }

            UzytkownicyKopia = uzytkownicy;
        }

        private void WyborUzytkownika(object x)
        {
            if (x == null)
            {
                return;
            }

            WybranyUzytkownik = x as Uzytkownik;

            PanelAdmina.Rama.Content = new PanelAdminaUzytkownicy();
        }

        private void Sortuj(object x)
        {
            var tag = x.ToString();
            var uzytkownicy = UzytkownicyKopia;

            if (posortowano == false)
            {
                if (tag == "Id")
                {
                    uzytkownicy = uzytkownicy.OrderBy(u => u.Id).ToList();
                }
                else if (tag == "Login")
                {
                    uzytkownicy = uzytkownicy.OrderBy(u => u.Login).ToList();
                }
                posortowano = true;
            }
            else
            {
                if (tag == "Id")
                {
                    uzytkownicy = uzytkownicy.OrderByDescending(u => u.Id).ToList();
                }
                else if (tag == "Login")
                {
                    uzytkownicy = uzytkownicy.OrderByDescending(u => u.Login).ToList();
                }
                posortowano = false;
            }

            if (UzytkownicyLista != null)
            {
                UzytkownicyLista.Clear();
            }
            foreach (var uzytkownik in uzytkownicy)
            {
                UzytkownicyLista.Add(uzytkownik);
            }
            UzytkownicyKopia = uzytkownicy;
        }

        private void Filtruj(object x)
        {
            var text = x as string;
            var uzytkownicy = UzytkownicyKopia;

            if (text == string.Empty)
            {
                uzytkownicy = UzytkownicyKopia;
            }
            else
            {
                uzytkownicy = uzytkownicy.Where(u => u.Login.Contains(text)).ToList();
            }

            if (UzytkownicyLista != null)
            {
                UzytkownicyLista.Clear();
            }
            foreach (var uzytkownik in uzytkownicy)
            {
                UzytkownicyLista.Add(uzytkownik);
            }
        }
    }
}
