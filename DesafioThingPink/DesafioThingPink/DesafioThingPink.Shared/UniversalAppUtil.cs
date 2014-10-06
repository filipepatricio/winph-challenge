using DesafioThingPink.Manager;
using DesafioThingPink.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

namespace DesafioThingPink
{
    class UniversalAppUtil
    {
        //public static double DateTimeMinValue = DateTimeToUnixTimestamp(DateTime.MinValue);
        //public static double DateTimeMaxValue = DateTimeToUnixTimestamp(DateTime.Now);

        public static FacebookManager _fbm;

        public const string UniversalSearchItems = "UniversalSearchItems";

        public static ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
        public static StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;

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

            //List<SearchItem> search_list = await GetSearchItemsFromRoamingSettings();

            //search_list.Add(search_item);

            //await StorageUtil.SaveData("SEARCH_LIST.dat", roamingFolder, search_list);

            ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();
            composite["location"] = search_item.location;
            composite["lat"] = search_item.lat;
            composite["lng"] = search_item.lng;
            composite["min_timestamp"] = search_item.min_timestamp;
            composite["max_timestamp"] = search_item.max_timestamp;

            roamingSettings.Values[roamingSettings.Values.Count.ToString()] = composite;
            
        }

        public async static Task<List<SearchItem>> GetSearchItemsFromRoamingSettings()
        {
            //object search_list_obj = await StorageUtil.LoadData("SEARCH_LIST.dat", roamingFolder, typeof(List<SearchItem>));
            //List<SearchItem> search_list = (search_list_obj as List<SearchItem>);

            //if (search_list == null)
            //    search_list = new List<SearchItem>();

            List<SearchItem> search_list = new List<SearchItem>();

            for (int i = 0; i < roamingSettings.Values.Values.Count; i++)
            {
                //Workaround
                ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)roamingSettings.Values[i.ToString()];
                SearchItem search_item = new SearchItem((string)composite["location"], (double)composite["lat"], (double)composite["lng"], (double)composite["min_timestamp"], (double)composite["max_timestamp"]);
                search_list.Add(search_item);
            }
            search_list.Reverse();

            return search_list;

        }

        public static async void ShowMessage(string message)
        {
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }
    }
}
