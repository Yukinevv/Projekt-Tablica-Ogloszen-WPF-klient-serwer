using BibliotekaEncje.Encje;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Klient
{
    public class StronaGlownaModelWidoku : BaseViewModel
    {
        public static ObservableCollection<KategoriaModelWidoku> KategorieLista { get; set; } = new ObservableCollection<KategoriaModelWidoku>();

        private List<KategoriaModelWidoku> KategorieKopia = new List<KategoriaModelWidoku>();

        public string MojProfilButtonContentModelWidoku { get; set; }

        public string TextBoxNazwaNowejKategoriiTextModelWidoku { get; set; }

        public Visibility UsunKategorieButtonVisibilityModelWidoku { get; set; }

        public Visibility PanelAdministracyjnyButtonVisibilityModelWidoku { get; set; }

        public ICommand WyborKategoriiKomenda { get; set; }
        public ICommand WylogujKomenda { get; set; }
        public ICommand DodajKategorieKomenda { get; set; }
        public ICommand UsunKategorieKomenda { get; set; }
        public ICommand SortujKomenda { get; set; }
        public ICommand FiltrujKomenda { get; set; }
        public ICommand OtworzPanelAdminaKomenda { get; set; }
        public ICommand PrzejdzDoMojProfilKomenda { get; set; }

        public static int idKategorii;

        public static PanelAdmina PanelAdmina;

        public static bool CzyPanelAdminaOtwarty;

        public static string czyAdmin;

        private static bool posortowano = false;

        public StronaGlownaModelWidoku()
        {
            WyborKategoriiKomenda = new RelayCommand(WyborKategorii);
            WylogujKomenda = new RelayCommand(Wyloguj);
            DodajKategorieKomenda = new RelayCommand(DodajKategorie);
            UsunKategorieKomenda = new RelayCommand(UsunKategorie);
            SortujKomenda = new RelayCommand(Sortuj);
            FiltrujKomenda = new RelayCommand(Filtruj);
            OtworzPanelAdminaKomenda = new RelayCommand(OtworzPanelAdmina);
            PrzejdzDoMojProfilKomenda = new RelayCommand(PrzejdzDoMojProfil);

            MojProfilButtonContentModelWidoku = "Profil " + LogowanieModelWidoku.TextBoxLoginTextModelWidoku;

            OperacjeKlient.Wyslij("DODAJ LOGIN i SPRAWDZ CZY ADMIN i POBIERZ KATEGORIE");
            // DODAJ LOGIN w celu zabezpieczenia aby drugi klient nie mogl zalogowac sie na to same konto
            // SPRAWDZ CZY ADMIN - sprawdzenie czy uzytkownik ma uprawnienia administratora

            OperacjeKlient.Wyslij(LogowanieModelWidoku.TextBoxLoginTextModelWidoku);

            czyAdmin = OperacjeKlient.Odbierz();
            if (czyAdmin == "nie admin")
            {
                UsunKategorieButtonVisibilityModelWidoku = Visibility.Hidden;
                PanelAdministracyjnyButtonVisibilityModelWidoku = Visibility.Hidden;
            }

            string katSerialized = OperacjeKlient.Odbierz();
            var kategorie = JsonConvert.DeserializeObject<List<Kategoria>>(katSerialized);

            string iloscOgloszenWDanychKategoriach = OperacjeKlient.Odbierz();
            int[] ilosciOgloszen = JsonConvert.DeserializeObject<int[]>(iloscOgloszenWDanychKategoriach);

            if (KategorieLista != null)
            {
                KategorieLista.Clear();
            }
            for (int i = 0; i < kategorie.Count; i++)
            {
                KategorieLista.Add(new KategoriaModelWidoku
                {
                    Id = kategorie[i].Id,
                    Nazwa = kategorie[i].Nazwa,
                    Data_utw = kategorie[i].Data_utw,
                    UzytkownikId = kategorie[i].UzytkownikId,
                    IloscOgloszen = ilosciOgloszen[i]
                });

                KategorieKopia.Add(KategorieLista[i]);
            }          
        }

        public void WyborKategorii(object x)
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

            KategoriaModelWidoku kategoria = x as KategoriaModelWidoku;
            idKategorii = kategoria.Id;

            MainWindow.Rama.Content = new StronaOgloszenia();
        }

        private void Wyloguj(object x)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Utracono polaczenie z serwerem! Aplikacja zostanie zamknieta.");
                OperacjeKlient.clientSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            OperacjeKlient.Wyslij("WYLOGUJ");
            OperacjeKlient.Wyslij(LogowanieModelWidoku.TextBoxLoginTextModelWidoku);

            MainWindow.Rama.Content = new Logowanie();
            if (CzyPanelAdminaOtwarty)
            {
                PanelAdmina.Close();
                CzyPanelAdminaOtwarty = false;
            }
        }

        private void DodajKategorie(object x)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Utracono polaczenie z serwerem! Aplikacja zostanie zamknieta.");
                OperacjeKlient.clientSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            if (TextBoxNazwaNowejKategoriiTextModelWidoku == string.Empty || TextBoxNazwaNowejKategoriiTextModelWidoku == null)
            {
                MessageBox.Show("Nie wpisales nazwy kategorii!");
                return;
            }
            var result = MessageBox.Show("Czy na pewno chcesz dodac te kategorie?", "Dodawanie kategorii",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            OperacjeKlient.Wyslij("DODANIE KATEGORII");
            var nowaKategoria = new Kategoria()
            {
                Nazwa = TextBoxNazwaNowejKategoriiTextModelWidoku,
                Data_utw = DateTime.Now,
            };
            string kategoriaSerialized = JsonConvert.SerializeObject(nowaKategoria, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeKlient.Wyslij(kategoriaSerialized);
            OperacjeKlient.Wyslij(LogowanieModelWidoku.TextBoxLoginTextModelWidoku);

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "Dodano")
            {
                MessageBox.Show("Dodano nowa kategorie o nazwie: " + TextBoxNazwaNowejKategoriiTextModelWidoku);

                // dodaje kategorie do ObservableCollection KategorieLista
                var kategoria = new KategoriaModelWidoku
                {
                    Nazwa = TextBoxNazwaNowejKategoriiTextModelWidoku,
                    Data_utw = DateTime.Now
                };
                KategorieLista.Add(kategoria);
            }
            else
            {
                MessageBox.Show(odpowiedz);
            }
        }

        private void UsunKategorie(object x)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Utracono polaczenie z serwerem! Aplikacja zostanie zamknieta.");
                OperacjeKlient.clientSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            if (TextBoxNazwaNowejKategoriiTextModelWidoku == string.Empty || TextBoxNazwaNowejKategoriiTextModelWidoku == null)
            {
                MessageBox.Show("Nie wpisales nazwy kategorii!");
                return;
            }
            var result = MessageBox.Show("Czy na pewno chcesz usunac te kategorie?", "Usuniecie kategorii",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            OperacjeKlient.Wyslij("USUNIECIE KATEGORII");
            OperacjeKlient.Wyslij(TextBoxNazwaNowejKategoriiTextModelWidoku);

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "usunieto")
            {
                MessageBox.Show("Kategoria zostala usunieta!");

                // usuniecie kategorii z ObservableCollection KategorieLista
                var kategoriaDoUsuniecia = KategorieLista.FirstOrDefault(x => x.Nazwa == TextBoxNazwaNowejKategoriiTextModelWidoku);
                KategorieLista.Remove(kategoriaDoUsuniecia);
            }
            else if (odpowiedz == "nie usunieto")
            {
                MessageBox.Show("W kategorii znajduja sie ogloszenia. Nie moze zostac usunieta!");
            }
            else
            {
                MessageBox.Show("Taka kategoria nie istnieje! Prosze sprawdzic poprawnosc wpisanej nazwy.");
            }
        }

        private void Sortuj(object x)
        {
            var tag = x.ToString();
            var kategorie = KategorieKopia;

            if (posortowano == false)
            {
                if (tag == "Nazwa")
                {
                    kategorie = kategorie.OrderBy(k => k.Nazwa).ToList();
                }
                else if (tag == "IloscOgloszen")
                {
                    kategorie = kategorie.OrderBy(k => k.IloscOgloszen).ToList();
                }
                posortowano = true;
            }
            else
            {
                if (tag == "Nazwa")
                {
                    kategorie = kategorie.OrderByDescending(k => k.Nazwa).ToList();
                }
                else if (tag == "IloscOgloszen")
                {
                    kategorie = kategorie.OrderByDescending(k => k.IloscOgloszen).ToList();
                }
                posortowano = false;
            }

            if (KategorieLista != null)
            {
                KategorieLista.Clear();
            }
            foreach (var kategoria in kategorie)
            {
                KategorieLista.Add(kategoria);
            }
            KategorieKopia = kategorie;
        }

        private void Filtruj(object x)
        {
            var text = x as string;
            var kategorie = KategorieKopia;

            if (text == string.Empty)
            {
                kategorie = KategorieKopia;
            }
            else
            {
                kategorie = kategorie.Where(k => k.Nazwa.Contains(text)).ToList();
            }

            if (KategorieLista != null)
            {
                KategorieLista.Clear();
            }
            foreach (var kategoria in kategorie)
            {
                KategorieLista.Add(kategoria);
            }
        }

        private void OtworzPanelAdmina(object x)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Utracono polaczenie z serwerem! Aplikacja zostanie zamknieta.");
                OperacjeKlient.clientSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            PanelAdmina = new PanelAdmina();
            PanelAdmina.Show();
            CzyPanelAdminaOtwarty = true;
        }

        private void PrzejdzDoMojProfil(object x)
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
    }
}
