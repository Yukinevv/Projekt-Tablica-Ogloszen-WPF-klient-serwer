using System.Windows.Controls;

namespace Klient
{
    /// <summary>
    /// Logika interakcji dla klasy PanelAdminaMenu.xaml
    /// </summary>
    public partial class PanelAdminaMenu : Page
    {
        public PanelAdminaMenu()
        {
            InitializeComponent();

            DataContext = new PanelAdminaMenuModelWidoku();
        }
    }
}
