using System.Windows.Controls;

namespace Klient
{
    /// <summary>
    /// Logika interakcji dla klasy StronaOgloszenia.xaml
    /// </summary>
    public partial class StronaOgloszenia : Page
    {
        public StronaOgloszenia()
        {
            InitializeComponent();

            DataContext = new StronaOgloszeniaModelWidoku();      
        }
    }
}
