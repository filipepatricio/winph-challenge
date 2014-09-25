using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace DesafioThingPink
{
    class ApiRequests
    {
        //INSTA EXAMPLE: https://api.instagram.com/v1/media/search?lat=41.1462127&lng=-8.6064951&min_timestamp=1395014400&max_timestamp=1395097200&client_id=a2d145d8c2e248b786ac778920e0437e
        //GOOGLE MAPS EXAMPLE: http://maps.google.com/maps/api/geocode/json?address=Porto,%20Portugal&sensor=false

        private const string API_INSTAGRAM_URL = "https://api.instagram.com/v1";
        private const string INSTA_SEARCH_URL = "/media/search";
        private const string INSTA_CLIENT_ID = "a2d145d8c2e248b786ac778920e0437e";
        private const string API_GOOGLE_MAPS = "http://maps.google.com/maps/api";
        private const string GOOGLE_MAPS_SEARCH = "/geocode/json";

        private static string GetResponseContent(HttpWebResponse response)
        {
            Stream responseStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8);
            string responseContent = readStream.ReadToEnd();
            return responseContent;
        }

        public void GetInstaImages(double lat, double lng, double min_timestamp, double max_timestamp)
        {
            string url = String.Format("{0}{1}?lat={2}&lng={3}&min_timestamp={4}&max_timestamp={5}&client_id={6}", API_INSTAGRAM_URL, INSTA_SEARCH_URL, lat.ToString(), lng.ToString(), min_timestamp.ToString(), max_timestamp.ToString(), INSTA_CLIENT_ID);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //request.Headers[x_api_key] = APP_ID;
            request.Method = "GET";

            request.BeginGetResponse(InstaImagesCallback, request);
        }

        private void InstaImagesCallback(IAsyncResult ar)
        {
            HttpWebRequest request = (HttpWebRequest)ar.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ar);

            try
            {
                string response_string = GetResponseContent(response);
                Debug.WriteLine(response_string);
                //WeatherResponse forecastResponse = JsonUtil.Deserialize<WeatherResponse>(response_string);

                //lat = forecastResponse.city.coord.lat;
                //lon = forecastResponse.city.coord.lon;
                //Deployment.Current.Dispatcher.BeginInvoke(() =>
                //{
                //    map.Center = new System.Device.Location.GeoCoordinate(lat, lon);
                //    map.ZoomLevel = 10;
                //});


                //GetForecast(response_string);

                //apiCall.GetSevenDaysForecastByCoords(lat, lon, SevenDaysForecastCallBack);

            }
            catch (WebException webex)
            {
                Debug.WriteLine(response.StatusCode);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
