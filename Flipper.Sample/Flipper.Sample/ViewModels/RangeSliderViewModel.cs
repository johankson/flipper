using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flipper.Sample.ViewModels
{
    public class RangeSliderViewModel : INotifyPropertyChanged
    {
        private double _leftValue;
        public double LeftValue
        {
            get { return _leftValue; }
            set 
            { 
                _leftValue = value; 
                NotifyPropertyChanged("LeftValue"); 
            }
        }

        private double _rightValue;
        public double RightValue
        {
            get 
            { 
                return _rightValue; 
            }
            set 
            { 
                _rightValue = value; 
                NotifyPropertyChanged("RightValue"); 
            }
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
