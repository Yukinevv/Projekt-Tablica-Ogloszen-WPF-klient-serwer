using Newtonsoft.Json;
using BibliotekaEncje.Encje;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static void PolaczZSerwerem()
        {
            EndPoint serverAddress = new IPEndPoint(IPAddress.Loopback, 11111);
            try
            {
                //await Task.Factory.StartNew(() => clientSocket.Connect(serverAddress));
                clientSocket.Connect(serverAddress);
            }
            catch (SocketException)
            {
                MessageBox.Show("BLAD: Polaczenie NIE zostalo nawiazane!");
            }
        }

        public static string Odbierz()
        {
            var buffer = new byte[2048];
            int received = clientSocket.Receive(buffer, SocketFlags.None); // odebranie zserializowanych danych od serwera
            if (received == 0)
            {
                MessageBox.Show("Blad przy otrzymaniu informacji");
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
            clientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None); // wyslanie rzadania do serwera
        }
    }
}
