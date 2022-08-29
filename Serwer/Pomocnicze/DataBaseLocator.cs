namespace Serwer
{
    /// <summary>
    /// Klasa pomocnicza zawierajaca obiekt bazy danych
    /// </summary>
    public class DataBaseLocator
    {
        public static MyDbContext Context { get; set; }
    }
}
