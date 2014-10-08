using ClassLibrary;
using DesafioThingPink.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.Web.Syndication;

namespace BackgroundTasks
{
    public sealed class BackgroundTask: IBackgroundTask
    {
        static ApiRequests apiRequests = new ApiRequests();
        CancellationTokenSource cts = null;
        static string textElementName = "text";

        public async void Run( IBackgroundTaskInstance taskInstance )
        {
            // Get a deferral, to prevent the task from closing prematurely 
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            var settings = ApplicationData.Current.LocalSettings;

            double lat = 0.0;
            double lng = 0.0;
 
            try 
            { 
                // Associate a cancellation handler with the background task. 
                taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled); 
 
                // Get cancellation token 
                if (cts == null) 
                { 
                    cts = new CancellationTokenSource(); 
                } 
                CancellationToken token = cts.Token; 
 
                // Create geolocator object 
                Geolocator geolocator = new Geolocator(); 
 
                // Make the request for the current position 
                Geoposition pos = await geolocator.GetGeopositionAsync().AsTask(token); 
 
                DateTime currentTime = DateTime.Now; 
 
                WriteStatusToAppData("Time: " + currentTime.ToString()); 
                WriteGeolocToAppData(pos);

                lat = (double)settings.Values["Latitude"];
                lng = (double)settings.Values["Longitude"];

            } 
            catch (UnauthorizedAccessException) 
            { 
                WriteStatusToAppData("Disabled"); 
                WipeGeolocDataFromAppData();

                GetLastCoordsSearched(ref lat, ref lng);
            } 
            catch (Exception ex) 
            { 
//#if WINDOWS_APP 
//                // If there are no location sensors GetGeopositionAsync() 
//                // will timeout -- that is acceptable. 
//                const int WaitTimeoutHResult = unchecked((int)0x80070102); 
 
//                if (ex.HResult == WaitTimeoutHResult) // WAIT_TIMEOUT 
//                { 
//                    WriteStatusToAppData("An operation requiring location sensors timed out. Possibly there are no location sensors."); 
//                } 
//                else 
//#endif 
//                { 
                    WriteStatusToAppData(ex.ToString()); 
                //} 
 
                WipeGeolocDataFromAppData();

                GetLastCoordsSearched(ref lat, ref lng);
            } 
            finally 
            { 
                cts = null;
            }

            try
            {

                // Download the feed.
                var feed = await GetInstaImages(lat, lng);

                // Update the live tile with the feed items.
                UpdateTile(feed);

            }catch
            {
                return;
            }

            // Inform the system that the task is finished.
            deferral.Complete();



        }

        private static void GetLastCoordsSearched(ref double lat, ref double lng)
        {
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            try
            {
                ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)roamingSettings.Values.LastOrDefault().Value;
                lat = (double)composite["lat"];
                lng = (double)composite["lng"];
            }
            catch
            {
                return;
            }
        }

        private void WriteGeolocToAppData(Geoposition pos)
        {
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["Latitude"] = pos.Coordinate.Point.Position.Latitude;
            settings.Values["Longitude"] = pos.Coordinate.Point.Position.Longitude;
            settings.Values["Accuracy"] = pos.Coordinate.Accuracy.ToString();
        }

        private void WipeGeolocDataFromAppData()
        {
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["Latitude"] = "";
            settings.Values["Longitude"] = "";
            settings.Values["Accuracy"] = "";
        }

        private void WriteStatusToAppData(string status)
        {
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["Status"] = status;
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }
        } 

        private static async Task<InstaRootObject> GetInstaImages(double lat, double lng)
        {

            string insta_images_response = String.Empty;
            string location = "DeviceLocation";

            insta_images_response = await apiRequests.GetInstaImages(location, lat, lng);

            Debug.WriteLine(insta_images_response);

            //if (insta_images_response != null)
            InstaRootObject insta_root = JsonUtil.Deserialize<InstaRootObject>(insta_images_response);

            return insta_root;

        }

        private static void UpdateTile( InstaRootObject feed )
        {
            //Create a tile update manager for the specified syndication feed.
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue( true );
            updater.Clear();

            // Keep track of the number feed items that get tile notifications. 
            //int itemCount = 0;

            var item = feed.data[0];
            // Create a tile notification for each feed item.

            TileTemplateType[] tile_template_array = new TileTemplateType[3];
            tile_template_array[0] = TileTemplateType.TileSquare150x150Image;
            tile_template_array[1] = TileTemplateType.TileSquare310x310Image;
            tile_template_array[2] = TileTemplateType.TileWide310x150Image;

            foreach (TileTemplateType template in tile_template_array)
            {
                XmlDocument tileXml = TileUpdateManager.GetTemplateContent(template);

                var title = item.caption;
                string titleText = title.text == null ? String.Empty : title.text;
                //tileXml.GetElementsByTagName( textElementName )[0].InnerText = titleText;
                XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");
                ((XmlElement)tileImageAttributes[0]).SetAttribute("src", item.images.thumbnail.url);
                ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "instagram image");

                // Create a new tile notification. 
                updater.Update(new TileNotification(tileXml));
            }


            
        }

    }
    
}
