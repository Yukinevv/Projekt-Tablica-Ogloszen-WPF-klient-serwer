using System.Windows.Controls;

namespace Klient
{
    /// <summary>
    /// Logika interakcji dla klasy KomentarzeOgloszenia.xaml
    /// </summary>
    public partial class KomentarzeOgloszenia : Page
    {
        public KomentarzeOgloszenia()
        {
            InitializeComponent();

            DataContext = new KomentarzeOgloszeniaModelWidoku();
        }
    }
}
