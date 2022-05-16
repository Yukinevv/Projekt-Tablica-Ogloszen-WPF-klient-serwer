using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;
using System.Security.Cryptography;

namespace Projekt
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int id;
        public static int id_wybranej_kategorii;
        private PanelAdmina panelAdmina;

        public MainWindow()
        {
            InitializeComponent();

            // pokaz i ukryj odpowiednie gridy
            logowanie.Visibility = (Visibility)0;
            TextBlock1.Visibility = (Visibility)0;

            rejestracja.Visibility = Visibility.Hidden;
            LabelPassError.Visibility = Visibility.Hidden;
            program.Visibility = Visibility.Hidden;
            dodajOgloszenie.Visibility = Visibility.Hidden;
            edycjaOgloszenia.Visibility = Visibility.Hidden;

            rezultatDodania.Visibility = Visibility.Hidden;
            rezultatEdycji.Visibility = Visibility.Hidden;

            program_kategorie.Visibility = Visibility.Hidden;

            // wypisz dosetpne konta uzytkownikow - roboczo
            TextBlock1.Text = "Dostepni uzytkownicy:\n(hasło do jank: qwerty123)\n";
            foreach (string elements in Connect.SelectRecords())
            {
                TextBlock1.Text += elements + "\n";
            }

            // pokaz opcje comboboxa
            ComboBox1.ItemsSource = new string[] { "Malejąco", "Rosnąco" };
            ComboBox2.ItemsSource = new string[] { "Tytul", "Data" };
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Ogloszenia> ogloszenia = Connect.SelectRecordsOgloszenia2(id_wybranej_kategorii);
            if(ComboBox2.SelectedItem == null)
            {
                if ((string)ComboBox1.SelectedItem == "Rosnąco")
                {
                    ogloszenia = ogloszenia.OrderBy(x => x.Id_o).ToList();
                    ListView1.ItemsSource = ogloszenia;
                }
                else if ((string)ComboBox1.SelectedItem == "Malejąco")
                {
                    ogloszenia = ogloszenia.OrderByDescending(x => x.Id_o).ToList();
                    ListView1.ItemsSource = ogloszenia;
                }
            }
            else
            {
                if ((string)ComboBox1.SelectedItem == "Rosnąco")
                {
                    switch (ComboBox2.SelectedItem as string)
                    {
                        case "Data":
                            ogloszenia = ogloszenia.OrderBy(x => x.Data_utw).ToList();
                            break;

                        case "Tytul":
                            ogloszenia = ogloszenia.OrderBy(x => x.Tytul).ToList();
                            break;
                    }
                    ListView1.ItemsSource = ogloszenia;
                }
                else if ((string)ComboBox1.SelectedItem == "Malejąco")
                {
                    switch (ComboBox2.SelectedItem as string)
                    {
                        case "Data":
                            ogloszenia = ogloszenia.OrderByDescending(x => x.Data_utw).ToList();
                            break;

                        case "Tytul":
                            ogloszenia = ogloszenia.OrderByDescending(x => x.Tytul).ToList();
                            break;
                    }
                    ListView1.ItemsSource = ogloszenia;
                }
            }
        }

        public Predicate<object> GetFilter()
        {
            return TytulFilter;
        }

        public Predicate<object> GetFilterK()
        {
            return NazwaFilter;
        }

        private bool TytulFilter(object obj)
        {
            var Filterobj = obj as Ogloszenia;
            return Filterobj.Tytul.Contains(FilterTextBox.Text);
        }
        private bool NazwaFilter(object obj)
        {
            var Filterobj = obj as Kategoria;
            return Filterobj.Nazwa.Contains(FilterTextBoxK.Text);
        }

        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FilterTextBox.Text == null)
            {
                ListView1.Items.Filter = null;
            }
            else
            {
                ListView1.Items.Filter = GetFilter();
            }
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            rejestracja.Visibility = (Visibility)1;
            // pokaz grid logowania
            logowanie.Visibility = (Visibility)0;
            TextBlock1.Visibility = (Visibility)0;
        }

        private void ZarejestrujSieDoGrida_Click(object sender, RoutedEventArgs e)
        {
            logowanie.Visibility = (Visibility)1;
            // pokaz grid rejestracji
            rejestracja.Visibility = (Visibility)0;
            //mozliwosc wprowadzenia daty urodzenia do dzisiaj(zakladamy ze czlonek naszego poratlu mogl urodzic sie dzisiaj, ale nie w przyszlosci :D )
            DatePicker1.DisplayDateEnd = DateTime.Today;
            //zakladamy ze osoba majaca majaca wiecej niz 122 lata do nas nie bedzie chciala sie zarejestrowac :(
            DatePicker1.DisplayDateStart = DateTime.Parse("1900-01-01");
            TextBlock1.Visibility = (Visibility)1;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            using (NpgsqlConnection conn = Connect.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT * FROM login(:_login, :_haslo)"; //login() jest funkcja w postgresie

                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    // hashowanie hasla w celu porownania zahashowanego znajdujacego sie w bazie
                    SHA256 sha256Hash = SHA256.Create();
                    string hash = Operacje.GetHash(sha256Hash, PassBox.Password);

                    cmd.Parameters.AddWithValue("_login", LoginBox.Text);
                    cmd.Parameters.AddWithValue("_haslo", hash);

                    int result = (int)cmd.ExecuteScalar();

                    if (result == 1) // logowanie powiodlo sie
                    {
                        PassBox.Password = "";

                        // wez id zalogowanego uzytkownika
                        string query2 = @"SELECT id FROM uzytkownicy WHERE login=:_login";
                        NpgsqlCommand cmd2 = new NpgsqlCommand(query2, conn);

                        cmd2.Parameters.AddWithValue("_login", LoginBox.Text);

                        using (NpgsqlDataReader reader = cmd2.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                id = int.Parse(reader["id"].ToString());
                            }
                        }

                        // wypisz nazwe uzytkownika
                        WitajLabel.Content = $"Witaj {LoginBox.Text}!";
                        WitajLabel_kat.Content = $"Witaj {LoginBox.Text}!";

                        // pokaz grid glownego layoutu
                        program.Visibility = (Visibility)1;
                        logowanie.Visibility = (Visibility)1;
                        program_kategorie.Visibility = (Visibility)0;

                        //wypisz dostepne kategorie posortowane rosnaco po nazwie
                        List<Kategoria> kategorie = Connect.SelectRecordsKategoria();
                        kategorie = kategorie.OrderBy(x => x.Nazwa).ToList();
                        ListView2.ItemsSource = kategorie;
                      
                        //ukrycie pomocniczych danych do logowania kont
                        TextBlock1.Visibility= Visibility.Hidden;
                        PokazPanelAdmina.Visibility = Visibility.Hidden;

                        //wyswietlanie okna w ktorym bedzie panel administracyjny
                        int czy_admin = 0;
                        string query3 = @"SELECT COUNT(*) AS ile FROM uzytkownicy WHERE login=:_login AND uprawnienia='admin'";
                        NpgsqlCommand cmd3 = new NpgsqlCommand(query3, conn);
                        cmd3.Parameters.AddWithValue("_login", LoginBox.Text);
                     
                        using (NpgsqlDataReader reader2 = cmd3.ExecuteReader())
                        {
                            while (reader2.Read())
                            {
                                czy_admin = int.Parse(reader2["ile"].ToString());
                            }
                        }
                        if (czy_admin == 1)
                        {
                            //otworz nowe okno
                            panelAdmina = new PanelAdmina();
                            panelAdmina.Show();
                            PokazPanelAdmina.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sprawdz swoje dane logowania", "Blad logowania", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        conn.Close();
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ListView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListView1.SelectedItem == null)
            {
                return;
            }

            program.Visibility = Visibility.Hidden;
            edycjaOgloszenia.Visibility = Visibility.Visible;

            ZatwierdzButton.Visibility = Visibility.Hidden;
            UsunButton.Visibility = Visibility.Hidden;

            try
            {
                List<Ogloszenia> ogloszenia = (List<Ogloszenia>)ListView1.ItemsSource;
                Tytul.Text = ogloszenia[ListView1.SelectedIndex].Tytul;
                Tresc.Text = ogloszenia[ListView1.SelectedIndex].Tresc;
                Tytul.IsReadOnly = true;
                Tresc.IsReadOnly = true;

                //sprawdzenie uprawnien zalogowanego uzytkownika do edycji i usuwania wybranego ogloszenia
                using (NpgsqlConnection conn = Connect.GetConnection())
                {
                    string query = @"SELECT U.login FROM uzytkownicy U JOIN ogloszenia O ON U.id=O.id_u WHERE O.id_u=:_id_u OR U.uprawnienia='admin'";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("_id_u", ogloszenia[ListView1.SelectedIndex].Id_u);

                    string login = "";

                    conn.Open();

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            login = (string)reader["login"];

                            if (login == LoginBox.Text)
                            {
                                ZatwierdzButton.Visibility = Visibility.Visible;
                                UsunButton.Visibility = Visibility.Visible;
                                Tytul.IsReadOnly = false;
                                Tresc.IsReadOnly = false;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // pokaz grid logowania
            logowanie.Visibility = (Visibility)0;
            program.Visibility = (Visibility)1;
            program_kategorie.Visibility = (Visibility)1;

            // wypisz dostepne konta uztykownikow - roboczo
            TextBlock1.Visibility = Visibility.Visible;
            TextBlock1.Text = "Dostepni uzytkownicy:\n";
            foreach (string elements in Connect.SelectRecords())
            {
                TextBlock1.Text += elements + "\n";
            }
            panelAdmina.Close();
        }

        private void Rejestruj_Click(object sender, RoutedEventArgs e)
        {
            using (NpgsqlConnection conn = Connect.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"INSERT INTO uzytkownicy VALUES(
                                     nextval('increment_id_uzytkownicy'), :_login, :_haslo, :_imie, :_nazwisko, :_email, :_data_ur, 'user')";
                                    //increment_id_uzytkownicy jest to sekwencja robiaca za AUTO INCREMENT w postgresie

                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    // hashowanie hasla
                    SHA256 sha256Hash = SHA256.Create();
                    string hash = Operacje.GetHash(sha256Hash, PassBox1.Password);

                    cmd.Parameters.AddWithValue("_login", TextBoxLogin.Text);
                    cmd.Parameters.AddWithValue("_haslo", hash);
                    cmd.Parameters.AddWithValue("_imie", TextBoxImie.Text);
                    cmd.Parameters.AddWithValue("_nazwisko", TextBoxNazwisko.Text);
                    cmd.Parameters.AddWithValue("_email", TextBoxEmail.Text);
                    //cmd.Parameters.AddWithValue("_data_ur", DatePicker1.SelectedDate);
                    cmd.Parameters.AddWithValue("_data_ur", DatePicker1.SelectedDate.ToString());

                    // warunki poprawnosci hasla
                    if (PassBox1.Password.Length < 8 || PassBox1.Password.Length > 20)
                    {
                        LabelPassError.Visibility = (Visibility)0;
                        LabelPassError.Content = "Niepoprawna długosc hasła!";
                        conn.Close();
                        return;
                    }
                    if (PassBox1.Password != PassBox2.Password)
                    {
                        LabelPassError.Visibility = (Visibility)0;
                        LabelPassError.Content = "Podane hasła nie są takie same!";
                        conn.Close();
                        return;
                    }

                    int n = cmd.ExecuteNonQuery();
                    if (n == 1)
                    {
                        LabelPassError.Visibility = (Visibility)1;
                        TextBlock1.Visibility = (Visibility)0;
                        TextBlock1.Text = "Rejestracja zakonczona powodzeniem!";
                        // pokaz grid logowania
                        logowanie.Visibility = (Visibility)0;
                        rejestracja.Visibility = (Visibility)1;
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ZatwierdzButton_Click(object sender, RoutedEventArgs e)
        {
            if (Tytul.Text != "" && Tresc.Text != "")
            {
                using (NpgsqlConnection conn = Connect.GetConnection())
                {
                    try
                    {
                        conn.Open();
                        string query = @"UPDATE ogloszenia SET tytul=:_tytul, tresc=:_tresc, data_ed=:_data_ed WHERE id_o=:_id_o";

                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                        cmd.Parameters.AddWithValue("_tytul", Tytul.Text);
                        cmd.Parameters.AddWithValue("_tresc", Tresc.Text);
                        cmd.Parameters.AddWithValue("_data_ed", DateTime.Now.ToString("yyyy-MM-dd"));

                        List<Ogloszenia> ogloszenia = (List<Ogloszenia>)ListView1.ItemsSource;
                        cmd.Parameters.AddWithValue("_id_o", ogloszenia[ListView1.SelectedIndex].Id_o);

                        int n = cmd.ExecuteNonQuery();
                        if (n == 1)
                        {
                            rezultatEdycji.Visibility = Visibility.Visible;
                            rezultatEdycji.Content = "Zedytowano ogłoszenie!";
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
                rezultatEdycji.Visibility = Visibility.Visible;
                rezultatEdycji.Content = "Uzupełnij tytuł oraz treść!";
            }
        }

        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            rezultatEdycji.Visibility = Visibility.Hidden;

            edycjaOgloszenia.Visibility = Visibility.Hidden;
            program.Visibility = Visibility.Visible;

            try
            {
                List<Ogloszenia> ogloszenia = Connect.SelectRecordsOgloszenia2(id_wybranej_kategorii);
                ogloszenia = ogloszenia.OrderBy(x => x.Id_o).ToList();
                ListView1.ItemsSource = ogloszenia;

                int iloscOgloszen = Operacje.PoliczOgloszeniaK(id_wybranej_kategorii);
                IloscOgloszenLabel.Content = $"Wyświetlono {iloscOgloszen} ogłoszeń/nia";
            }
            catch (Exception err)
            {
                MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak 2", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OdswiezButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Ogloszenia> ogloszenia = Connect.SelectRecordsOgloszenia2(id_wybranej_kategorii);
                ogloszenia = ogloszenia.OrderBy(x => x.Id_o).ToList();
                ListView1.ItemsSource = ogloszenia;

                int iloscOgloszen = Operacje.PoliczOgloszeniaK(id_wybranej_kategorii);
                IloscOgloszenLabel.Content = $"Wyświetlono {iloscOgloszen} ogłoszeń/nia";
            }
            catch (Exception err)
            {
                MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak 2", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ComboBox1.SelectedIndex = 0;
            ComboBox2.SelectedIndex = 0;
            FilterTextBox.Text = "";
        }

        private void DodajOgloszenieButton_Click(object sender, RoutedEventArgs e)
        {
            program.Visibility = Visibility.Hidden;
            dodajOgloszenie.Visibility = Visibility.Visible;

            List<string> kategorie = Connect.SelectRecordsKategoriaS();
            ListBoxKategorie.ItemsSource = kategorie;
        }

        private void ZatwierdzButtonD_Click(object sender, RoutedEventArgs e)
        {
            using (NpgsqlConnection conn = Connect.GetConnection())
            {
                if (TytulD.Text != "" && TrescD.Text != "" && ListBoxKategorie.SelectedItems.Count > 0)
                {
                    try
                    {
                        //Dodawanie nowych ogloszen
                        conn.Open();

                        string query = @"INSERT INTO ogloszenia VALUES(
                                     nextval('increment_id_ogloszenia'), :_id_u, :_tytul, :_data_utw, :_data_ed, :_tresc)";

                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                        cmd.Parameters.AddWithValue("_tytul", TytulD.Text);
                        cmd.Parameters.AddWithValue("_tresc", TrescD.Text);
                        cmd.Parameters.AddWithValue("_id_u", id);
                        cmd.Parameters.AddWithValue("_data_utw", DateTime.Now.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("_data_ed", DateTime.Now.ToString("yyyy-MM-dd"));

                        int n = cmd.ExecuteNonQuery();

                        int id_dodanego_ogloszenia = Operacje.IdOstatnioDodanegoOgloszenia(id);
                        int zaznaczonych = ListBoxKategorie.SelectedItems.Count;

                        //dodawanie do tabeli wiele do wielu ogloszenia do wybranych przez uzytkownika kategorii za pomoca listboxa
                        foreach (object zaznaczono in ListBoxKategorie.SelectedItems)
                        {
                            Operacje.DodajOglDoKat(id_dodanego_ogloszenia, Operacje.IdKategorii(zaznaczono.ToString()));
                        }

                        if (n == 1)
                        {
                            rezultatDodania.Visibility = Visibility.Visible;
                            rezultatDodania.Content = $"Dodano nowe ogłoszenie, do {zaznaczonych} kategorii!";
                            //czyszczenie okienek, ktore beda gotowe do dodania nowego ogloszenia
                            TytulD.Text = "";
                            TrescD.Text = "";
                            ListBoxKategorie.SelectedItems.Clear();
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    rezultatDodania.Visibility = Visibility.Visible;
                    rezultatDodania.Content = "Podaj tytuł, nazwę oraz zaznacz conajmniej 1 kategorię!";
                }
            }
        }

        private void PowrotButtonD_Click(object sender, RoutedEventArgs e)
        {
            rezultatDodania.Visibility = Visibility.Hidden;

            dodajOgloszenie.Visibility = Visibility.Hidden;
            program.Visibility = Visibility.Visible;

            try
            {
                List<Ogloszenia> ogloszenia = Connect.SelectRecordsOgloszenia2(id_wybranej_kategorii);
                ogloszenia = ogloszenia.OrderBy(x => x.Id_o).ToList();
                ListView1.ItemsSource = ogloszenia;

                int iloscOgloszen = Operacje.PoliczOgloszeniaK(id_wybranej_kategorii);
                IloscOgloszenLabel.Content = $"Wyświetlono {iloscOgloszen} ogłoszeń/nia";
            }
            catch (Exception err)
            {
                MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak 2", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UsunButton_Click(object sender, RoutedEventArgs e)
        {
            using (NpgsqlConnection conn = Connect.GetConnection())
            {
                try
                {
                    conn.Open();
                    List<Ogloszenia> ogloszenia = (List<Ogloszenia>)ListView1.ItemsSource;
                    string query = @"DELETE FROM ogloszenia WHERE id_o = :_id_o";

                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("_id_o", ogloszenia[ListView1.SelectedIndex].Id_o);

                    string query2 = @"DELETE FROM kattoogl WHERE id_o = :_id_o";

                    NpgsqlCommand cmd2 = new NpgsqlCommand(query2, conn);
                    cmd2.Parameters.AddWithValue("_id_o", ogloszenia[ListView1.SelectedIndex].Id_o);

                    //najpierw musza zostac usuniete wiersze z tabeli wiele do wielu, bo sa tam klucze obce
                    cmd2.ExecuteNonQuery();
                    int n = cmd.ExecuteNonQuery();
                    if (n == 1)
                    {
                        //przechodzenie do grida z ogloszeniami z wybranej kategorii
                        Tytul.Text = "";
                        Tresc.Text = "";
                        edycjaOgloszenia.Visibility = Visibility.Hidden;
                        program.Visibility = Visibility.Visible;
                        //odswiezenie listview
                        List<Ogloszenia> ogloszenia_new = Connect.SelectRecordsOgloszenia2(id_wybranej_kategorii);
                        ogloszenia = ogloszenia_new.OrderBy(x => x.Id_o).ToList();
                        ListView1.ItemsSource = ogloszenia_new;

                        int iloscOgloszen = Operacje.PoliczOgloszeniaK(id_wybranej_kategorii);
                        IloscOgloszenLabel.Content = $"Wyświetlono {iloscOgloszen} ogłoszeń/nia";
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ListView2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListView2.SelectedItem == null)
            {
                return;
            }
            List<Kategoria> kategorie = (List<Kategoria>)ListView2.ItemsSource;
            //zapisuje sobie id_k w ktore kliknal uzytkownik, aby wyswietlic wszystkie ogloszenia z tej kategorii
            id_wybranej_kategorii = kategorie[ListView2.SelectedIndex].Id_k;

            List<Ogloszenia> ogloszenia = Connect.SelectRecordsOgloszenia2(id_wybranej_kategorii);
            ogloszenia = ogloszenia.OrderBy(x => x.Id_o).ToList();
            ListView1.ItemsSource = ogloszenia;

            program_kategorie.Visibility = Visibility.Hidden;
            program.Visibility = Visibility.Visible;

            int iloscOgloszen = Operacje.PoliczOgloszeniaK(id_wybranej_kategorii);
            IloscOgloszenLabel.Content = $"Wyświetlono {iloscOgloszen} ogłoszeń/nia";
        }

        private void OdswiezButton_kat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Kategoria> kategorie = Connect.SelectRecordsKategoria();
                kategorie = kategorie.OrderBy(x => x.Nazwa).ToList();
                ListView2.ItemsSource = kategorie;

                rezultatDodania_kat.Content = "";
                rezultatDodania_kat.Visibility = Visibility.Hidden;
                TextBox_NazwaNowejKategorii.Text = "";
            }
            catch (Exception err)
            {
                MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak 2", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DodajKategorie_Click(object sender, RoutedEventArgs e)
        {
            if (TextBox_NazwaNowejKategorii.Text != "")
            {
                //sprawdzenie czy jest juz kategoria o takiej nazwie(przeciwdzialam duplikacji)                                
                rezultatDodania_kat.Visibility = Visibility.Visible;
                if (Operacje.PoliczKategorie(TextBox_NazwaNowejKategorii.Text) == 0)
                {
                    using (NpgsqlConnection conn = Connect.GetConnection())
                    {
                        try
                        {
                            conn.Open();

                            string query = "INSERT INTO kategoria VALUES(nextval('increment_id_kategoria'), :_nazwa, :_id_u, :_data_utw)";

                            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                            cmd.Parameters.AddWithValue("_nazwa", TextBox_NazwaNowejKategorii.Text);
                            cmd.Parameters.AddWithValue("_id_u", id);
                            cmd.Parameters.AddWithValue("_data_utw", DateTime.Now.ToString("yyyy-MM-dd"));

                            int n = cmd.ExecuteNonQuery();
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }

                    rezultatDodania_kat.Foreground = new SolidColorBrush(Colors.Green);
                    rezultatDodania_kat.Visibility = Visibility.Visible;
                    rezultatDodania_kat.Content = "Brawo dodałeś/aś nową kategorię!";

                    List<Kategoria> kategorie = Connect.SelectRecordsKategoria();
                    kategorie = kategorie.OrderBy(x => x.Nazwa).ToList();
                    ListView2.ItemsSource = kategorie;
                }
                else //jezeli textbox od wprowadzenia nowej nazwy kategorii jest pusty
                {
                    rezultatDodania_kat.Foreground = new SolidColorBrush(Colors.Red);
                    rezultatDodania_kat.Visibility = Visibility.Visible;
                    rezultatDodania_kat.Content = "Taka kategoria już istnieje!";
                }

            }
            else
            {
                rezultatDodania_kat.Foreground = new SolidColorBrush(Colors.Red);
                rezultatDodania_kat.Visibility = Visibility.Visible;
                rezultatDodania_kat.Content = "Powyższe pole nie może być puste!";
            }
        }

        private void FilterTextBoxK_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FilterTextBox.Text == null)
            {
                ListView2.Items.Filter = null;
            }
            else
            {
                ListView2.Items.Filter = GetFilterK();
            }
        }

        private void Wroc_Click(object sender, RoutedEventArgs e)
        {
            program.Visibility = Visibility.Hidden;
            program_kategorie.Visibility = Visibility.Visible;
            FilterTextBox.Text = "";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PokazPanelAdmina_Click(object sender, RoutedEventArgs e)
        {
            panelAdmina = new PanelAdmina();
            panelAdmina.Show();
        }
    }
}
