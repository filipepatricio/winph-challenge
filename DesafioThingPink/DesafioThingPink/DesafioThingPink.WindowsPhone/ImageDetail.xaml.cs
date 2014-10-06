using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using DesafioThingPink.ViewModels;
using DesafioThingPink.Models;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using System.Net.Http;
using System.Threading.Tasks;
using DesafioThingPink.Manager;
using DesafioThingPink.Misc;
using Windows.ApplicationModel.Activation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace DesafioThingPink
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImageDetail : Page, IWebAuthenticationBrokerContinuable
    {

        public ImageDetail()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            image_data = (e.Parameter as ImageData);
            bitmap_image = new BitmapImage(new Uri(image_data.images.standard_resolution.url, UriKind.Absolute));
            Image.Source = bitmap_image;
            UsernameText.Text = "@" + image_data.user.username;
            LikesText.Text = image_data.likes.count + " Likes";

            ObservableCommentItems comment_collection = new ObservableCommentItems(image_data.comments.data);

            CommentList.ItemsSource = comment_collection;
            
        }

        private void BackTapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void SaveImageOnCameraRoll_Click(object sender, RoutedEventArgs e)
        {

            await SaveImageOnLibrary();

        }

        private void ShareOnFacebook_Click(object sender, RoutedEventArgs e)
        {
            if (UniversalAppUtil._fbm == null)
            {
                UniversalAppUtil._fbm = new FacebookManager();
                UniversalAppUtil._fbm.LoginAndContinue();
            }

            if (UniversalAppUtil._fbm._fb.AccessToken == null)
                UniversalAppUtil._fbm.LoginAndContinue();

            PublishStory();
            
        }

        public void ContinueWithWebAuthenticationBroker(WebAuthenticationBrokerContinuationEventArgs args)
        {
            UniversalAppUtil._fbm.ContinueAuthentication(args);
            //txtFbToken.Text = _fbm.AccessToken;

            PublishStory();
        }
    }
}
