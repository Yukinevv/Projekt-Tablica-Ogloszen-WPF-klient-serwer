using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading.Tasks;

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
            PolaczZSerweremKomenda = new RelayCommand(PolaczZSerwerem);
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

        public static void PolaczZSerwerem(object x = null)
        {
            EndPoint serverAddress = new IPEndPoint(IPAddress.Loopback, 11111);
            try
            {
                OperacjeKlient.clientSocket.Connect(serverAddress);
                PolaczZSerweremButtonVisibilityModelWidoku = Visibility.Hidden;
                MainWindow.Rama.Content = new Logowanie();
            }
            catch (SocketException)
            {
                MessageBox.Show("BLAD: Polaczenie z serwerem nie zostalo nawiazane!");
                OperacjeKlient.clientSocket.Close();

                OperacjeKlient.clientSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            }
        }

        private void PrzejdzDoRejestracji(object x)
        {
            if (!OperacjeKlient.clientSocket.Connected)
            {
                MessageBox.Show("Brak polaczenia z serwerem! Przepraszamy za utrudnienia!");
                return;
            }
            MainWindow.Rama.Content = new Rejestracja();
        }

        private void Zaloguj(object x)
        {
            if (!OperacjeKlient.clientSocket.Connected)
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
            OperacjeKlient.Odbierz();
            OperacjeKlient.Wyslij(hash);
            string czyZalogowac = OperacjeKlient.Odbierz();

            if (czyZalogowac == "true")
            {
                MainWindow.Rama.Content = new StronaGlowna();
            }
            else
            {
                MessageBox.Show("Nieprawidlowe dane logowania!", "Blad logowania");
            }
        }
    }
}
