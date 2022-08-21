using System.Windows;
using System.Windows.Input;

namespace Serwer
{
    public class MainWindowModelWidoku : BaseViewModel
    {
        public Visibility UruchomSerwerButtonVisibilityModelWidoku { get; set; } = Visibility.Visible;
        public bool WylaczSerwerButtonIsEnabledModelWidoku { get; set; } = false;

        public ICommand UruchomSerwerKomenda { get; set; }
        public ICommand WylaczSerwerKomenda { get; set; }

        public MainWindowModelWidoku()
        {
            UruchomSerwerKomenda = new RelayCommand(UruchomSerwer);
            WylaczSerwerKomenda = new RelayCommand(WylaczSerwer);

            StartoweDane startoweDane = new StartoweDane(DataBaseLocator.Context);
            startoweDane.DodajStartoweDane();
        }

        private void UruchomSerwer(object x)
        {
            OperacjeSerwer.SerwerOperacje(x);

            UruchomSerwerButtonVisibilityModelWidoku = Visibility.Hidden;
            WylaczSerwerButtonIsEnabledModelWidoku = true;

            OnPropertyChanged(nameof(UruchomSerwerButtonVisibilityModelWidoku));
            OnPropertyChanged(nameof(WylaczSerwerButtonIsEnabledModelWidoku));
        }

        private void WylaczSerwer(object x)
        {
            OperacjeSerwer.serverSocket.Close();
            MessageBox.Show("Serwer zostal wylaczony!");
            Application.Current.Shutdown();
        }
    }
}
