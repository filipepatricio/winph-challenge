using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace DesafioThingPink
{
    class UniversalAppUtil
    {
        //public static double DateTimeMinValue = DateTimeToUnixTimestamp(DateTime.MinValue);
        //public static double DateTimeMaxValue = DateTimeToUnixTimestamp(DateTime.Now);

        public const string UniversalSearchItems = "UniversalSearchItems";

        public static ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;

        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            //http://stackoverflow.com/questions/249760/how-to-convert-unix-timestamp-to-datetime-and-vice-versa
            return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static void AddSearchItemToRoamingSettings(SearchItem search_item)
        {
            ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();
            composite["location"] = search_item.location;
            composite["lat"] = search_item.lat;
            composite["lng"] = search_item.lng;
            composite["min_timestamp"] = search_item.min_timestamp;
            composite["max_timestamp"] = search_item.max_timestamp;

            roamingSettings.Values[roamingSettings.Values.Count.ToString()] = composite;
            
        }

        public static List<SearchItem> GetSearchItemsFromRoamingSettings()
        {
            List<SearchItem> search_item_list = new List<SearchItem>();

            for (int i = 0; i < roamingSettings.Values.Values.Count; i++)
            {
                //Workaround
                ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)roamingSettings.Values[i.ToString()];
                SearchItem search_item = new SearchItem((string)composite["location"], (double)composite["lat"], (double)composite["lng"], (double)composite["min_timestamp"], (double)composite["max_timestamp"]);
                search_item_list.Add(search_item);
            }
            search_item_list.Reverse();

            return search_item_list;

        }
    }
}
