using ClassLibrary;
using DesafioThingPink.Models;
using DesafioThingPink.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DesafioThingPink
{
    public sealed partial class ImageListControl : UserControl
    {
        ApiRequests apiRequests = new ApiRequests();
        ObservableImageItems insta_image_collection;

        public ImageListControl()
        {
            this.InitializeComponent();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            //lat = 0;//41.1462127; //Google maps Request
            //lng = 0;//-8.6064951; //Google maps Request
            //min_timestamp = 1395014400;
            //max_timestamp = 1395097200;

            double lat = 0;
            double lng = 0;
            string location = LocationTextBox.Text;

            if (!location.Equals(String.Empty))
            {
                string google_coords_response = await apiRequests.GetGoogleCoords(location);
                GoogleRootObject google_root = JsonUtil.Deserialize<GoogleRootObject>(google_coords_response);

                if (google_root.status.Equals("OK"))
                {
                    GoogleLocation google_location = google_root.results[0].geometry.location;
                    lat = google_location.lat;
                    lng = google_location.lng;

                    BasicGeoposition geopos = new BasicGeoposition();
                    geopos.Latitude = lat;
                    geopos.Longitude = lng;
                    Geopoint geopoint = new Geopoint(geopos);
                    //MyMap.Center = geopoint;
                    //MyMap.Zoom = 7;
                }
                else
                {
                    UniversalAppUtil.ShowMessage("Localizacao nao encontrada");
                    return;
                }
            }

            else
            {
                UniversalAppUtil.ShowMessage("Introduza uma localizacao");
                return;
            }



            double max_timestamp = UniversalAppUtil.DateTimeToUnixTimestamp(UntilDate.Date.DateTime);
            double min_timestamp = UniversalAppUtil.DateTimeToUnixTimestamp(SinceDate.Date.DateTime);

            if (max_timestamp - min_timestamp < 0)
            {
                UniversalAppUtil.ShowMessage("Intervalo de tempo inválido");
                return;
            }

            await InstaSearch(lat, lng, location, max_timestamp, min_timestamp);



        }

        private async Task InstaSearch(double lat, double lng, string location, double max_timestamp, double min_timestamp)
        {
            string insta_images_response = String.Empty;
            insta_images_response = await apiRequests.GetInstaImages(location, lat, lng, min_timestamp, max_timestamp);

            UniversalAppUtil.AddSearchItemToRoamingSettings(new SearchItem(location, lat, lng, min_timestamp, max_timestamp));

            Debug.WriteLine(insta_images_response);

            if (insta_images_response == null)
                return;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {

                InstaRootObject insta_root = JsonUtil.Deserialize<InstaRootObject>(insta_images_response);

                insta_image_collection = new ObservableImageItems(insta_root.data);

                if (insta_root.data.Count == 0)
                {
                    UniversalAppUtil.ShowMessage("Resultados não encontrados");
                }

                ImageList.ItemsSource = insta_image_collection;

                List<SearchItem> roaming_search_list = await UniversalAppUtil.GetSearchItemsFromRoamingSettings();
                //RecentSearchList.ItemsSource = new ObservableSearchItems(roaming_search_list);
                //RefreshMap(roaming_search_list);

            });
        }

        private void ImageList_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

    }
}
