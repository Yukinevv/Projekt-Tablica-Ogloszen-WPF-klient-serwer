using BibliotekaEncje.Encje;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Klient
{
    public class MojProfilModelWidoku : BaseViewModel
    {
        public DateTime DatePickerDateEndModelWidoku { get; set; }
        public DateTime DatePickerDateStartModelWidoku { get; set; }

        public string TextBoxImieModelWidoku { get; set; }
        public string TextBoxNazwiskoModelWidoku { get; set; }
        public string TextBoxLoginModelWidoku { get; set; }
        public string TextBoxEmailModelWidoku { get; set; }
        public DateTime DatePickerSelectedDateModelWidoku { get; set; }

        public ICommand ZaktualizujKomenda { get; set; }
        public ICommand PowrotKomenda { get; set; }
        public ICommand PrzejdzDoMojeOgloszeniaKomenda { get; set; }

        private Uzytkownik UzytkownikKopia;

        public MojProfilModelWidoku()
        {
            ZaktualizujKomenda = new RelayCommand(Zaktualizuj);
            PowrotKomenda = new RelayCommand(Powrot);
            PrzejdzDoMojeOgloszeniaKomenda = new RelayCommand(PrzejdzDoMojeOgloszenia);

            DatePickerDateEndModelWidoku = DateTime.Today;
            DatePickerDateStartModelWidoku = DateTime.Parse("1900-01-01");

            OperacjeKlient.Wyslij("MOJE DANE");
            OperacjeKlient.Wyslij(LogowanieModelWidoku.TextBoxLoginTextModelWidoku);

            string uzytkownikSerialized = OperacjeKlient.Odbierz();
            var uzytkownik = JsonConvert.DeserializeObject<Uzytkownik>(uzytkownikSerialized);
            UzytkownikKopia = uzytkownik;

            TextBoxImieModelWidoku = uzytkownik.Imie;
            TextBoxNazwiskoModelWidoku = uzytkownik.Nazwisko;
            TextBoxLoginModelWidoku = uzytkownik.Login;
            TextBoxEmailModelWidoku = uzytkownik.Email;
            DatePickerSelectedDateModelWidoku = uzytkownik.Data_ur;
        }

        private void Zaktualizuj(object x)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Utracono polaczenie z serwerem! Aplikacja zostanie zamknieta.");
                OperacjeKlient.clientSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            var parametry = (object[])x;
            string haslo1 = (parametry[0] as PasswordBox).Password;
            string haslo2 = (parametry[1] as PasswordBox).Password;

            if (haslo1 != haslo2)
            {
                MessageBox.Show("Podane hasła nie są takie same!");
                return;
            }

            string hash = "";
            if (haslo1 != string.Empty)
            {
                SHA256 sha256Hash = SHA256.Create();
                hash = RejestracjaModelWidoku.GetHash(sha256Hash, haslo1);
            }

            if (TextBoxImieModelWidoku == string.Empty || TextBoxNazwiskoModelWidoku == string.Empty || TextBoxLoginModelWidoku == string.Empty ||
                TextBoxEmailModelWidoku == string.Empty || haslo1 == string.Empty || DatePickerSelectedDateModelWidoku.ToString() == string.Empty)
            {
                MessageBox.Show("Uzupelnij wszystkie pola!");
                return;
            }
            else if (TextBoxImieModelWidoku == UzytkownikKopia.Imie && TextBoxNazwiskoModelWidoku == UzytkownikKopia.Nazwisko &&
                TextBoxLoginModelWidoku == UzytkownikKopia.Login && TextBoxEmailModelWidoku == UzytkownikKopia.Email && hash == UzytkownikKopia.Haslo &&
                UzytkownikKopia.Data_ur.ToString() == DatePickerSelectedDateModelWidoku.ToString())
            {
                MessageBox.Show("Nie dokonano zadnych zmian!");
                return;
            }
            else if (haslo1.Length < 8 || haslo1.Length > 20)
            {
                MessageBox.Show("Hasło powinno miec długość od 8 do 20 znaków");
                return;
            }
            else if (!TextBoxEmailModelWidoku.Contains("@"))
            {
                MessageBox.Show("Nieprawidłowy format adresu Email");
                return;
            }

            var result = MessageBox.Show("Czy na pewno chcesz edytowac swoje dane?", "Edycja danych uzytkownika",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            OperacjeKlient.Wyslij("EDYCJA DANYCH UZYTKOWNIKA");
            var uzytkownik = new Uzytkownik()
            {
                Imie = TextBoxImieModelWidoku,
                Nazwisko = TextBoxNazwiskoModelWidoku,
                Login = TextBoxLoginModelWidoku,
                Email = TextBoxEmailModelWidoku,
                Haslo = hash,
                Data_ur = (DateTime)DatePickerSelectedDateModelWidoku
            };
            string uzytkownikSerialized = JsonConvert.SerializeObject(uzytkownik, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeKlient.Wyslij(uzytkownikSerialized);
            OperacjeKlient.Wyslij(LogowanieModelWidoku.TextBoxLoginTextModelWidoku);

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "edytowano")
            {
                MessageBox.Show("Twoja dane zostaly zaktualizowane!");
                MainWindow.Rama.Content = new MojProfil();
            }
            else
            {
                MessageBox.Show(odpowiedz);
                return;
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

        private void PrzejdzDoMojeOgloszenia(object x)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Utracono polaczenie z serwerem! Aplikacja zostanie zamknieta.");
                OperacjeKlient.clientSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            MainWindow.Rama.Content = new MojeOgloszenia();
        }
    }
}
