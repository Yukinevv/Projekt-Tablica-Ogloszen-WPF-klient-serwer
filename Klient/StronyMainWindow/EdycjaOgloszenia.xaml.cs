using System.Windows.Controls;

namespace Klient
{
    /// <summary>
    /// Logika interakcji dla klasy EdycjaOgloszenia.xaml
    /// </summary>
    public partial class EdycjaOgloszenia : Page
    {
        public EdycjaOgloszenia()
        {
            InitializeComponent();

            DataContext = new EdycjaOgloszeniaModelWidoku(ListBoxKategorie);
        }
    }
}
