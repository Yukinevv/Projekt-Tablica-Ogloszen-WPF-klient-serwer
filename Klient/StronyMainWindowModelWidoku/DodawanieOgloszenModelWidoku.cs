﻿using BibliotekaEncje.Encje;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Klient
{
    /// <summary>
    /// Klasa robiaca za model widoku dla strony DodawanieOgloszen
    /// </summary>
    public class DodawanieOgloszenModelWidoku : BaseViewModel
    {
        public ObservableCollection<string> ListBoxKategorieModelWidoku { get; set; } = new ObservableCollection<string>();

        public string TextBoxWybraneKategorieModelWidoku { get; set; } = "WYBRANE KATEGORIE:\nBrak wybranych kategorii...";

        public ICommand PowrotKomenda { get; set; }
        public ICommand ZatwierdzKomenda { get; set; }
        public ICommand ZaktualizujTextBoxWybraneKategorieKomenda { get; set; }

        public static string SkadWchodze;

        public DodawanieOgloszenModelWidoku()
        {
            PowrotKomenda = new RelayCommand(Powrot);
            ZatwierdzKomenda = new RelayCommand(Zatwierdz);
            ZaktualizujTextBoxWybraneKategorieKomenda = new RelayCommand(ZaktualizujTextBoxWybraneKategorie);

            // wyswietlanie dostepnych kategorii w listboxie

            foreach (var kategoria in StronaGlownaModelWidoku.KategorieLista)
            {
                ListBoxKategorieModelWidoku.Add(kategoria.Nazwa);
            }
        }

        private void ZaktualizujTextBoxWybraneKategorie(object x)
        {
            var elementy = (System.Collections.IList)x;
            var wybraneKategorie = elementy.Cast<string>().ToList();

            TextBoxWybraneKategorieModelWidoku = string.Empty;
            TextBoxWybraneKategorieModelWidoku += "WYBRANE KATEGORIE:\n";

            foreach (var kategoria in wybraneKategorie)
            {
                TextBoxWybraneKategorieModelWidoku += kategoria + ", ";
            }

            if (wybraneKategorie.Count == 0) TextBoxWybraneKategorieModelWidoku += "Brak wybranych kategorii...";
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

        private void Zatwierdz(object x)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Utracono polaczenie z serwerem! Aplikacja zostanie zamknieta.");
                OperacjeKlient.clientSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            var parametry = (object[])x;
            string tytul = (string)parametry[0];
            string tresc = (string)parametry[1];
            var elementy = (System.Collections.IList)parametry[2];
            var wybraneKategorie = elementy.Cast<string>().ToList();

            if (tytul == string.Empty || tresc == string.Empty || wybraneKategorie.Count == 0)
            {
                MessageBox.Show("Uzupełnij wszystkie pola!");
                return;
            }
            var result = MessageBox.Show("Czy na pewno chcesz dodac te ogloszenie?", "Dodawanie ogloszenia",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            // wyslanie ogloszenia do dodania
            OperacjeKlient.Wyslij("DODANIE OGLOSZENIA");
            OperacjeKlient.Wyslij(LogowanieModelWidoku.TextBoxLoginTextModelWidoku);
            if (OperacjeKlient.Odbierz() != "OK") return;
            var ogloszenie = new Ogloszenie()
            {
                Tytul = tytul,
                Data_utw = DateTime.Now,
                Data_ed = DateTime.Now,
                Tresc = tresc
            };
            string oglSerialized = JsonConvert.SerializeObject(ogloszenie, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeKlient.Wyslij(oglSerialized);

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "dodano ogloszenie")
            {
                // wyslanie wybranych kategorii z listboxa
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

                string drugaOdpowiedz = OperacjeKlient.Odbierz();
                if (drugaOdpowiedz == "zakonczono dodawanie")
                {
                    MessageBox.Show("Ogłoszenie zostało dodane! Znajdziesz je w kategorii: " + String.Join(", ", nazwyWybranychKategorii));
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
