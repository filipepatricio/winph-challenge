using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DesafioThingPink
{
    class ApiRequests
    {
        //INSTA EXAMPLE: https://api.instagram.com/v1/media/search?lat=41.1462127&lng=-8.6064951&min_timestamp=1395014400&max_timestamp=1395097200&client_id=a2d145d8c2e248b786ac778920e0437e
        //GOOGLE MAPS EXAMPLE: http://maps.google.com/maps/api/geocode/json?address=Porto,%20Portugal&sensor=false

        private const string API_INSTAGRAM_URL = "https://api.instagram.com/v1";
        private const string INSTA_SEARCH_URL = "/media/search";
        private const string INSTA_CLIENT_ID = "a2d145d8c2e248b786ac778920e0437e";

        private const string API_GOOGLE_MAPS_URL = "http://maps.google.com/maps/api";
        private const string GOOGLE_MAPS_SEARCH_URL = "/geocode/json";

        public static string GetResponseContent(HttpWebResponse response)
        {
            Stream responseStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8);
            string responseContent = readStream.ReadToEnd();
            return responseContent;
        }

        public async Task<string> GetInstaImages(string location, double lat, double lng, double min_timestamp, double max_timestamp)
        {
            string url = String.Empty;

            if (min_timestamp != UniversalAppUtil.DateTimeMinValue && max_timestamp != UniversalAppUtil.DateTimeMaxValue)
                url = String.Format("{0}{1}?lat={2}&lng={3}&min_timestamp={4}&max_timestamp={5}&client_id={6}", API_INSTAGRAM_URL, INSTA_SEARCH_URL, lat.ToString(), lng.ToString(), min_timestamp.ToString(), max_timestamp.ToString(), INSTA_CLIENT_ID);
            else if (min_timestamp != UniversalAppUtil.DateTimeMinValue)
                url = String.Format("{0}{1}?lat={2}&lng={3}&min_timestamp={4}&client_id={5}", API_INSTAGRAM_URL, INSTA_SEARCH_URL, lat.ToString(), lng.ToString(), min_timestamp.ToString(), INSTA_CLIENT_ID);
            else if (max_timestamp != UniversalAppUtil.DateTimeMaxValue)
                url = String.Format("{0}{1}?lat={2}&lng={3}&max_timestamp={4}&client_id={5}", API_INSTAGRAM_URL, INSTA_SEARCH_URL, lat.ToString(), lng.ToString(), max_timestamp.ToString(), INSTA_CLIENT_ID);
            else
                url = String.Format("{0}{1}?lat={2}&lng={3}&client_id={4}", API_INSTAGRAM_URL, INSTA_SEARCH_URL, lat.ToString(), lng.ToString(), INSTA_CLIENT_ID);

            //TODO: SAVE Search on Roaming settings
            UniversalAppUtil.AddSearchItemToRoamingSettings(new SearchItem(location, lat, lng, min_timestamp, max_timestamp));


            return await DoRequest(url);
        }

        public async Task<string> GetGoogleCoords(string address)
        {
            string url = String.Format("{0}{1}?address={2}&sensor=false", API_GOOGLE_MAPS_URL, GOOGLE_MAPS_SEARCH_URL, address);
            return await DoRequest(url);
        }

        private async Task<string> DoRequest(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

            string response_string = ApiRequests.GetResponseContent(response);

            //Debug.WriteLine(response_string);

            return response_string;
        }

    }
}
