using BibliotekaEncje.Encje;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Logika interakcji dla klasy EdycjaOgloszenia.xaml
    /// </summary>
    public partial class EdycjaOgloszenia : Page
    {
        public static TextBox TextBoxTytulOgl;

        public static TextBox TextBoxTrescOgl;

        public EdycjaOgloszenia()
        {
            InitializeComponent();

            TextBoxTytulOgl = TextBoxTytul;
            TextBoxTrescOgl = TextBoxTresc;
        }

        private void PowrótButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.rama.Content = new StronaOgloszenia();

            OperacjeKlient.Wyslij("OGLOSZENIA");
            OperacjeKlient.Wyslij(StronaGlowna.idKategorii.ToString());

            string oglSerialized = OperacjeKlient.Odbierz();
            var ogloszenia = JsonConvert.DeserializeObject<List<Ogloszenie>>(oglSerialized);
            StronaOgloszenia.ListViewOgl.ItemsSource = ogloszenia;
        }

        private void UsunButton_Click(object sender, RoutedEventArgs e)
        {
            OperacjeKlient.Wyslij("USUNIECIE OGLOSZENIA");
            OperacjeKlient.Wyslij(TextBoxTytulOgl.Text);
            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "Usunieto")
            {
                MessageBox.Show("Ogloszenie zostalo usuniete (ze wszystkich kategorii)!");
                MainWindow.rama.Content = new StronaOgloszenia();

                OperacjeKlient.Wyslij("OGLOSZENIA");
                OperacjeKlient.Wyslij(StronaGlowna.idKategorii.ToString());

                string oglSerialized = OperacjeKlient.Odbierz();
                var ogloszenia = JsonConvert.DeserializeObject<List<Ogloszenie>>(oglSerialized);
                StronaOgloszenia.ListViewOgl.ItemsSource = ogloszenia;
            }
        }
    }
}
