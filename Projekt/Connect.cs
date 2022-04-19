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
                        result.Add(reader["id_o"] + " " + reader["id_u"] + " " + reader["tytul"] + " " + reader["kategoria"] + " "
                            + reader["data_utw"] + " " + reader["data_ed"] + " " + reader["tresc"]);
                    }
                }
            }
            return result;
        }

        /*public static List<Ogloszenia> SelectRecordsOgloszenia2() // testowo
        {
            List<Ogloszenia> result = new List<Ogloszenia>();
            using (NpgsqlConnection conn = GetConnection())
            {
                string query = @"SELECT * FROM ogloszenia";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                conn.Open();
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    Ogloszenia tmp = new Ogloszenia();

                    while (reader.Read())
                    {
                        tmp.Id_o = int.Parse(reader["id_o"].ToString());
                        tmp.Id_u = int.Parse(reader["id_u"].ToString());
                        tmp.Tytul = (string)reader["tytul"];
                        tmp.Kategoria = (string)reader["kategoria"];
                        tmp.Tresc = (string)reader["tresc"];
                        tmp.Data_utw = (DateTime)reader["data_utw"];
                        tmp.Data_ed = (DateTime)reader["data_ed"];

                        result.Add(tmp);
                    }
                }
            }
            return result;
        }*/

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