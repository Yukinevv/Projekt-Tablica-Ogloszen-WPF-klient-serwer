using BibliotekaEncje.Encje;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Klient
{
    /// <summary>
    /// Klasa robiaca za model widoku dla strony Rejestracja
    /// </summary>
    public class RejestracjaModelWidoku : BaseViewModel
    {
        public DateTime DatePickerDateEndModelWidoku { get; set; }
        public DateTime DatePickerDateStartModelWidoku { get; set; }
        public DateTime DatePickerSelectedDateModelWidoku { get; set; } = DateTime.Today;

        public ICommand PowrotKomenda { get; set; }
        public ICommand ZarejestrujKomenda { get; set; }

        public RejestracjaModelWidoku()
        {
            PowrotKomenda = new RelayCommand(Powrot);
            ZarejestrujKomenda = new RelayCommand(Zarejestruj);

            DatePickerDateEndModelWidoku = DateTime.Today;
            DatePickerDateStartModelWidoku = DateTime.Parse("1900-01-01");
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

            MainWindow.Rama.Content = new Logowanie();
        }

        private void Zarejestruj(object x)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Utracono polaczenie z serwerem! Aplikacja zostanie zamknieta.");
                OperacjeKlient.clientSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            var parametry = (object[])x;
            string imie = (string)parametry[0];
            string nazwisko = (string)parametry[1];
            string login = (string)parametry[2];
            string email = (string)parametry[3];
            DateTime data_ur = (DateTime)parametry[4];
            string haslo1 = (parametry[5] as PasswordBox).Password;
            string haslo2 = (parametry[6] as PasswordBox).Password;

            if (imie == string.Empty || nazwisko == string.Empty || login == string.Empty
                || email == string.Empty || haslo1 == string.Empty || haslo2 == string.Empty
                || data_ur.ToString() == string.Empty)
            {
                MessageBox.Show("Uzupelnij wszystkie pola!");
                return;
            }
            else if (haslo1 != haslo2)
            {
                MessageBox.Show("Podane hasła nie są takie same!");
                return;
            }
            else if (haslo1.Length < 8 || haslo2.Length > 20)
            {
                MessageBox.Show("Hasło powinno miec długość od 8 do 20 znaków");
                return;
            }
            else if (!email.Contains("@"))
            {
                MessageBox.Show("Nieprawidłowy format adresu Email");
                return;
            }

            var result = MessageBox.Show("Czy zarejestrowac na podane dane?", "Rejestracja uzytkownika",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            // hashuje podane przez uzytkownika haslo
            SHA256 sha256Hash = SHA256.Create();
            string hash = GetHash(sha256Hash, haslo1);

            OperacjeKlient.Wyslij("REJESTRACJA");
            Uzytkownik uzytkownik = new Uzytkownik()
            {
                Imie = imie,
                Nazwisko = nazwisko,
                Login = login,
                Email = email,
                Haslo = hash,
                Data_ur = data_ur,
                Uprawnienia = "uzytkownik"
            };
            string uzytkownikSerialized = JsonConvert.SerializeObject(uzytkownik, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeKlient.Wyslij(uzytkownikSerialized);

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "zarejestrowano")
            {
                MessageBox.Show("Udana rejestracja!\n Teraz mozesz sie zalogowac.");
                MainWindow.Rama.Content = new Logowanie();
            }
            else
            {
                MessageBox.Show(odpowiedz);
            }
        }

        public static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
