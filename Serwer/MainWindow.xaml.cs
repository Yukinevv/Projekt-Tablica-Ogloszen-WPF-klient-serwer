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

namespace Serwer
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new MyContext())
            {
                try
                {
                    var ogl = context.Ogloszenia.ToArray();
                    //działa jeden do wielu - pole typu Uzytkownik o nazwie Uzytkownik z ktorego mozemy odczytac od razu login imie autora itd.
                    WynikBox.Text = $"Mamy {ogl.Length} ogłoszeń\n" + "Autor imie: " + ogl[0].Uzytkownik.imie + " o nicku: " + ogl[0].Uzytkownik.login;
                    //Console.WriteLine($"Mamy {cars[2].data_utw} ogłoszeń");
                }
                catch (Exception err)
                {
                    WynikBox.Text = $"{err.Message}";
                }
            }
        }

        private void TestButton2_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new MyContext())
            {
                var uzyt = context.Uzytkownicy.ToArray();
                WynikBox.Text = $"Mamy {uzyt.Length} użytkowników" + "\n" + uzyt[1] + "\nMa ogłoszeń: " + uzyt[1].OgloszeniaUzytkownika.Count.ToString()
                + "\nPierwsze ogloszenie tego uzytkownika ma tytul: " + uzyt[1].OgloszeniaUzytkownika[0].tytul;
                //Jezeli nie ma zdefiniowanych GridViewColumn to przypisuje override ToString() danej klasy, a jeżeli jest to wszystko ładnie przypisuje
                PrzykladLista.ItemsSource = uzyt[1].OgloszeniaUzytkownika;
            }
        }
    }
}