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

        public static List<Ogloszenia> SelectRecordsOgloszenia()
        {
            List<Ogloszenia> result = new List<Ogloszenia>();
            using (NpgsqlConnection conn = GetConnection())
            {
                string query = @"SELECT * FROM Ogloszenia";
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
                            Tresc = (string)reader["tresc"],
                            //Data_utw = (DateTime)reader["data_utw"],
                            //Data_ed = (DateTime)reader["data_ed"]
                            Data_utw = (string)reader["data_utw"],
                            Data_ed = (string)reader["data_ed"]
                        };

                        result.Add(tmp);
                    }
                }
            }
            return result;
        }

        public static List<Uzytkownicy> SelectRecordsUzytkownicy()
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
                            //Data_ur = (DateTime)reader["data_ur"],
                            Data_ur = (string)reader["data_ur"],
                            Uprawnienia = (string)reader["uprawnienia"]
                        };
                        result.Add(tmp);
                    }
                }
            }
            return result;
        }

        public static List<Kategoria> SelectRecordsKategoria()
        {
            List<Kategoria> result = new List<Kategoria>();
            using (NpgsqlConnection conn = GetConnection())
            {
                string query = @"SELECT * FROM kategoria";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                conn.Open();
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Kategoria tmp = new Kategoria
                        {
                            Id_k = int.Parse(reader["id_k"].ToString()),
                            Nazwa = (string)reader["nazwa"],
                            Id_u = int.Parse(reader["id_u"].ToString()),
                            //Data_utw = (DateTime)reader["data_utw"],
                            Data_utw = (string)reader["data_utw"]
                        };
                        result.Add(tmp);
                    }
                }
            }
            return result;
        }

        public static List<string> SelectRecordsKategoria2()
        {
            List<string> result = new List<string>();
            using (NpgsqlConnection conn = GetConnection())
            {
                string query = @"SELECT nazwa FROM kategoria";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                conn.Open();
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add((string)reader["nazwa"]);
                    }
                }
            }
            return result;
        }
        public static List<KatToOgl> SelectRecordsKatToOgl()
        {
            List<KatToOgl> result = new List<KatToOgl>();
            using (NpgsqlConnection conn = GetConnection())
            {
                string query = @"SELECT * FROM kattoogl";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                conn.Open();
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        KatToOgl tmp = new KatToOgl
                        {
                            Id_o= int.Parse(reader["id_o"].ToString()),
                            Id_k = int.Parse(reader["id_k"].ToString())
                        };
                        result.Add(tmp);
                    }
                }
            }
            return result;
        }

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(@"Server=sxterm;Port=5432;User ID=adrianrodzic;Password=ADrian8151!@#;Database=adrianrodzic");
        }
    }
}