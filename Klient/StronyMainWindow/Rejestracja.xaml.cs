using System.Windows.Controls;

namespace Klient
{
    /// <summary>
    /// Logika interakcji dla klasy Rejestracja.xaml
    /// </summary>
    public partial class Rejestracja : Page
    {
        public Rejestracja()
        {
            InitializeComponent();

            DataContext = new RejestracjaModelWidoku();
        }
    }
}
