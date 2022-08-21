using BibliotekaEncje.Encje;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Klient
{
    public class StronaOgloszeniaModelWidoku : BaseViewModel
    {
        public ObservableCollection<Ogloszenie> OgloszeniaLista { get; set; } = new ObservableCollection<Ogloszenie>();

        private List<Ogloszenie> OgloszeniaKopia = new List<Ogloszenie>();

        public ICommand PowrotKomenda { get; set; }

        public ICommand WyborOgloszeniaKomenda { get; set; }

        public ICommand PrzejdzDoDodajOgloszenieKomenda { get; set; }

        public ICommand SortujKomenda { get; set; }

        public ICommand FiltrujKomenda { get; set; }

        public static int idWybranegoOgloszenia;

        public static string TytulWybranegoOgloszenia;

        public static string TrescWybranegoOgloszenia;   

        public static List<string> NazwyWybranychKategoriiDoListBoxa;

        private bool posortowano = false;

        public StronaOgloszeniaModelWidoku()
        {
            PowrotKomenda = new RelayCommand(Powrot);
            WyborOgloszeniaKomenda = new RelayCommand(WyborOgloszenia);
            PrzejdzDoDodajOgloszenieKomenda = new RelayCommand(PrzejdzDoDodajOgloszenie);
            SortujKomenda = new RelayCommand(Sortuj);
            FiltrujKomenda = new RelayCommand(Filtruj);

            OperacjeKlient.Wyslij("OGLOSZENIA");
            OperacjeKlient.Wyslij(StronaGlownaModelWidoku.idKategorii.ToString());
            string oglSerialized = OperacjeKlient.Odbierz();
            var ogloszenia = JsonConvert.DeserializeObject<List<Ogloszenie>>(oglSerialized);
            foreach (var ogloszenie in ogloszenia)
            {
                OgloszeniaLista.Add(ogloszenie);

                OgloszeniaKopia.Add(ogloszenie);
            }
        }

        private void Powrot(object x)
        {
            MainWindow.Rama.Content = new StronaGlowna();
        }

        private void WyborOgloszenia(object x)
        {
            if (x == null) // x to SelectedItem z OgloszeniaLista
            {
                return;
            }

            // domyslnie ustawiam mozliwosc edycji, a pozniej bede sprawdzac czy uzytkownik moze edytowac ogloszenie / jest adminem
            EdycjaOgloszeniaModelWidoku.ZatwierdzEdycjeOgloszeniaButtonVisibilityModelWidoku = Visibility.Visible;
            EdycjaOgloszeniaModelWidoku.UsunOgloszenieButtonVisibilityModelWidoku = Visibility.Visible;
            EdycjaOgloszeniaModelWidoku.TextBoxTytulIsReadOnlyModelWidoku = false;
            EdycjaOgloszeniaModelWidoku.TextBoxTrescIsReadOnlyModelWidoku = false;

            var ogloszenie = x as Ogloszenie;

            // potrzebne do np. usuniecia wybranego ogloszenia
            idWybranegoOgloszenia = ogloszenie.Id;

            // potrzebne do edycji kategorii ogloszenia
            OperacjeKlient.Wyslij("WYBRANE NAZWY KATEGORII");
            OperacjeKlient.Wyslij(idWybranegoOgloszenia.ToString());
            string nazwyKategoriiSerialized = OperacjeKlient.Odbierz();
            NazwyWybranychKategoriiDoListBoxa = JsonConvert.DeserializeObject<List<string>>(nazwyKategoriiSerialized);

            MainWindow.Rama.Content = new EdycjaOgloszenia();
            EdycjaOgloszeniaModelWidoku.SkadWchodze = "ze strony ogloszenia";

            // uzupelnienie textboxow danymi ogloszenia      
            EdycjaOgloszeniaModelWidoku.TextBoxTytulTextModelWidoku = ogloszenie.Tytul;
            EdycjaOgloszeniaModelWidoku.TextBoxTrescTextModelWidoku = ogloszenie.Tresc;

            // potrzebne do weryfikacji zmian przy edycji ogloszenia
            TytulWybranegoOgloszenia = ogloszenie.Tytul;
            TrescWybranegoOgloszenia = ogloszenie.Tresc;

            // sprawdzenie czy uzytkownik jest wlascicielem wybranego ogloszenia lub czy jest adminem
            // jezeli NIE to ukrywam przyciski odpowiadajace za edycje i usuniecie ogloszenia
            int idUzytkownika = ogloszenie.UzytkownikId;
            OperacjeKlient.Wyslij("CZY MOZE EDYTOWAC");
            OperacjeKlient.Wyslij(LogowanieModelWidoku.TextBoxLoginTextModelWidoku);
            OperacjeKlient.Odbierz();
            OperacjeKlient.Wyslij(idUzytkownika.ToString());

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "NIE")
            {
                EdycjaOgloszeniaModelWidoku.ZatwierdzEdycjeOgloszeniaButtonVisibilityModelWidoku = Visibility.Hidden;
                EdycjaOgloszeniaModelWidoku.UsunOgloszenieButtonVisibilityModelWidoku = Visibility.Hidden;
                EdycjaOgloszeniaModelWidoku.TextBoxTytulIsReadOnlyModelWidoku = true;
                EdycjaOgloszeniaModelWidoku.TextBoxTrescIsReadOnlyModelWidoku = true;
            }
        }
     
        private void PrzejdzDoDodajOgloszenie(object x)
        {
            MainWindow.Rama.Content = new DodawanieOgloszen();
            DodawanieOgloszenModelWidoku.SkadWchodze = "ze strony ogloszenia";
        }

        private void Sortuj(object x)
        {
            var tag = x.ToString();
            var ogloszenia = OgloszeniaKopia;

            if (posortowano == false)
            {
                if (tag == "Id")
                {
                    ogloszenia = ogloszenia.OrderBy(o => o.Id).ToList();
                }
                else if (tag == "UzytkownikId")
                {
                    ogloszenia = ogloszenia.OrderBy(o => o.UzytkownikId).ToList();
                }
                else if (tag == "Tytul")
                {
                    ogloszenia = ogloszenia.OrderBy(o => o.Tytul).ToList();
                }
                else if (tag == "Data_utw")
                {
                    ogloszenia = ogloszenia.OrderBy(o => o.Data_utw).ToList();
                }
                else if (tag == "Data_ed")
                {
                    ogloszenia = ogloszenia.OrderBy(o => o.Data_ed).ToList();
                }
                else if (tag == "Tresc")
                {
                    ogloszenia = ogloszenia.OrderBy(o => o.Tresc).ToList();
                }
                posortowano = true;
            }
            else
            {
                if (tag == "Id")
                {
                    ogloszenia = ogloszenia.OrderByDescending(o => o.Id).ToList();
                }
                else if (tag == "UzytkownikId")
                {
                    ogloszenia = ogloszenia.OrderByDescending(o => o.UzytkownikId).ToList();
                }
                else if (tag == "Tytul")
                {
                    ogloszenia = ogloszenia.OrderByDescending(o => o.Tytul).ToList();
                }
                else if (tag == "Data_utw")
                {
                    ogloszenia = ogloszenia.OrderByDescending(o => o.Data_utw).ToList();
                }
                else if (tag == "Data_ed")
                {
                    ogloszenia = ogloszenia.OrderByDescending(o => o.Data_ed).ToList();
                }
                else if (tag == "Tresc")
                {
                    ogloszenia = ogloszenia.OrderByDescending(o => o.Tresc).ToList();
                }
                posortowano = false;
            }

            if (OgloszeniaLista != null)
            {
                OgloszeniaLista.Clear();
            }
            foreach (var ogloszenie in ogloszenia)
            {
                OgloszeniaLista.Add(ogloszenie);
            }
            OgloszeniaKopia = ogloszenia;
        }

        private void Filtruj(object x)
        {
            var text = x as string;
            var ogloszenia = OgloszeniaKopia;

            if (text == string.Empty)
            {
                ogloszenia = OgloszeniaKopia;
            }
            else
            {
                ogloszenia = ogloszenia.Where(o => o.Tytul.Contains(text)).ToList();
            }

            if (OgloszeniaLista != null)
            {
                OgloszeniaLista.Clear();
            }
            foreach (var ogloszenie in ogloszenia)
            {
                OgloszeniaLista.Add(ogloszenie);
            }
        }
    }
}
