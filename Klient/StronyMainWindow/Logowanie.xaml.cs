using System.Windows.Controls;

namespace Klient
{
    /// <summary>
    /// Logika interakcji dla klasy StronaGlowna.xaml
    /// </summary>
    public partial class Logowanie : Page
    {
        public Logowanie()
        {
            InitializeComponent();

            DataContext = new LogowanieModelWidoku(PasswordBoxHaslo);
        }
    }
}
