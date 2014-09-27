using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using DesafioThingPink.Models;

namespace DesafioThingPink.ViewModels
{
    class SearchItemDesign : INotifyPropertyChanged
    {
        private string _search_text;
        public SearchItem search_item { get; set; }
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
            this.search_item = search_item;

            string min_timestamp_string = UniversalAppUtil.UnixTimeStampToDateTime(search_item.min_timestamp).ToString("dd/MM/yy");
            string max_timestamp_string = UniversalAppUtil.UnixTimeStampToDateTime(search_item.max_timestamp).ToString("dd/MM/yy");

            if (Math.Round(search_item.min_timestamp) < Math.Round(search_item.max_timestamp))
                search_text = String.Format("{0} - de {1} até {2}", search_item.location, min_timestamp_string, max_timestamp_string);
            else
                search_text = String.Format("{0}", search_item.location);
        }

    }
}
