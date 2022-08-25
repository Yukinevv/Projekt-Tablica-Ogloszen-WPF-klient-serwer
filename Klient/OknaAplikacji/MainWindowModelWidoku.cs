using System.Windows;
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

            OperacjeKlient.PolaczZSerwerem();
        }

        private void ZamkniecieOkna(object x)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                OperacjeKlient.clientSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            OperacjeKlient.Wyslij("ODLACZENIE KLIENTA");
            OperacjeKlient.Wyslij(LogowanieModelWidoku.TextBoxLoginTextModelWidoku);

            OperacjeKlient.clientSocket.Close();
            if (StronaGlownaModelWidoku.CzyPanelAdminaOtwarty)
            {
                StronaGlownaModelWidoku.PanelAdmina.Close();
            }
        }
    }
}
