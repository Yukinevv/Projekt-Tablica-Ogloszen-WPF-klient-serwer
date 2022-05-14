using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Npgsql;

namespace Projekt
{
    /// <summary>
    /// Logika interakcji dla klasy PanelAdmina.xaml
    /// </summary>
    public partial class PanelAdmina : Window
    {
        public PanelAdmina()
        {
            InitializeComponent();
            uzytkownicyListView.ItemsSource = Connect.SelectRecordsUzytkownicy();
            Start.Visibility = Visibility.Visible;
            Podglad.Visibility = Visibility.Hidden;
        }

        private void uzytkownicyListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Start.Visibility = Visibility.Hidden;
            Podglad.Visibility = Visibility.Visible;

            List<Uzytkownicy> uzytkownik = (List<Uzytkownicy>)uzytkownicyListView.ItemsSource;
            LoginText.Text = uzytkownik[uzytkownicyListView.SelectedIndex].Login;
            MailText.Text = uzytkownik[uzytkownicyListView.SelectedIndex].Email;
            ImieText.Text = uzytkownik[uzytkownicyListView.SelectedIndex].Imie;
            NazwiskoText.Text = uzytkownik[uzytkownicyListView.SelectedIndex].Nazwisko;
            DataText.Text = uzytkownik[uzytkownicyListView.SelectedIndex].Data_ur;
            UprawnieniaText.Text = uzytkownik[uzytkownicyListView.SelectedIndex].Uprawnienia;

            LoginText.IsReadOnly = true;
            MailText.IsReadOnly = true;
            ImieText.IsReadOnly = true;
            NazwiskoText.IsReadOnly = true;
            DataText.IsReadOnly = true;
            UprawnieniaText.IsReadOnly = true;
        }

        private void AwansujButton_Click(object sender, RoutedEventArgs e)
        {
            if (UprawnieniaText.Text != "admin")
            {
                using (NpgsqlConnection conn = Connect.GetConnection())
                {
                    try
                    {
                        List<Uzytkownicy> uzytkownik = (List<Uzytkownicy>)uzytkownicyListView.ItemsSource;

                        conn.Open();
                        string query = @"UPDATE uzytkownicy SET uprawnienia='admin' WHERE id=:_id_u";

                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                        cmd.Parameters.AddWithValue("_id_u", uzytkownik[uzytkownicyListView.SelectedIndex].Id);

                        int n = cmd.ExecuteNonQuery();
                        if (n == 1)
                        {
                            UprawnieniaText.Text="admin";
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else 
            {
                MessageBox.Show("Ten użytkownik jest już administratorem!", "Blad!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ZdegradujButton_Click(object sender, RoutedEventArgs e)
        {
            if (UprawnieniaText.Text != "user")
            {
                using (NpgsqlConnection conn = Connect.GetConnection())
                {
                    try
                    {
                        List<Uzytkownicy> uzytkownik = (List<Uzytkownicy>)uzytkownicyListView.ItemsSource;

                        conn.Open();
                        string query = @"UPDATE uzytkownicy SET uprawnienia='user' WHERE id=:_id_u";

                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                        cmd.Parameters.AddWithValue("_id_u", uzytkownik[uzytkownicyListView.SelectedIndex].Id);

                        int n = cmd.ExecuteNonQuery();
                        if (n == 1)
                        {
                            UprawnieniaText.Text = "user";
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Ten użytkownik jest już zwyklym uzytkownikiem", "Blad!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WrocButton_Click(object sender, RoutedEventArgs e)
        {
            Start.Visibility = Visibility.Visible;
            Podglad.Visibility = Visibility.Hidden;
            uzytkownicyListView.ItemsSource = Connect.SelectRecordsUzytkownicy();

        }
    }
}
