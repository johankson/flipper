using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flipper.Sample.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        ObservableCollection<string> _items;
        public ObservableCollection<string> Items
        {
            get { return _items; }
            set { _items = value; NotifyPropertyChanged("Items"); }
        }
        
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
