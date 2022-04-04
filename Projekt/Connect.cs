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
            using (NpgsqlConnection con = GetConnection())
            {
                //string query = @"INSERT INTO public.emp(empno, ename, deptno) VALUES(4, 'Kowalski', 10)";
                string query = @"INSERT INTO public.test VALUES(1, 'Jan', 'Kowalski', 123456789)";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
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
            using (NpgsqlConnection con = GetConnection())
            {
                string query = @"SELECT * FROM ogloszenia";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //Console.WriteLine(reader.GetString(0));
                        //Console.WriteLine(reader["id"] + " " + reader["imie"] + " " + reader["nazwisko"] + " " + reader["nr_tel"]);
                        result.Add(reader["id_o"] + " " + reader["id_u"] + " " + reader["tytul"] + " " + reader["kategoria"] + " " + reader["tresc"]);
                    }
                }
            }
            return result;
        }

        public static void TestConnection()
        {
            using (NpgsqlConnection con = GetConnection())
            {
                con.Open();
                if (con.State == System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("Polaczono z baza danych");
                }
            }
        }

        private static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(@"Server=sxterm;Port=5432;User ID=adrianrodzic;Password=ADrian8151!@#;Database=adrianrodzic");
        }
    }
}
