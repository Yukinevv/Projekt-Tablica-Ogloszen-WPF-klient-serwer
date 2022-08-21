using System.Windows.Controls;

namespace Klient
{
    /// <summary>
    /// Logika interakcji dla klasy MojeOgloszenia.xaml
    /// </summary>
    public partial class MojeOgloszenia : Page
    {
        public MojeOgloszenia()
        {
            InitializeComponent();

            DataContext = new MojeOgloszeniaModelWidoku();
        }
    }
}
