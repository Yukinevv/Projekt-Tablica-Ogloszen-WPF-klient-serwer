using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Projekt
{
    class Operacje
    {
        public static int PoliczOgloszenia()
        {
            int iloscOgloszen = 0;
            using (NpgsqlConnection conn = Connect.GetConnection())
            {
                try
                {
                    string query = @"SELECT COUNT(*) AS ile FROM ogloszenia";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    conn.Open();

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            iloscOgloszen = int.Parse(reader["ile"].ToString());
                        }
                    }
                }
                catch (Exception)
                {
                    //MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak", MessageBoxButton.OK, MessageBoxImage.Error);
                    Debug.WriteLine("Blad: PoliczOgloszenia");
                }
            }
            return iloscOgloszen;
        }

        public static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
