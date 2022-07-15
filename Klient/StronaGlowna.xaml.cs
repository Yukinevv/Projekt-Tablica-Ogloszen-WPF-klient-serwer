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
    /// Logika interakcji dla klasy StronaGlowna.xaml
    /// </summary>
    public partial class StronaGlowna : Page
    {
        public static ListView ListViewKat;

        public StronaGlowna()
        {
            InitializeComponent();

            ListViewKat = ListViewKategorie;
        }

        private void ListViewKategorie_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MainWindow.rama.Content = new StronaOgloszenia();

            int idKategorii = ListViewKat.SelectedIndex + 1;
            OperacjeKlient.Wyslij("OGLOSZENIA");
            OperacjeKlient.Wyslij(idKategorii.ToString());

            string oglSerialized = OperacjeKlient.Odbierz();
            var ogloszenia = JsonConvert.DeserializeObject<List<Ogloszenie>>(oglSerialized);

            StronaOgloszenia.ListViewOgl.ItemsSource = ogloszenia;
        }

        private void WylogujButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.rama.Content = new Logowanie();
        }
    }
}
