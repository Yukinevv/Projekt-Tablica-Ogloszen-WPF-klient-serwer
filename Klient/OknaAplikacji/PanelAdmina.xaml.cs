using System.Windows;
using System.Windows.Controls;

namespace Klient
{
    /// <summary>
    /// Logika interakcji dla klasy PanelAdmina.xaml
    /// </summary>
    public partial class PanelAdmina : Window
    {
        public static Frame Rama;

        public PanelAdmina()
        {
            InitializeComponent();

            Rama = Ramka;
            Rama.Content = new PanelAdminaMenu();
        }    
    }
}
