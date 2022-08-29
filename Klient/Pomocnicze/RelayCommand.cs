using System;
using System.Windows.Input;

namespace Klient
{
    /// <summary>
    /// Klasa pomocnicza implementujaca interfejs ICommand. Potrzebna aby dla wlasnosci typu ICommand uzywanych w modelach widoku mozna bylo utworzyc
    /// instancje gdzie w konstruktorze jako argument podajemy metode, ktora ma byc wykonywana przy okreslonym zbindowanym z kontrolka zdarzeniu.
    /// Inaczej obsluga zdarzen dla wzorca MVVM bez koniecznosci programowania zdarzeniowego.
    /// </summary>
    public class RelayCommand : ICommand
    {
        public Action<object> mAction;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> mAction)
        {
            this.mAction = mAction;
        }   

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            mAction(parameter);
        }
    }
}
