using System.Windows.Controls;

namespace Klient
{
    /// <summary>
    /// Logika interakcji dla klasy StronaGlowna.xaml
    /// </summary>
    public partial class StronaGlowna : Page
    {
        public StronaGlowna()
        {
            InitializeComponent();

            DataContext = new StronaGlownaModelWidoku();
        }
    }
}
