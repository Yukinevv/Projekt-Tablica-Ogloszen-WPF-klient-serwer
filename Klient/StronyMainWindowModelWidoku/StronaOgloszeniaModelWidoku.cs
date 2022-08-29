using BibliotekaEncje.Encje;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Klient
{
    /// <summary>
    /// Klasa robiaca za model widoku dla StronaOgloszenia
    /// </summary>
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

        public static int idUzytkownika;

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
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Utracono polaczenie z serwerem! Aplikacja zostanie zamknieta.");
                OperacjeKlient.clientSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            MainWindow.Rama.Content = new StronaGlowna();
        }

        private void WyborOgloszenia(object x)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Utracono polaczenie z serwerem! Aplikacja zostanie zamknieta.");
                OperacjeKlient.clientSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            if (x == null) // x to SelectedItem z OgloszeniaLista
            {
                return;
            }

            var ogloszenie = x as Ogloszenie;

            // potrzebne do np. usuniecia wybranego ogloszenia
            idWybranegoOgloszenia = ogloszenie.Id;
            idUzytkownika = ogloszenie.UzytkownikId;

            // potrzebne do weryfikacji zmian przy edycji ogloszenia
            TytulWybranegoOgloszenia = ogloszenie.Tytul;
            TrescWybranegoOgloszenia = ogloszenie.Tresc;

            // potrzebne do edycji kategorii ogloszenia
            OperacjeKlient.Wyslij("WYBRANE NAZWY KATEGORII");
            OperacjeKlient.Wyslij(idWybranegoOgloszenia.ToString());

            string nazwyKategoriiSerialized = OperacjeKlient.Odbierz();
            NazwyWybranychKategoriiDoListBoxa = JsonConvert.DeserializeObject<List<string>>(nazwyKategoriiSerialized);

            MainWindow.Rama.Content = new EdycjaOgloszenia();
            EdycjaOgloszeniaModelWidoku.SkadWchodze = "ze strony ogloszenia";
        }
     
        private void PrzejdzDoDodajOgloszenie(object x)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Utracono polaczenie z serwerem! Aplikacja zostanie zamknieta.");
                OperacjeKlient.clientSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            MainWindow.Rama.Content = new DodawanieOgloszen();
            DodawanieOgloszenModelWidoku.SkadWchodze = "ze strony ogloszenia";
        }

        private void Sortuj(object x)
        {
            var tag = x.ToString();
            var ogloszenia = OgloszeniaKopia;

            if (posortowano == false)
            {
                if (tag == "Tytul")
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
                posortowano = true;
            }
            else
            {
                if (tag == "Tytul")
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
