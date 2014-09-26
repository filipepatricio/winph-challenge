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
        ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;

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
            //lat = 0;//41.1462127; //Google maps Request
            //lng = 0;//-8.6064951; //Google maps Request
            //min_timestamp = 1395014400;
            //max_timestamp = 1395097200;
            string insta_images_response = String.Empty;
            
            
            


            if(!LocationTextBox.Equals(String.Empty))
            {
                string google_coords_response = await apiRequests.GetGoogleCoords(LocationTextBox.Text);
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
                    MapControl.Center = geopoint;
                    MapControl.ZoomLevel = 10;
                    MapIcon pin = new MapIcon();
                    pin.Location = geopoint;
                    MapControl.MapElements.Add(pin);
                }
                else
                {
                    ShowMessage("Localizacao nao encontrada");
                    return;
                }

            }

            if (max_timestamp - min_timestamp < 0)
            {
                ShowMessage("Intervalo de tempo inválido");
                return;
            }
            else
            {
                insta_images_response = await apiRequests.GetInstaImages(lat, lng, min_timestamp, max_timestamp);
            }

            Debug.WriteLine(insta_images_response);

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {

                InstaRootObject insta_root = JsonUtil.Deserialize<InstaRootObject>(insta_images_response);

                insta_image_collection = new ObservableImageItems(insta_root.data);

                if (insta_root.data.Count == 0)
                {
                    ShowMessage("Resultados não encontrados");
                }

                ImageList.ItemsSource = insta_image_collection;


            });
        }

        private async void ShowMessage(string message)
        {
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }

        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            //http://stackoverflow.com/questions/249760/how-to-convert-unix-timestamp-to-datetime-and-vice-versa
            return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

        private void SinceDate_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            min_timestamp = DateTimeToUnixTimestamp(SinceDate.Date.DateTime);
        }

        private void UntilDate_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            max_timestamp = DateTimeToUnixTimestamp(UntilDate.Date.DateTime);
        }

    }
}
