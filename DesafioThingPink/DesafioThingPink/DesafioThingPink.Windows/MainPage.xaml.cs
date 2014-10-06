using ClassLibrary;
using DesafioThingPink.Common;
using DesafioThingPink.Models;
using DesafioThingPink.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Hub Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=321224

namespace DesafioThingPink
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private TextBox LocationTextBox;
        private DatePicker SinceDate;
        private DatePicker UntilDate;
        private ListView ImageList;
        private ListView RecentSearchList;
        private MapView MyMap;

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
        }


        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Assign a collection of bindable groups to this.DefaultViewModel["Groups"]
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);

            this.RegisterBackgroundTask();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void LocationTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            LocationTextBox = (TextBox)sender;
        }

        private void SinceDate_Loaded(object sender, RoutedEventArgs e)
        {
            SinceDate = (DatePicker)sender;
        }

        private void UntilDate_Loaded(object sender, RoutedEventArgs e)
        {
            UntilDate = (DatePicker)sender;
        }

        private void ImageList_Loaded(object sender, RoutedEventArgs e)
        {
            ImageList = (ListView)sender;
        }

        private async void RecentSearchList_Loaded(object sender, RoutedEventArgs e)
        {
            RecentSearchList = (ListView)sender;
            roaming_search_list = await UniversalAppUtil.GetSearchItemsFromRoamingSettings();
            RecentSearchList.ItemsSource = new ObservableSearchItems(roaming_search_list);
        }

        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            MyMap = (MapView)sender;

            RefreshMap(roaming_search_list);
        }


        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
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
                    MyMap.Center = geopoint;
                    MyMap.Zoom = 7;
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

        private void ImageList_ItemClick(object sender, ItemClickEventArgs e)
        {
            //GO TO IMAGE DETAIL
            ImageItemDesign clicked_image = (e.ClickedItem as ImageItemDesign);
            ImageData image_data = clicked_image.image_data;

            this.Frame.Navigate(typeof(ImageDetail), image_data);
        }

        private async void RecentSearchList_ItemClick(object sender, ItemClickEventArgs e)
        {
            SearchItemDesign clicked_item = (e.ClickedItem as SearchItemDesign);
            SearchItem search_item = clicked_item.search_item;

            await InstaSearch(search_item.lat, search_item.lng, search_item.location, search_item.max_timestamp, search_item.min_timestamp);

            //MainPivot.SelectedIndex = 1;
        }


    }
}
