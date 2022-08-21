using System.Windows.Controls;

namespace Klient
{
    /// <summary>
    /// Logika interakcji dla klasy MojProfil.xaml
    /// </summary>
    public partial class MojProfil : Page
    {
        public MojProfil()
        {
            InitializeComponent();

            DataContext = new MojProfilModelWidoku();
        }
    }
}
