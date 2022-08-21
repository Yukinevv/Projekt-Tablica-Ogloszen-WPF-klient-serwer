using System.Windows;
using System.Windows.Input;

namespace Klient
{
    public class PanelAdminaUzytkownicyModelWidoku : BaseViewModel
    {
        public string TextBoxImieUzytkownikaModelWidoku { get; set; }
        public string TextBoxNazwiskoUzytkownikaModelWidoku { get; set; }
        public string TextBoxLoginUzytkownikaModelWidoku { get; set; }
        public string TextBoxEmailUzytkownikaModelWidoku { get; set; }
        public string TextBoxDataUrodzeniaUzytkownikaModelWidoku { get; set; }
        public string TextBoxRolaUzytkownikaModelWidoku { get; set; }

        public ICommand PowrotKomenda { get; set; }
        public ICommand AwansujKomenda { get; set; }
        public ICommand ZdegradujKomenda { get; set; }

        public PanelAdminaUzytkownicyModelWidoku()
        {
            PowrotKomenda = new RelayCommand(Powrot);
            AwansujKomenda = new RelayCommand(Awansuj);
            ZdegradujKomenda = new RelayCommand(Zdegraduj);

            TextBoxImieUzytkownikaModelWidoku = PanelAdminaMenuModelWidoku.WybranyUzytkownik.Imie;
            TextBoxNazwiskoUzytkownikaModelWidoku = PanelAdminaMenuModelWidoku.WybranyUzytkownik.Nazwisko;
            TextBoxLoginUzytkownikaModelWidoku = PanelAdminaMenuModelWidoku.WybranyUzytkownik.Login;
            TextBoxEmailUzytkownikaModelWidoku = PanelAdminaMenuModelWidoku.WybranyUzytkownik.Email;
            TextBoxDataUrodzeniaUzytkownikaModelWidoku = PanelAdminaMenuModelWidoku.WybranyUzytkownik.Data_ur.ToString();
            TextBoxRolaUzytkownikaModelWidoku = PanelAdminaMenuModelWidoku.WybranyUzytkownik.Uprawnienia;
        }

        private void Powrot(object x)
        {
            PanelAdmina.Rama.Content = new PanelAdminaMenu();
        }

        private void Awansuj(object x)
        {
            OperacjeKlient.Wyslij("AWANS UZYTKOWNIKA");
            OperacjeKlient.Wyslij(TextBoxLoginUzytkownikaModelWidoku);

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "awansowano")
            {
                TextBoxRolaUzytkownikaModelWidoku = "admin";
                MessageBox.Show("Użytkownik został awansowany na administratora!");
            }
            else
            {
                MessageBox.Show(odpowiedz);
            }
        }

        private void Zdegraduj(object x)
        {
            OperacjeKlient.Wyslij("ZDEGRADOWANIE UZYTKOWNIKA");
            OperacjeKlient.Wyslij(TextBoxLoginUzytkownikaModelWidoku);

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "zdegradowano")
            {
                TextBoxRolaUzytkownikaModelWidoku = "uzytkownik";
                MessageBox.Show("Użytkownik administrator zostal zdegradowany do rangi zwykłego użytkownika!");
            }
            else
            {
                MessageBox.Show(odpowiedz);
            }
        }
    }
}
