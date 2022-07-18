using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Klient
{
    /// <summary>
    /// Logika interakcji dla klasy PanelAdmina.xaml
    /// </summary>
    public partial class PanelAdmina : Window
    {
        public static Frame rama;

        public PanelAdmina()
        {
            InitializeComponent();

            rama = Ramka;
            rama.Content = new PanelAdminaMenu();
        }    
    }
}
