using System;
using System.Collections.Generic;
using System.Text;

namespace DesafioThingPink.Models
{
    class SearchItem
    {
        public string location {get; set;}
        public double lat { get; set; }
        public double lng { get; set; }
        public double min_timestamp { get; set; }
        public double max_timestamp { get; set; }

        public SearchItem(string location, double lat, double lng, double min_timestamp, double max_timestamp)
        {
            this.location = location;
            this.lat = lat;
            this.lng = lng;
            this.min_timestamp = min_timestamp;
            this.max_timestamp = max_timestamp;
        }
    }
}
