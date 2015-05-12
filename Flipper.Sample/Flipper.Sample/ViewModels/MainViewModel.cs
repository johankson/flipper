using Flipper.Sample.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

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

        int _itemCount;
        public int ItemCount
        {
            get { return _itemCount; }
            set { _itemCount = value; NotifyPropertyChanged("ItemCount"); }
        }

        int _index;
        public int Index
        {
            get { return _index; }
            set { 
                _index = value; 
                NotifyPropertyChanged("Index");
                Status = String.Format("{0} of {1}", _index + 1, this.Items.Count);
            }
        }


        string _status;
        public string Status
        {
            get { return _status; }
            set { _status = value; NotifyPropertyChanged("Status"); }
        }
        


        public ICommand EndIsNearCommand
        {
            get
            {
                return new Command(() =>
                {
                    // The end is near, add more random images
                    MainView.AddFiveRandomImages(Items);
                });
            }
        }

        public ICommand GotoRandom
        {
            get
            {
                return new Command(() =>
                {
                    var r = new Random();
                    Index = r.Next(0, Items.Count - 1);
                });
            }
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
