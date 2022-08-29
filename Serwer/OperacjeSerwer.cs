using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Serwer
{
    /// <summary>
    /// Klasa odpowiadajaca za dzialanie serwera w tym m.in. obsluge wysylanych przez klientow rzadan
    /// </summary>
    public class OperacjeSerwer : BaseViewModel
    {
        public static Socket serverSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

        private static readonly List<Socket> clientSockets = new List<Socket>();

        private const int BUFFER_SIZE = 8192;
        private const int PORT = 11111;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];
        public static string odKlienta;

        private static List<string> zalogowaniKlienciLoginy = new List<string>();

        public static void SerwerStart()
        {
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        public static void SerwerStop()
        {
            foreach (Socket socket in clientSockets)
            {
                socket.Close();
            }
            serverSocket.Close();
        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;

            try
            {
                socket = serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException)
            {
                return;
            }

            clientSockets.Add(socket);
            MainWindowModelWidoku.ListBoxPolaczeniKlienciModelWidoku.Add(socket.RemoteEndPoint.ToString());
            MainWindow.TextBoxLogs.Dispatcher.Invoke(new Action(() =>
            {
                MainWindow.TextBoxLogs.Text += "Polaczono z klientem o adresie IP: " + socket.RemoteEndPoint.ToString() + "    (" + DateTime.Now.ToString("HH:mm:ss") + ")\n";
            }));

            socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, SerwerOperacje, socket);

            serverSocket.BeginAccept(AcceptCallback, null);
        }

        public static void Odbierz(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received = 0;

            try
            {
                received = current.EndReceive(AR);
            }
            //catch (SocketException)
            //{
            //    current.Close();
            //    clientSockets.Remove(current);
            //    return;
            //}
            catch (Exception)
            {
                return;
            }

            var data = new byte[received];
            Array.Copy(buffer, data, received);
            odKlienta = Encoding.ASCII.GetString(data);
        }

        public static void Wyslij(string text, Socket current)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            try
            {
                current.Send(buffer, 0, buffer.Length, SocketFlags.None);
            }
            catch (Exception err)
            {
                current.Close();
                clientSockets.Remove(current);
                //MessageBox.Show(err.Message);
            }
        }

        private static void SerwerOperacje(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            Odbierz(AR);

            MainWindow.TextBoxLogs.Dispatcher.Invoke(new Action(() =>
            {
                string rzadanie = odKlienta;
                if (odKlienta.Length > 25) rzadanie = odKlienta.Substring(0, 25);
                MainWindow.TextBoxLogs.Text += "[" + current.RemoteEndPoint.ToString() + "] Wyslal rzadanie: " + rzadanie + "    (" + DateTime.Now.ToString("HH:mm:ss") + ")\n";
            }));

            if (odKlienta == "REJESTRACJA")
            {
                PoleceniaOdKlienta.Rejestracja(buffer, BUFFER_SIZE, current);
            }
            else if (odKlienta == "LOGOWANIE")
            {
                PoleceniaOdKlienta.Logowanie(buffer, BUFFER_SIZE, current, zalogowaniKlienciLoginy);
            }
            else if (odKlienta == "DODAJ LOGIN i SPRAWDZ CZY ADMIN i POBIERZ KATEGORIE")
            {
                PoleceniaOdKlienta.DodajLoginCzyAdminKategorie(buffer, BUFFER_SIZE, current, zalogowaniKlienciLoginy);
            }
            else if (odKlienta == "WYLOGUJ")
            {
                PoleceniaOdKlienta.Wyloguj(buffer, BUFFER_SIZE, current, zalogowaniKlienciLoginy);
            }
            else if (odKlienta == "DODANIE KATEGORII")
            {
                PoleceniaOdKlienta.DodanieKategorii(buffer, BUFFER_SIZE, current);
            }
            else if (odKlienta == "USUNIECIE KATEGORII")
            {
                PoleceniaOdKlienta.UsuniecieKategorii(buffer, BUFFER_SIZE, current);
            }
            else if (odKlienta == "OGLOSZENIA")
            {
                PoleceniaOdKlienta.Ogloszenia(buffer, BUFFER_SIZE, current);
            }
            else if (odKlienta == "WYBRANE NAZWY KATEGORII")
            {
                PoleceniaOdKlienta.WybraneNazwyKategorii(buffer, BUFFER_SIZE, current);
            }
            else if (odKlienta == "CZY MOZE EDYTOWAC")
            {
                PoleceniaOdKlienta.CzyMozeEdytowac(buffer, BUFFER_SIZE, current);
            }
            else if (odKlienta == "DODANIE OGLOSZENIA")
            {
                PoleceniaOdKlienta.DodanieOgloszenia(buffer, BUFFER_SIZE, current);
            }
            else if (odKlienta == "USUNIECIE OGLOSZENIA")
            {
                PoleceniaOdKlienta.UsuniecieOgloszenia(buffer, BUFFER_SIZE, current);
            }
            else if (odKlienta == "EDYCJA OGLOSZENIA")
            {
                PoleceniaOdKlienta.EdycjaOgloszenia(buffer, BUFFER_SIZE, current);
            } 
            else if (odKlienta == "UZYTKOWNICY")
            {
                PoleceniaOdKlienta.Uzytkownicy(buffer, BUFFER_SIZE, current);
            }
            else if (odKlienta == "MOJE DANE")
            {
                PoleceniaOdKlienta.MojeDane(buffer, BUFFER_SIZE, current);
            }
            else if (odKlienta == "EDYCJA DANYCH UZYTKOWNIKA")
            {
                PoleceniaOdKlienta.EdycjaDanychUzytkownika(buffer, BUFFER_SIZE, current);
            }
            else if (odKlienta == "MOJE OGLOSZENIA")
            {
                PoleceniaOdKlienta.MojeOgloszenia(buffer, BUFFER_SIZE, current);
            }
            else if (odKlienta == "AWANS UZYTKOWNIKA")
            {
                PoleceniaOdKlienta.AwansUzytkownika(buffer, BUFFER_SIZE, current);
            }
            else if (odKlienta == "ZDEGRADOWANIE UZYTKOWNIKA")
            {
                PoleceniaOdKlienta.ZdegradowanieUzytkownika(buffer, BUFFER_SIZE, current);
            }
            else if (odKlienta == "KOMENTARZE")
            {
                PoleceniaOdKlienta.Komentarze(buffer, BUFFER_SIZE, current);
            }
            else if (odKlienta == "DODANIE KOMENTARZA")
            {
                PoleceniaOdKlienta.DodanieKomentarza(buffer, BUFFER_SIZE, current);
            }
            else if (odKlienta == "USUNIECIE KOMENTARZY")
            {
                PoleceniaOdKlienta.UsuniecieKomentarzy(buffer, BUFFER_SIZE, current);
            }
            else if (odKlienta == "ODLACZENIE KLIENTA")
            {
                PoleceniaOdKlienta.OdlaczenieKlienta(buffer, BUFFER_SIZE, current, clientSockets, zalogowaniKlienciLoginy);
                return;
            }
            else
            {
                PoleceniaOdKlienta.NierozpoznaneRzadanie(current, clientSockets);            
                return;
            }

            try
            {
                current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, SerwerOperacje, current);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
