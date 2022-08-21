using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Serwer
{
    public class OperacjeSerwer
    {
        public static Socket serverSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

        private static Socket gniazdoPolaczenia;

        public static string Odbierz()
        {
            var buf = new byte[5000];
            int received = 0;
            try
            {
                received = gniazdoPolaczenia.Receive(buf, SocketFlags.None); // otrzymuje wstepna informacje od klienta
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            if (received == 0)
            {
                return "";
            }
            var data = new byte[received];
            Array.Copy(buf, data, received);
            string odKlienta = Encoding.ASCII.GetString(data);
            return odKlienta;
        }

        public static void Wyslij(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            try
            {
                gniazdoPolaczenia.Send(buffer, 0, buffer.Length, SocketFlags.None);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        public static async Task SerwerOperacje(object listBox)
        {
            EndPoint serverAddress = new IPEndPoint(IPAddress.Any, 11111);
            serverSocket.Bind(serverAddress);
            serverSocket.Listen(5);

            try
            {
                gniazdoPolaczenia = await Task<Socket>.Factory.StartNew(serverSocket.Accept);
            }
            catch (Exception)
            {
                return;
            }

            ListBox ListBoxPolaczeniKlienci = listBox as ListBox;
            ListBoxPolaczeniKlienci.Items.Add(gniazdoPolaczenia.RemoteEndPoint);

            await Task.Factory.StartNew(() =>
            {
                while (1 == 1)
                {
                    if (gniazdoPolaczenia.Connected)
                    {
                        string odKlienta = Odbierz();

                        if (odKlienta == "REJESTRACJA")
                        {
                            PoleceniaOdKlienta.Rejestracja();
                        }
                        else if (odKlienta == "LOGOWANIE")
                        {
                            PoleceniaOdKlienta.Logowanie();
                        }
                        else if (odKlienta == "CZY ADMIN")
                        {
                            PoleceniaOdKlienta.CzyAdmin();
                        }
                        else if (odKlienta == "KATEGORIE")
                        {
                            PoleceniaOdKlienta.Kategorie();
                        }
                        else if (odKlienta == "WYBRANE NAZWY KATEGORII")
                        {
                            PoleceniaOdKlienta.WybraneNazwyKategorii();
                        }
                        else if (odKlienta == "DODANIE KATEGORII")
                        {
                            PoleceniaOdKlienta.DodanieKategorii();
                        }
                        else if (odKlienta == "USUNIECIE KATEGORII")
                        {
                            PoleceniaOdKlienta.UsuniecieKategorii();
                        }
                        else if (odKlienta == "OGLOSZENIA")
                        {
                            PoleceniaOdKlienta.Ogloszenia();
                        }
                        else if (odKlienta == "MOJE OGLOSZENIA")
                        {
                            PoleceniaOdKlienta.MojeOgloszenia();
                        }
                        else if (odKlienta == "DODANIE OGLOSZENIA")
                        {
                            PoleceniaOdKlienta.DodanieOgloszenia();
                        }
                        else if (odKlienta == "USUNIECIE OGLOSZENIA")
                        {
                            PoleceniaOdKlienta.UsuniecieOgloszenia();
                        }
                        else if (odKlienta == "EDYCJA OGLOSZENIA")
                        {
                            PoleceniaOdKlienta.EdycjaOgloszenia();
                        }
                        else if (odKlienta == "CZY MOZE EDYTOWAC")
                        {
                            PoleceniaOdKlienta.CzyMozeEdytowac();
                        }
                        else if (odKlienta == "MOJE DANE")
                        {
                            PoleceniaOdKlienta.MojeDane();
                        }
                        else if (odKlienta == "EDYCJA DANYCH UZYTKOWNIKA")
                        {
                            PoleceniaOdKlienta.EdycjaDanychUzytkownika();
                        }
                        else if (odKlienta == "UZYTKOWNICY")
                        {
                            PoleceniaOdKlienta.Uzytkownicy();
                        }
                        else if (odKlienta == "AWANS UZYTKOWNIKA")
                        {
                            PoleceniaOdKlienta.AwansUzytkownika();
                        }
                        else if (odKlienta == "ZDEGRADOWANIE UZYTKOWNIKA")
                        {
                            PoleceniaOdKlienta.ZdegradowanieUzytkownika();
                        }
                        else
                        {
                            gniazdoPolaczenia.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Klient zakonczyl polaczenie!");
                        break;
                    }
                }
            });
            ListBoxPolaczeniKlienci.Items.RemoveAt(ListBoxPolaczeniKlienci.Items.Count - 1);
        }
    }
}
