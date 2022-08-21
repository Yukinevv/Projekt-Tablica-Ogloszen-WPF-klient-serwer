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
        public ObservableCollection<Kategoria> KategorieLista { get; set; } = new ObservableCollection<Kategoria>();

        private List<Kategoria> KategorieKopia = new List<Kategoria>();

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

            // sprawdzenie czy uzytkownik ma uprawnienia administratora
            OperacjeKlient.Wyslij("CZY ADMIN");
            OperacjeKlient.Wyslij(LogowanieModelWidoku.TextBoxLoginTextModelWidoku);
            czyAdmin = OperacjeKlient.Odbierz();
            if (czyAdmin == "nie admin")
            {
                UsunKategorieButtonVisibilityModelWidoku = Visibility.Hidden;
                PanelAdministracyjnyButtonVisibilityModelWidoku = Visibility.Hidden;
            }

            OperacjeKlient.Wyslij("KATEGORIE");
            string katSerialized = OperacjeKlient.Odbierz();
            var kategorie = JsonConvert.DeserializeObject<List<Kategoria>>(katSerialized);
            foreach (var kategoria in kategorie)
            {
                KategorieLista.Add(kategoria);

                KategorieKopia.Add(kategoria);
            }
        }

        public void WyborKategorii(object x)
        {
            if (x == null)
            {
                return;
            }

            Kategoria kategoria = x as Kategoria;
            idKategorii = kategoria.Id;

            MainWindow.Rama.Content = new StronaOgloszenia();
        }

        private void Wyloguj(object x)
        {
            MainWindow.Rama.Content = new Logowanie();
            if (CzyPanelAdminaOtwarty)
            {
                PanelAdmina.Close();
                CzyPanelAdminaOtwarty = false;
            }
        }

        private void DodajKategorie(object x)
        {
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
                var kategoria = new Kategoria
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
                if (tag == "Id")
                {
                    kategorie = kategorie.OrderBy(k => k.Id).ToList();
                }
                else if (tag == "Nazwa")
                {
                    kategorie = kategorie.OrderBy(k => k.Nazwa).ToList();
                }
                else if (tag == "Data_utw")
                {
                    kategorie = kategorie.OrderBy(k => k.Data_utw).ToList();
                }
                posortowano = true;
            }
            else
            {
                if (tag == "Id")
                {
                    kategorie = kategorie.OrderByDescending(k => k.Id).ToList();
                }
                else if (tag == "Nazwa")
                {
                    kategorie = kategorie.OrderByDescending(k => k.Nazwa).ToList();
                }
                else if (tag == "Data_utw")
                {
                    kategorie = kategorie.OrderByDescending(k => k.Data_utw).ToList();
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
            PanelAdmina = new PanelAdmina();
            PanelAdmina.Show();
            CzyPanelAdminaOtwarty = true;
        }

        private void PrzejdzDoMojProfil(object x)
        {
            MainWindow.Rama.Content = new MojProfil();
        }
    }
}
