using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klient
{
    public static class Ustawienia
    {
        public static string[] WczytajDaneLogowania()
        {
            Properties.Settings Ustaw = Properties.Settings.Default;
            string[] daneLogowanie = new string[2] { Ustaw.Login, Ustaw.Haslo };
            return daneLogowanie;
        }

        public static void ZapiszDaneLogowania(string login, string haslo)
        {
            Properties.Settings Ustaw = Properties.Settings.Default;
            Ustaw.Login = login;
            Ustaw.Haslo = haslo;
            Ustaw.Save();
        }
    }
}
