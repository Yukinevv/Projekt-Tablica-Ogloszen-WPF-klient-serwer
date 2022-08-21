using System.Windows.Controls;

namespace Klient
{
    /// <summary>
    /// Logika interakcji dla klasy PanelAdminaUzytkownicy.xaml
    /// </summary>
    public partial class PanelAdminaUzytkownicy : Page
    {
        public PanelAdminaUzytkownicy()
        {
            InitializeComponent();

            DataContext = new PanelAdminaUzytkownicyModelWidoku();
        }
    }
}
