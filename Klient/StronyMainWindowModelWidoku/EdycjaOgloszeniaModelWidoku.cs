using BibliotekaEncje.Encje;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Klient
{
    public class EdycjaOgloszeniaModelWidoku : BaseViewModel
    {
        public static string TextBoxTytulTextModelWidoku { get; set; }
        public static string TextBoxTrescTextModelWidoku { get; set; }

        public static bool TextBoxTytulIsReadOnlyModelWidoku { get; set; }
        public static bool TextBoxTrescIsReadOnlyModelWidoku { get; set; }

        public static Visibility ZatwierdzEdycjeOgloszeniaButtonVisibilityModelWidoku { get; set; }
        public static Visibility UsunOgloszenieButtonVisibilityModelWidoku { get; set; }

        public ICommand PowrotKomenda { get; set; }

        public ICommand UsunKomenda { get; set; }

        public ICommand ZatwierdzKomenda { get; set; }

        public static string SkadWchodze;

        public EdycjaOgloszeniaModelWidoku(object x)
        {
            var listBox = x as ListBox;
            PowrotKomenda = new RelayCommand(Powrot);
            UsunKomenda = new RelayCommand(Usun);
            ZatwierdzKomenda = new RelayCommand(Zatwierdz);

            // wyswietlanie dostepnych kategorii w listboxie
            OperacjeKlient.Wyslij("KATEGORIE");
            string katSerialized = OperacjeKlient.Odbierz();
            var kategorie = JsonConvert.DeserializeObject<List<Kategoria>>(katSerialized);
            foreach (var kat in kategorie)
            {
                listBox.Items.Add(kat.Nazwa);
            }

            // zaznaczenie kategorii wybranego ogloszenia
            foreach (var nazwa in StronaOgloszeniaModelWidoku.NazwyWybranychKategoriiDoListBoxa)
            {
                listBox.SelectedItems.Add(nazwa);
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

            if (SkadWchodze == "ze strony ogloszenia")
            {
                MainWindow.Rama.Content = new StronaOgloszenia();
            }
            else if (SkadWchodze == "z moich ogloszen")
            {
                MainWindow.Rama.Content = new MojeOgloszenia();
            }
        }

        private void Usun(object x)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Utracono polaczenie z serwerem! Aplikacja zostanie zamknieta.");
                OperacjeKlient.clientSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            var result = MessageBox.Show("Czy na pewno chcesz usunac te ogloszenie?", "Usuniecie ogloszenia",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            OperacjeKlient.Wyslij("USUNIECIE OGLOSZENIA");
            OperacjeKlient.Wyslij(StronaOgloszeniaModelWidoku.idWybranegoOgloszenia.ToString());
            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "Usunieto")
            {
                MessageBox.Show("Ogloszenie zostalo usuniete (ze wszystkich kategorii)!");
                if (SkadWchodze == "ze strony ogloszenia")
                {
                    MainWindow.Rama.Content = new StronaOgloszenia();
                }
                else if (SkadWchodze == "z moich ogloszen")
                {
                    MainWindow.Rama.Content = new MojeOgloszenia();
                }
            }
        }

        private void Zatwierdz(object x)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Utracono polaczenie z serwerem! Aplikacja zostanie zamknieta.");
                OperacjeKlient.clientSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            var elementy = (System.Collections.IList)x;
            var wybraneKategorie = elementy.Cast<string>().ToList();

            // sprawdzam czy nastapila zmiana w wyborze kategorii
            bool zmianaWKategoriach = false;
            if (wybraneKategorie.Count != StronaOgloszeniaModelWidoku.NazwyWybranychKategoriiDoListBoxa.Count)
            {
                zmianaWKategoriach = true;
            }
            else
            {
                foreach (var nazwa in StronaOgloszeniaModelWidoku.NazwyWybranychKategoriiDoListBoxa)
                {
                    bool czyZawiera = wybraneKategorie.Contains(nazwa);
                    if (!czyZawiera)
                    {
                        zmianaWKategoriach = true;
                    }
                }
            }

            if (TextBoxTytulTextModelWidoku == string.Empty || TextBoxTrescTextModelWidoku == string.Empty || wybraneKategorie.Count == 0)
            {
                MessageBox.Show("Uzupełnij wszystkie pola!");
                return;
            }
            else if (TextBoxTytulTextModelWidoku == StronaOgloszeniaModelWidoku.TytulWybranegoOgloszenia &&
                TextBoxTrescTextModelWidoku == StronaOgloszeniaModelWidoku.TrescWybranegoOgloszenia && zmianaWKategoriach == false)
            {
                MessageBox.Show("Nie zostały dokonane żadne zmiany!");
                return;
            }

            var result = MessageBox.Show("Czy na pewno chcesz edytowac te ogloszenie?", "Edycja ogloszenia",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            OperacjeKlient.Wyslij("EDYCJA OGLOSZENIA");
            // potrzebuje przeslac id, tytul i tresc ogloszenia, ale zeby nie wysylac trzech danych oddzielnie to wysylam jeden obiekt
            var ogloszenie = new Ogloszenie()
            {
                Id = StronaOgloszeniaModelWidoku.idWybranegoOgloszenia,
                Tytul = TextBoxTytulTextModelWidoku,
                Data_utw = DateTime.Now,
                Data_ed = DateTime.Now,
                Tresc = TextBoxTrescTextModelWidoku
            };
            string ogloszenieSerialized = JsonConvert.SerializeObject(ogloszenie, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeKlient.Wyslij(ogloszenieSerialized);

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "zedytowano ogloszenie")
            {
                // wyslanie nowo wybranych kategorii z listboxa
                var nazwyWybranychKategorii = new List<string>();
                foreach (var nazwa in wybraneKategorie)
                {
                    nazwyWybranychKategorii.Add(nazwa.ToString());
                }
                string nazwyWybranychKategoriiSerialized = JsonConvert.SerializeObject(nazwyWybranychKategorii, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                OperacjeKlient.Wyslij(nazwyWybranychKategoriiSerialized);

                string odpowiedz2 = OperacjeKlient.Odbierz();
                if (odpowiedz2 == "zakonczono edycje")
                {
                    MessageBox.Show("Ogloszenie zostalo zedytowane! Znajdziesz je w kategorii: " + String.Join(", ", nazwyWybranychKategorii));
                    if (SkadWchodze == "ze strony ogloszenia")
                    {
                        MainWindow.Rama.Content = new StronaOgloszenia();
                    }
                    else if (SkadWchodze == "z moich ogloszen")
                    {
                        MainWindow.Rama.Content = new MojeOgloszenia();
                    }
                }
            }
        }
    }
}
