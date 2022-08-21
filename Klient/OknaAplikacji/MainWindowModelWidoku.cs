using System.Windows.Input;

namespace Klient
{
    public class MainWindowModelWidoku : BaseViewModel
    {
        public ICommand ZamkniecieOknaKomenda { get; set; }

        public MainWindowModelWidoku()
        {
            ZamkniecieOknaKomenda = new RelayCommand(ZamkniecieOkna);

            MainWindow.Rama.Content = new Logowanie();

            LogowanieModelWidoku.PolaczZSerwerem();
        }

        private void ZamkniecieOkna(object x)
        {
            OperacjeKlient.clientSocket.Close();
            if (StronaGlownaModelWidoku.CzyPanelAdminaOtwarty)
            {
                StronaGlownaModelWidoku.PanelAdmina.Close();
            }
        }
    }
}
