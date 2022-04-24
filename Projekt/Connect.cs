using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Projekt
{
    class Connect
    {
        public static void InsertRecord()
        {
            using (NpgsqlConnection conn = GetConnection())
            {
                string query = @"INSERT INTO public.test VALUES(1, 'Jan', 'Kowalski', 123456789)";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                conn.Open();
                int n = cmd.ExecuteNonQuery();
                if (n == 1)
                {
                    Console.WriteLine("Rekord zostal dodany do bazy");
                }
            }
        }

        public static List<string> SelectRecords()
        {
            List<string> result = new List<string>();
            using (NpgsqlConnection conn = GetConnection())
            {
                string query = @"SELECT login, haslo FROM uzytkownicy";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                conn.Open();
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add("Login: " + reader["login"] + "   Hasło: " + reader["haslo"]);
                    }
                }
            }
            return result;
        }

        public static List<string> SelectRecordsOgloszenia()
        {
            List<string> result = new List<string>();
            using (NpgsqlConnection conn = GetConnection())
            {
                string query = @"SELECT * FROM ogloszenia";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                conn.Open();
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string[] tmp = reader["data_utw"].ToString().Split(' ');

                        string[] tmp2 = reader["data_ed"].ToString().Split(' ');

                        //result.Add(reader["id_o"] + " " + reader["id_u"] + " " + reader["tytul"] + " " + reader["kategoria"] + " "
                        //    + tmp[0] + " " + tmp2[0] + " " + reader["tresc"]);

                        result.Add(reader["id_o"] + "\t" + reader["id_u"] + "\t" + reader["tytul"] + "\t" + reader["kategoria"] + " \t"
                            + tmp[0] + "\t" + tmp2[0] + "\t" + reader["tresc"]);
                    }
                }
            }
            return result;
        }

        public static List<Ogloszenia> SelectRecordsOgloszenia2() // sposob z mapowaniem
        {
            List<Ogloszenia> result = new List<Ogloszenia>();
            using (NpgsqlConnection conn = GetConnection())
            {
                string query = @"SELECT * FROM ogloszenia";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                conn.Open();
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Ogloszenia tmp = new Ogloszenia
                        {
                            Id_o = int.Parse(reader["id_o"].ToString()),
                            Id_u = int.Parse(reader["id_u"].ToString()),
                            Tytul = (string)reader["tytul"],
                            Kategoria = (string)reader["kategoria"],
                            Tresc = (string)reader["tresc"],
                            Data_utw = (DateTime)reader["data_utw"],
                            Data_ed = (DateTime)reader["data_ed"]
                        };
                        result.Add(tmp);
                    }
                }
            }
            return result;
        }

        public static List<Uzytkownicy> SelectRecordsUzytkownicy() // sposob z mapowaniem
        {
            List<Uzytkownicy> result = new List<Uzytkownicy>();
            using (NpgsqlConnection conn = GetConnection())
            {
                string query = @"SELECT * FROM uzytkownicy";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                conn.Open();
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Uzytkownicy tmp = new Uzytkownicy
                        {
                            Id = int.Parse(reader["id"].ToString()),
                            Login = (string)reader["login"],
                            Haslo = (string)reader["haslo"],
                            Imie = (string)reader["imie"],
                            Nazwisko = (string)reader["nazwisko"],
                            Email = (string)reader["email"],
                            Data_ur = (DateTime)reader["data_ur"],
                            Uprawnienia = (string)reader["uprawnienia"]
                        };
                        result.Add(tmp);
                    }
                }
            }
            return result;
        }

        public static void TestConnection()
        {
            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("Polaczono z baza danych");
                }
                conn.Close();
            }
        }

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(@"Server=sxterm;Port=5432;User ID=adrianrodzic;Password=ADrian8151!@#;Database=adrianrodzic");
        }
    }
}