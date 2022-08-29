using System.ComponentModel;

namespace Klient
{
    /// <summary>
    /// Klasa odpowiadajaca za implementacje interfejsu INotifyPropertyChanged z ktorej pozniej dziedzicza klasy robiace za model widoku
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

        protected void OnPropertyChanged(string nazwa)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(nazwa));
        }
    }
}
