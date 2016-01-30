using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace dron.Helpers
{

    public class PropertyChangedNotification : INotifyPropertyChanged
    {
        /// <summary>
        /// Event informujący o zmianie wartości pola
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Wywołanie eventu informującego o zmianie wartości pola
        /// </summary>
        /// <param name="propertyName">Nazwa pola, które zmieniło swą wartość</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Zmiana wartości property
        /// </summary>
        /// <param name="field">Zmieniane pole</param>
        /// <param name="value">Nowa wartość</param>
        /// <param name="propertyName">Nazwa zmienianego pola, dzięki atrybutowi <c>[CallerMemberName]</c> nie trzeba wpisywać tego pola</param>
        /// <typeparam name="T">Typ zmienianego pola</typeparam>
        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            OnPropertyChanged(propertyName);
        }
    }
}
