using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DesafioThingPink
{
    class SearchItemDesign : INotifyPropertyChanged
    {
        private string _search_text;
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string search_text
        {
            get
            {
                return _search_text;
            }
            set
            {
                _search_text = value;
                RaisePropertyChanged("search_text");
            }
        }

        public SearchItemDesign(SearchItem search_item)
        {
            string min_timestamp_string = UniversalAppUtil.UnixTimeStampToDateTime(search_item.min_timestamp).ToString("dd/MM/yy");
            string max_timestamp_string = UniversalAppUtil.UnixTimeStampToDateTime(search_item.max_timestamp).ToString("dd/MM/yy");

            if (search_item.min_timestamp != UniversalAppUtil.DateTimeMinValue && search_item.max_timestamp != UniversalAppUtil.DateTimeMaxValue)
                search_text = String.Format("{0} - de {1} até {2}", search_item.location, min_timestamp_string, max_timestamp_string);
            else if (search_item.min_timestamp != UniversalAppUtil.DateTimeMinValue)
                search_text = String.Format("{0} - de {1}", search_item.location, min_timestamp_string);
            else if (search_item.max_timestamp != UniversalAppUtil.DateTimeMaxValue)
                search_text = String.Format("{0} - até {1}", search_item.location, max_timestamp_string);
            else
                search_text = String.Format("{0}", search_item.location);
        }

    }
}
