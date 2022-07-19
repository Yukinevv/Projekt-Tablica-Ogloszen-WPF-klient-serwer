using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Serwer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            using (var context = new MyDbContext())
            {
                StartoweDane startoweDane = new StartoweDane(context);
                startoweDane.DodajStartoweDane();
            }
        }

        private void UruchomSerwerButton_Click(object sender, RoutedEventArgs e)
        {
            OperacjeSerwer.SerwerOperacje(ListBoxPolaczeniKlienci);
        }
    }
}
