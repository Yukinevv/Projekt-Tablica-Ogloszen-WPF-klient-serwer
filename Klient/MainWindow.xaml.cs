using BibliotekaEncje.Encje;
using Newtonsoft.Json;
using System.Collections.Generic;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Frame rama;

        public MainWindow()
        {
            InitializeComponent();

            rama = Ramka;
            rama.Content = new Logowanie();

            OperacjeKlient.PolaczZSerwerem();

            // --------------------------roboczo------------------------------

            //OperacjeKlient.Wyslij("UZYTKOWNICY");

            // wszyscy na raz
            //string uzytkownicySerialized = OperacjeKlient.Odbierz();
            //var uzytkownicy = JsonConvert.DeserializeObject<List<Uzytkownik>>(uzytkownicySerialized);
            //foreach (var item in uzytkownicy)
            //{
            //    Logowanie.TextBoxInfo.Text += "Login: " + item.Login + "  Hasło: " + item.Haslo + "\n";
            //}   

            // pojedynczo kazdy uzytkownik
            //string odpowiedz = OperacjeKlient.Odbierz();
            //int ileUzytkownikow = int.Parse(odpowiedz);
            //for (int i = 0; i < ileUzytkownikow; i++)
            //{
            //    string uzytkownikSerialized = OperacjeKlient.Odbierz();
            //    var uzytkownik = JsonConvert.DeserializeObject<Uzytkownik>(uzytkownikSerialized);
            //    Logowanie.TextBoxInfo.Text += "Login: " + uzytkownik.Login + "  Hasło: " + uzytkownik.Haslo + "\n";
            //}

            // tylko jeden
            //string uzytkownikSerialized = OperacjeKlient.Odbierz();
            //var uzytkownik = JsonConvert.DeserializeObject<Uzytkownik>(uzytkownikSerialized);
            //Logowanie.TextBoxInfo.Text += "Login: " + uzytkownik.Login + "  Hasło: " + uzytkownik.Haslo + "\n";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OperacjeKlient.clientSocket.Close();
        }
    }
}
