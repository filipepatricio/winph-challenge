using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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

        double lat, lng;
        double min_timestamp, max_timestamp;

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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            lat = 41.1462127; //Google maps Request
            lng = -8.6064951; //Google maps Request
            min_timestamp = 1395014400;
            max_timestamp = 1395097200;

            string insta_images_response = await apiRequests.GetInstaImages(lat, lng, min_timestamp, max_timestamp, InstaImagesCallback);

            Debug.WriteLine(insta_images_response);

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {

                InstaRootObject insta_root = JsonUtil.Deserialize<InstaRootObject>(insta_images_response);

                insta_image_collection = new ObservableImageItems(insta_root.data);
                ImageList.ItemsSource = insta_image_collection;
            });
        }

        private async void InstaImagesCallback(IAsyncResult ar)
        {
            HttpWebRequest request = (HttpWebRequest)ar.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ar);

            try
            {

                



                //ImageList.ItemsSource = insta_image_collection;

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
