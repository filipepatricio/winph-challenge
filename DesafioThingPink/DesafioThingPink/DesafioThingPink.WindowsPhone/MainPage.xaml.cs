using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DesafioThingPink.ViewModels;
using DesafioThingPink.Models;
using ClassLibrary;
using Windows.ApplicationModel.Background;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DesafioThingPink
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ApiRequests apiRequests;
        ObservableImageItems insta_image_collection;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            apiRequests = new ApiRequests();

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

            this.RegisterBackgroundTask();

            List<SearchItem> roaming_search_list = await UniversalAppUtil.GetSearchItemsFromRoamingSettings();
            RecentSearchList.ItemsSource = new ObservableSearchItems(roaming_search_list);
            RefreshMap(roaming_search_list);
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

                if(google_root.status.Equals("OK"))
                {
                    GoogleLocation google_location = google_root.results[0].geometry.location;
                    lat = google_location.lat;
                    lng = google_location.lng;

                    BasicGeoposition geopos = new BasicGeoposition();
                    geopos.Latitude = lat;
                    geopos.Longitude = lng;
                    Geopoint geopoint = new Geopoint(geopos);
                    MyMap.Center = geopoint;
                    MyMap.Zoom = 7;
                }
                else
                {
                    ShowMessage("Localizacao nao encontrada");
                    return;
                }
            }

            else
            {
                ShowMessage("Introduza uma localizacao");
                return;
            }

            

            double max_timestamp = UniversalAppUtil.DateTimeToUnixTimestamp(UntilDate.Date.DateTime);
            double min_timestamp = UniversalAppUtil.DateTimeToUnixTimestamp(SinceDate.Date.DateTime);

            if (max_timestamp - min_timestamp < 0)
            {
                ShowMessage("Intervalo de tempo inválido");
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
                    ShowMessage("Resultados não encontrados");
                }

                ImageList.ItemsSource = insta_image_collection;

                List<SearchItem> roaming_search_list = await UniversalAppUtil.GetSearchItemsFromRoamingSettings();
                RecentSearchList.ItemsSource = new ObservableSearchItems(roaming_search_list);
                RefreshMap(roaming_search_list);

            });
        }

        private void RefreshMap(List<SearchItem> roaming_search_list)
        {
            int i = 1;
            foreach (SearchItem search_item in roaming_search_list)
            {
                BasicGeoposition geopos = new BasicGeoposition();
                geopos.Latitude = search_item.lat;
                geopos.Longitude = search_item.lng;
                Geopoint geopoint = new Geopoint(geopos);
                MyMap.Center = geopoint;
                MyMap.Zoom = 7;
                MyMap.AddPushpin(geopos, i.ToString());
                i++;
            }
        }

        public static async void ShowMessage(string message)
        {
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }


        private async void RecentSearchList_ItemClick(object sender, ItemClickEventArgs e)
        {
            SearchItemDesign clicked_item = (e.ClickedItem as SearchItemDesign);
            SearchItem search_item = clicked_item.search_item;

            await InstaSearch(search_item.lat, search_item.lng, search_item.location, search_item.max_timestamp, search_item.min_timestamp);

            MainPivot.SelectedIndex = 1;
        }

        private void ImageList_ItemClick(object sender, ItemClickEventArgs e)
        {
            //GO TO IMAGE DETAIL
            ImageItemDesign clicked_image = (e.ClickedItem as ImageItemDesign);
            ImageData image_data = clicked_image.image_data;
            StorageUtil.SaveData("ImageSelected.dat", image_data);

            this.Frame.Navigate(typeof(ImageDetail), image_data);

        }

        private async void RegisterBackgroundTask()
        {
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if( backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity )
            {
                foreach( var task in BackgroundTaskRegistration.AllTasks )
                {
                    if( task.Value.Name == taskName )
                    {
                        task.Value.Unregister( true );
                    }
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.TaskEntryPoint = taskEntryPoint;
                taskBuilder.SetTrigger( new TimeTrigger( 15, false ) );
                var registration = taskBuilder.Register();
            }
        }

        private const string taskName = "BackgroundTask";
        private const string taskEntryPoint = "BackgroundTasks.BackgroundTask";
    }

    
}
