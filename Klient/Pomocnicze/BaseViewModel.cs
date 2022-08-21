using System.ComponentModel;

namespace Klient
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

        protected void OnPropertyChanged(string nazwa)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(nazwa));
        }
    }
}
