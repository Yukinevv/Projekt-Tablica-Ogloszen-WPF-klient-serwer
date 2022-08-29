using BibliotekaEncje.Encje;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;

namespace Klient
{
    /// <summary>
    /// Klasa robiaca za model widoku dla strony MojeOgloszenia
    /// </summary>
    public class MojeOgloszeniaModelWidoku : BaseViewModel
    {
        public ObservableCollection<Ogloszenie> OgloszeniaLista { get; set; } = new ObservableCollection<Ogloszenie>();

        private List<Ogloszenie> OgloszeniaKopia;

        public ICommand PowrotKomenda { get; set; }
        public ICommand WyborOgloszeniaKomenda { get; set; } 
        public ICommand PrzejdzDoDodawanieOgloszenKomenda { get; set; }
        public ICommand SortujKomenda { get; set; }
        public ICommand FiltrujKomenda { get; set; }

        private bool posortowano = false;

        public MojeOgloszeniaModelWidoku()
        {
            PowrotKomenda = new RelayCommand(Powrot);
            WyborOgloszeniaKomenda = new RelayCommand(WyborOgloszenia);
            PrzejdzDoDodawanieOgloszenKomenda = new RelayCommand(PrzejdzDoDodawanieOgloszen);
            SortujKomenda = new RelayCommand(Sortuj);
            FiltrujKomenda = new RelayCommand(Filtruj);

            OperacjeKlient.Wyslij("MOJE OGLOSZENIA");
            OperacjeKlient.Wyslij(LogowanieModelWidoku.TextBoxLoginTextModelWidoku);
            string oglSerialized = OperacjeKlient.Odbierz();
            var ogloszenia = JsonConvert.DeserializeObject<List<Ogloszenie>>(oglSerialized);
            foreach (var ogloszenie in ogloszenia)
            {
                OgloszeniaLista.Add(ogloszenie);
            }

            OgloszeniaKopia = ogloszenia;
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

            MainWindow.Rama.Content = new MojProfil();
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

            if (x == null)
            {
                return;
            }

            var ogloszenie = x as Ogloszenie;

            // potrzebne do np. usuniecia wybranego ogloszenia
            StronaOgloszeniaModelWidoku.idWybranegoOgloszenia = ogloszenie.Id;
            StronaOgloszeniaModelWidoku.idUzytkownika = ogloszenie.UzytkownikId;

            // potrzebne do weryfikacji zmian przy edycji ogloszenia
            StronaOgloszeniaModelWidoku.TytulWybranegoOgloszenia = ogloszenie.Tytul;
            StronaOgloszeniaModelWidoku.TrescWybranegoOgloszenia = ogloszenie.Tresc;

            // potrzebne do edycji kategorii ogloszenia
            OperacjeKlient.Wyslij("WYBRANE NAZWY KATEGORII");
            OperacjeKlient.Wyslij(StronaOgloszeniaModelWidoku.idWybranegoOgloszenia.ToString());

            string nazwyKategoriiSerialized = OperacjeKlient.Odbierz();
            StronaOgloszeniaModelWidoku.NazwyWybranychKategoriiDoListBoxa = JsonConvert.DeserializeObject<List<string>>(nazwyKategoriiSerialized);

            MainWindow.Rama.Content = new EdycjaOgloszenia();
            EdycjaOgloszeniaModelWidoku.SkadWchodze = "z moich ogloszen";   
        }

        private void PrzejdzDoDodawanieOgloszen(object x)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Utracono polaczenie z serwerem! Aplikacja zostanie zamknieta.");
                OperacjeKlient.clientSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            MainWindow.Rama.Content = new DodawanieOgloszen();
            DodawanieOgloszenModelWidoku.SkadWchodze = "z moich ogloszen";
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
