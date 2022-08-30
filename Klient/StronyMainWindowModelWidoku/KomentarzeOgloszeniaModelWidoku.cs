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
    /// Klasa robiaca za model widoku dla strony KomentarzeOgloszenia
    /// </summary>
    public class KomentarzeOgloszeniaModelWidoku : BaseViewModel
    {
        public static ObservableCollection<KomentarzModelWidoku> KomentarzeLista { get; set; } = new ObservableCollection<KomentarzModelWidoku>();

        public static string TextBoxTrescModelWidoku { get; set; } = string.Empty;

        public string TextBoxTytulOgloszeniaModelWidoku { get; set; }
        public string TextBoxTrescOgloszeniaModelWidoku { get; set; }

        public ICommand PowrotKomenda { get; set; }
        public ICommand DodajKomentarzKomenda { get; set; }
        public ICommand UsunKomentarzKomenda { get; set; }

        public KomentarzeOgloszeniaModelWidoku()
        {
            PowrotKomenda = new RelayCommand(PowrotKomentarz);
            DodajKomentarzKomenda = new RelayCommand(DodajKomentarz);
            UsunKomentarzKomenda = new RelayCommand(UsunKomentarz);

            TextBoxTytulOgloszeniaModelWidoku = StronaOgloszeniaModelWidoku.TytulWybranegoOgloszenia;
            TextBoxTrescOgloszeniaModelWidoku = StronaOgloszeniaModelWidoku.TrescWybranegoOgloszenia;

            Visibility CheckBoxVisibility = Visibility.Hidden;
            if (StronaGlownaModelWidoku.czyAdmin == "admin")
            {
                CheckBoxVisibility = Visibility.Visible;
            }

            // wyswietlenie komentarzy znajdujacych sie w bazie
            OperacjeKlient.Wyslij("KOMENTARZE");
            OperacjeKlient.Wyslij(StronaOgloszeniaModelWidoku.idWybranegoOgloszenia.ToString());

            string komentarzeSerialized = OperacjeKlient.Odbierz();
            var komentarze = JsonConvert.DeserializeObject<List<Komentarz>>(komentarzeSerialized);

            string loginySerialized = OperacjeKlient.Odbierz();
            var loginy = JsonConvert.DeserializeObject<string[]>(loginySerialized);

            if (KomentarzeLista != null)
            {
                KomentarzeLista.Clear();
            }

            for (int i = 0; i < komentarze.Count; i++)
            {
                KomentarzeLista.Add(new KomentarzModelWidoku
                {
                    Id = komentarze[i].Id,
                    Tresc = komentarze[i].Tresc,
                    UzytkownikId = komentarze[i].UzytkownikId,
                    OgloszenieId = komentarze[i].OgloszenieId,
                    Login = loginy[i],
                    CheckBoxWidocznosc = (loginy[i] == LogowanieModelWidoku.TextBoxLoginTextModelWidoku) ? Visibility.Visible : CheckBoxVisibility
                });
            }
        }

        private void PowrotKomentarz(object x)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Utracono polaczenie z serwerem! Aplikacja zostanie zamknieta.");
                OperacjeKlient.clientSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            MainWindow.Rama.Content = new EdycjaOgloszenia();
        }

        private void DodajKomentarz(object x)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Utracono polaczenie z serwerem! Aplikacja zostanie zamknieta.");
                OperacjeKlient.clientSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            if (TextBoxTrescModelWidoku == string.Empty)
            {
                MessageBox.Show("Komentarz nie moze byc pusty!");
                return;
            }

            var result = MessageBox.Show("Czy dodac nowy komentarz?", "Dodanie komentarza",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            // dodawanie komentarzy do ObservableCollection oraz do bazy
            OperacjeKlient.Wyslij("DODANIE KOMENTARZA");
            OperacjeKlient.Wyslij(LogowanieModelWidoku.TextBoxLoginTextModelWidoku);
            if (OperacjeKlient.Odbierz() != "OK") return;

            var komentarz = new Komentarz
            {
                Tresc = TextBoxTrescModelWidoku,
                OgloszenieId = StronaOgloszeniaModelWidoku.idWybranegoOgloszenia
            };

            string komentarzSerialized = JsonConvert.SerializeObject(komentarz, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeKlient.Wyslij(komentarzSerialized);

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "dodano")
            {
                MessageBox.Show("Dodano komentarz!");
                TextBoxTrescModelWidoku = string.Empty;
                MainWindow.Rama.Content = new KomentarzeOgloszenia();
            }
        }

        private void UsunKomentarz(object x)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Utracono polaczenie z serwerem! Aplikacja zostanie zamknieta.");
                OperacjeKlient.clientSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            if (!KomentarzeLista.Any(k => k.CzyZaznaczony == true))
            {
                MessageBox.Show("Nie zostal zaznaczony zaden komentarz do usuniecia!");
                return;
            }

            var result = MessageBox.Show("Czy usunac zaznaczone komentarze?", "Usuniecie komentarzy",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            OperacjeKlient.Wyslij("USUNIECIE KOMENTARZY");

            var idZaznaczonychKomentarzy = KomentarzeLista.Where(k => k.CzyZaznaczony == true).Select(k => k.Id).ToArray();

            string idZaznaczonychKomentarzySerialized = JsonConvert.SerializeObject(idZaznaczonychKomentarzy, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeKlient.Wyslij(idZaznaczonychKomentarzySerialized);

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "usunieto")
            {
                MessageBox.Show("Zaznaczone komentarze zostaly usuniete!");
                MainWindow.Rama.Content = new KomentarzeOgloszenia();
            }
        }
    }
}
