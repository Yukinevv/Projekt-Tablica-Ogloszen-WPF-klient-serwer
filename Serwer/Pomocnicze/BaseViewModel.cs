using System.ComponentModel;

namespace Serwer
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
