﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using BibliotekaEncje.Encje;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private static Socket serverSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

        private static Socket gniazdoPolaczenia;

        private static string Odbierz()
        {
            var buf = new byte[2048];
            int received = gniazdoPolaczenia.Receive(buf, SocketFlags.None); // otrzymuje wstepna informacje od klienta
            if (received == 0)
            {
                MessageBox.Show("Blad przy otrzymaniu informacji");
                return "";
            }
            var data = new byte[received];
            Array.Copy(buf, data, received);
            string odKlienta = Encoding.ASCII.GetString(data);
            return odKlienta;
        }

        private static void Wyslij(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            gniazdoPolaczenia.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        public static async void SerwerOperacje(object listbox)
        {
            EndPoint serverAddress = new IPEndPoint(IPAddress.Any, 11111);
            serverSocket.Bind(serverAddress);
            serverSocket.Listen(5);

            gniazdoPolaczenia = serverSocket.Accept();
            //await Task<Socket>.Factory.StartNew(serverSocket.Accept);

            ListBox ListBox1 = listbox as ListBox;
            ListBox1.Items.Add(gniazdoPolaczenia.RemoteEndPoint);

            await Task.Factory.StartNew(() =>
            {
                while (1 == 1)
                {
                    if (gniazdoPolaczenia.Connected)
                    {
                        string odKlienta = Odbierz();

                        if (odKlienta == "Ogloszenia")
                        {
                            using (var context = new MyDbContext())
                            {
                                StartoweDane dane = new StartoweDane(context);
                                dane.DodajStartoweDane(); // jezeli juz jakies sa w bazie to nie doda

                                var ogloszenia = context.Ogloszenia.ToList();
                                string oglSerialized = JsonConvert.SerializeObject(ogloszenia);
                                Wyslij(oglSerialized);
                            }
                        }
                        else if (odKlienta == "REJESTRACJA")
                        {
                            string zserializowanyObiekt = Odbierz();
                            var obiekt = JsonConvert.DeserializeObject<Uzytkownik>(zserializowanyObiekt);

                            using (var context = new MyDbContext())
                            {
                                context.Uzytkownicy.Add(obiekt);
                                context.SaveChanges();
                            }
                        }
                        else if (odKlienta == "LOGOWANIE")
                        {
                            string login = Odbierz();
                            Wyslij("OK");
                            string haslo = Odbierz();

                            using (var context = new MyDbContext())
                            {
                                bool czyZalogowac = context.Uzytkownicy.Any(u => u.Login == login && u.Haslo == haslo);
                                if (czyZalogowac)
                                {
                                    Wyslij("true");
                                }
                                else
                                {
                                    Wyslij("false");
                                }
                            }
                        }
                        else if (odKlienta == "KATEGORIE")
                        {
                            using (var context = new MyDbContext())
                            {
                                var kategorie = context.Kategorie.ToList();
                                string katSerialized = JsonConvert.SerializeObject(kategorie);
                                Wyslij(katSerialized);
                            }
                        }
                        else
                        {
                            gniazdoPolaczenia.Close();
                            MessageBox.Show("Nieznane rzadanie wyslania od klienta!");
                            //ListBox1.Items.Remove(gniazdoPolaczenia.RemoteEndPoint);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Klient NIE jest podlaczony! Wylaczam serwer.");
                        serverSocket.Close();
                        break;
                    }
                }
            });
        }

    }
}
