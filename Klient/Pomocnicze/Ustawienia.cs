namespace Klient
{
    /// <summary>
    /// Klasa pomocnicza, ktorej celem jest umozliwienie zapisania danych do pamieci podrecznej aplikacji, tak aby po wznowieniu jej dzialania uzytkownik
    /// nie utracil zapisanych danych
    /// </summary>
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
