using System.Windows.Controls;

namespace Klient
{
    /// <summary>
    /// Logika interakcji dla klasy DodawanieOgloszen.xaml
    /// </summary>
    public partial class DodawanieOgloszen : Page
    {
        public DodawanieOgloszen()
        {
            InitializeComponent();

            DataContext = new DodawanieOgloszenModelWidoku();
        }
    }
}
