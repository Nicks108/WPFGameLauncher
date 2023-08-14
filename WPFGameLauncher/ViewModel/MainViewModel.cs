using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFGameLauncher.ViewModel
{
    public class MainViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        

        private System.Collections.ObjectModel.ObservableCollection<Model.GameInfo> _gameInfoCollection;
        public System.Collections.ObjectModel.ObservableCollection<Model.GameInfo> GameInfoCollection
        {
            get
            {
                return _gameInfoCollection;
            }
            set
            {
                _gameInfoCollection = value;
                NotifyPropertyChanged("RadioStationsDAB");
            }
        }

        private Model.GameInfo _selectedGameInfo;
        public Model.GameInfo SelectedGameInfo
        {
            get
            {
                return _selectedGameInfo;
            }
            set
            {
                _selectedGameInfo = value;
                NotifyPropertyChanged("SelectedRadioStationDAB");
            }
        }

        public MainViewModel(System.Collections.ObjectModel.ObservableCollection<Model.GameInfo> gameInfoCollection)
        {
            GameInfoCollection = gameInfoCollection;
            if (GameInfoCollection.Count > 0)
            {
                SelectedGameInfo = GameInfoCollection[0];
            }
        }

        // Delete the selected item
        public void Delete()
        {
            //System.Collections.Generic.List<Model.RadioStation> newItems = RadioStationsDAB.ToList();
            //newItems.Remove(SelectedRadioStationDAB);
            //RadioStationsDAB = new System.Collections.ObjectModel.ObservableCollection<Model.RadioStation>(newItems);
            GameInfoCollection.Remove(SelectedGameInfo);
            SelectedGameInfo = GameInfoCollection[0];
        }

        #region INotifyPropertyChanged

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged
    }
}
