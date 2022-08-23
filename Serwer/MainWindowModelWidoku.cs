using System.Windows;
using System.Windows.Input;

namespace Serwer
{
    public class MainWindowModelWidoku : BaseViewModel
    {
        public static MTObservableCollection<string> ListBoxPolaczeniKlienciModelWidoku { get; set; } = new MTObservableCollection<string>();

        public Visibility UruchomSerwerButtonVisibilityModelWidoku { get; set; } = Visibility.Visible;
        public bool WylaczSerwerButtonIsEnabledModelWidoku { get; set; } = false;

        public ICommand UruchomSerwerKomenda { get; set; }
        public ICommand WylaczSerwerKomenda { get; set; }

        public static bool czyWylaczSerwerWcisniety = false;

        public MainWindowModelWidoku()
        {
            UruchomSerwerKomenda = new RelayCommand(UruchomSerwer);
            WylaczSerwerKomenda = new RelayCommand(WylaczSerwer);

            StartoweDane startoweDane = new StartoweDane(DataBaseLocator.Context);
            startoweDane.DodajStartoweDane();
        }

        private void UruchomSerwer(object x)
        {
            OperacjeSerwer.SerwerStart();

            UruchomSerwerButtonVisibilityModelWidoku = Visibility.Hidden;
            WylaczSerwerButtonIsEnabledModelWidoku = true;
        }

        private void WylaczSerwer(object x)
        {
            bool parametr = (x != null) ? (bool)x : false;
            czyWylaczSerwerWcisniety = true;
            OperacjeSerwer.SerwerStop();
            if (parametr) MessageBox.Show("Serwer zostal wylaczony!");
            Application.Current.Shutdown();
        }
    }
}
