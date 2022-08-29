using Microsoft.Xaml.Behaviors;
using System;
using System.Windows.Input;

namespace Klient
{
    /// <summary>
    /// Klasa pomocnicza bedaca rozszerzeniem dla klasy EventTrigger. Dzieki tej konkretnej implementacji komenda zostanie wykonana po wcisnieciu
    /// klawisza Enter
    /// </summary>
    public class EnterKeyDownEventTrigger : EventTrigger
    {
        public EnterKeyDownEventTrigger() : base("KeyDown")
        {
        }

        protected override void OnEvent(EventArgs eventArgs)
        {
            var e = eventArgs as KeyEventArgs;
            if (e != null && e.Key == Key.Enter)
            {
                this.InvokeActions(eventArgs);
            }     
        }
    }
}
