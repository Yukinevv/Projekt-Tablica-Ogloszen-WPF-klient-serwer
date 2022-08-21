using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Klient
{
    public class OperacjeKlient
    {
        public static Socket clientSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

        public static string Odbierz()
        {
            var buffer = new byte[100000];
            int received = 0;
            try
            {
                received = clientSocket.Receive(buffer, SocketFlags.None); // odebranie danych od serwera
            }
            catch (Exception)
            {
                //MessageBox.Show("Brak polaczenia z serwerem! Przepraszamy za utrudnienia!", "Blad przy odbiorze danych");
                return "";
            }
            if (received == 0)
            {
                return "";
            }
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            string wiadomosc = Encoding.ASCII.GetString(data);
            return wiadomosc;
        }

        public static void Wyslij(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);

            try
            {
                clientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None); // wyslanie rzadania/danych do serwera
            }
            catch (Exception)
            {
                //MessageBox.Show("Brak polaczenia z serwerem! Przepraszamy za utrudnienia!", "Blad wyslania rzadania");
            }
        }
    }
}
