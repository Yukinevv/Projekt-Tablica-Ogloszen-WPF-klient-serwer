using BibliotekaEncje.Encje;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Klient
{
    /// <summary>
    /// Logika interakcji dla klasy MojProfil.xaml
    /// </summary>
    public partial class MojProfil : Page
    {
        private static string ImieKopia;
        private static string NazwiskoKopia;
        private static string LoginKopia;
        private static string EmailKopia;
        private static string DataUrodzeniaKopia;
        private static string HasloKopia;

        public MojProfil()
        {
            InitializeComponent();

            OperacjeKlient.Wyslij("MOJE DANE");
            OperacjeKlient.Wyslij(Logowanie.TextBoxLogowanie.Text);

            string uzytkownikSerialized = OperacjeKlient.Odbierz();
            var uzytkownik = JsonConvert.DeserializeObject<Uzytkownik>(uzytkownikSerialized);

            TextBoxImie.Text = uzytkownik.Imie;
            TextBoxNazwisko.Text = uzytkownik.Nazwisko;
            TextBoxLogin.Text = uzytkownik.Login;
            TextBoxEmail.Text = uzytkownik.Email;
            DatePicker1.SelectedDate = DateTime.Parse(uzytkownik.Data_ur.ToString());

            ImieKopia = uzytkownik.Imie;
            NazwiskoKopia = uzytkownik.Nazwisko;
            LoginKopia = uzytkownik.Login;
            EmailKopia = uzytkownik.Email;
            HasloKopia = uzytkownik.Haslo;
            DataUrodzeniaKopia = uzytkownik.Data_ur.ToString();
        }

        private void ZaktualizujDaneButton_Click(object sender, RoutedEventArgs e)
        {
            string hash = "";
            if (PassBox1.Password != string.Empty)
            {
                SHA256 sha256Hash = SHA256.Create();
                hash = Rejestracja.GetHash(sha256Hash, PassBox1.Password);
            }

            if (TextBoxImie.Text == string.Empty || TextBoxNazwisko.Text == string.Empty || TextBoxLogin.Text == string.Empty ||
                TextBoxEmail.Text == string.Empty || PassBox1.Password == string.Empty || DatePicker1.SelectedDate.ToString() == string.Empty)
            {
                MessageBox.Show("Uzupelnij wszystkie pola!");
                return;
            }
            else if (TextBoxImie.Text == ImieKopia && TextBoxNazwisko.Text == NazwiskoKopia && TextBoxLogin.Text == LoginKopia &&
                TextBoxEmail.Text == EmailKopia && hash == HasloKopia && DataUrodzeniaKopia == DatePicker1.SelectedDate.ToString())
            {
                MessageBox.Show("Nie dokonano zadnych zmian!");
                return;
            }
            else if (PassBox1.Password != PassBox2.Password)
            {
                MessageBox.Show("Podane hasła nie są takie same!");
                return;
            }

            OperacjeKlient.Wyslij("EDYCJA DANYCH UZYTKOWNIKA");          
            var uzytkownik = new Uzytkownik()
            {
                Imie = TextBoxImie.Text,
                Nazwisko = TextBoxNazwisko.Text,
                Login = TextBoxLogin.Text,
                Email = TextBoxEmail.Text,
                Haslo = hash,
                Data_ur = (DateTime)DatePicker1.SelectedDate
            };
            string uzytkownikSerialized = JsonConvert.SerializeObject(uzytkownik, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeKlient.Wyslij(uzytkownikSerialized);
            OperacjeKlient.Wyslij(Logowanie.TextBoxLogowanie.Text);

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "edytowano")
            {
                MessageBox.Show("Twoja dane zostaly zaktualizowane!");
                MainWindow.rama.Content = new MojProfil();
            }
            else
            {
                MessageBox.Show(odpowiedz);
                return;
            }
        }

        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.rama.Content = new StronaGlowna();
        }

        private void MojeOgloszeniaButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.rama.Content = new MojeOgloszenia();
        }
    }
}
