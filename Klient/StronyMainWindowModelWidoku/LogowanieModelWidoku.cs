using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Klient
{
    public class LogowanieModelWidoku : BaseViewModel
    {
        public static string TextBoxLoginTextModelWidoku { get; set; } = string.Empty;

        public bool CheckBoxZapamietajIsCheckedModelWidoku { get; set; }

        public static Visibility PolaczZSerweremButtonVisibilityModelWidoku { get; set; } = Visibility.Hidden;

        public ICommand PolaczZSerweremKomenda { get; set; }
        public ICommand PrzejdzDoRejestracjiKomenda { get; set; }
        public ICommand ZalogujKomenda { get; set; }

        private static string[] daneLogowania = new string[2];

        public LogowanieModelWidoku(object x)
        {
            PolaczZSerweremKomenda = new RelayCommand(OperacjeKlient.PolaczZSerwerem);
            PrzejdzDoRejestracjiKomenda = new RelayCommand(PrzejdzDoRejestracji);
            ZalogujKomenda = new RelayCommand(Zaloguj);

            // jezeli nie jestesmy polaczeni z serwerem to pokazuje przycisk polaczenia
            if (!OperacjeKlient.clientSocket.Connected)
            {
                PolaczZSerweremButtonVisibilityModelWidoku = Visibility.Visible;
            }

            // wczytuje dane logowania z pamieci podrecznej
            daneLogowania = Ustawienia.WczytajDaneLogowania();

            TextBoxLoginTextModelWidoku = daneLogowania[0];
            (x as PasswordBox).Password = daneLogowania[1];
        }    

        private void PrzejdzDoRejestracji(object x)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Brak polaczenia z serwerem! Przepraszamy za utrudnienia!");
                return;
            }
            MainWindow.Rama.Content = new Rejestracja();
        }

        private void Zaloguj(object x)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Brak polaczenia z serwerem! Przepraszamy za utrudnienia!");
                return;
            }

            var parametry = (object[])x;
            TextBoxLoginTextModelWidoku = (string)parametry[0];
            string haslo = (parametry[1] as PasswordBox).Password;

            if (TextBoxLoginTextModelWidoku == string.Empty || haslo == string.Empty)
            {
                MessageBox.Show("Uzupelnij wszystkie pola!");
                return;
            }

            if (CheckBoxZapamietajIsCheckedModelWidoku == true)
            {
                Ustawienia.ZapiszDaneLogowania(TextBoxLoginTextModelWidoku, haslo);
            }

            // hashuje podane przez uzytkownika haslo
            SHA256 sha256Hash = SHA256.Create();
            string hash = RejestracjaModelWidoku.GetHash(sha256Hash, haslo);

            OperacjeKlient.Wyslij("LOGOWANIE");
            OperacjeKlient.Wyslij(TextBoxLoginTextModelWidoku);
            if (OperacjeKlient.Odbierz() != "OK") return;
            OperacjeKlient.Wyslij(hash);
            string czyZalogowac = OperacjeKlient.Odbierz();

            if (czyZalogowac == "zaloguj")
            {
                MainWindow.Rama.Content = new StronaGlowna();
            }
            else if (czyZalogowac == "jest juz zalogowany")
            {
                MessageBox.Show("Uzytkownik o podanym loginie jest juz zalogowany!", "Blad logowania");
            }
            else if (czyZalogowac == "nie ma w bazie")
            {
                MessageBox.Show("Nieprawidlowe dane logowania!", "Blad logowania");
            }
        }
    }
}
