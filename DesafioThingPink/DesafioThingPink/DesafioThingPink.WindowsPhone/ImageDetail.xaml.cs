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
        ImageData image_data;
        BitmapImage bitmap_image = new BitmapImage();

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

            try
            {
                // http://stackoverflow.com/questions/25343475/writeablebitmap-savejpeg-missing-for-universal-apps

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(image_data.images.standard_resolution.url);
                await Task.Run(async () =>
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var ras = new InMemoryRandomAccessStream();
                        var inputBytes = await response.Content.ReadAsByteArrayAsync();
                        var writer = new DataWriter(ras);
                        writer.WriteBytes(inputBytes);
                        await writer.StoreAsync();
                        var inputStream = ras.GetInputStreamAt(0);

                        // write the picture into this folder
                        var storageFile = await KnownFolders.PicturesLibrary.CreateFileAsync(image_data.id + ".jpg", CreationCollisionOption.GenerateUniqueName);
                        using (var storageStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            await RandomAccessStream.CopyAndCloseAsync(inputStream, storageStream.GetOutputStreamAt(0));
                        }
                    }
                });
                MainPage.ShowMessage("Imagem guardada com sucesso");
            }
            catch
            {
                MainPage.ShowMessage("Ocorreu um erro ao guardar a imagem");
            }


            
        }

        private async void ShareOnFacebook_Click(object sender, RoutedEventArgs e)
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

        private async void PublishStory()
        {
            //await this.loginButton.RequestNewPermissions("publish_stream");

            var facebookClient = UniversalAppUtil._fbm._fb;

            

            var postParams = new
            {
                //name = "Facebook SDK for .NET",
                //caption = "Build great social apps and get more installs.",
                //description = "The Facebook SDK for .NET makes it easier and faster to develop Facebook integrated .NET apps.",
                //link = "http://facebooksdk.net/",
                picture = image_data.images.standard_resolution.url
            };

            try
            {
                dynamic fbPostTaskResult = await facebookClient.PostTaskAsync("/me/feed", postParams);
                var result = (IDictionary<string, object>)fbPostTaskResult;

                var successMessageDialog = new Windows.UI.Popups.MessageDialog("Posted Open Graph Action, id: " + (string)result["id"]);
                await successMessageDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                var exceptionMessageDialog = new Windows.UI.Popups.MessageDialog("Exception during post: " + ex.Message);
                exceptionMessageDialog.ShowAsync();
            }
        }

        public void ContinueWithWebAuthenticationBroker(WebAuthenticationBrokerContinuationEventArgs args)
        {
            UniversalAppUtil._fbm.ContinueAuthentication(args);
            //txtFbToken.Text = _fbm.AccessToken;

            PublishStory();
        }
    }
}
