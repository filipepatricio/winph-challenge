﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using DesafioThingPink.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DesafioThingPink.Models;
using Windows.UI.Xaml.Media.Imaging;
using DesafioThingPink.ViewModels;
using DesafioThingPink.Manager;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Storage;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace DesafioThingPink
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class ImageDetail : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();


        ImageData image_data;
        BitmapImage bitmap_image = new BitmapImage();
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

        public ImageDetail()
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
            object navigationParameter;
            if (e.PageState != null && e.PageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = e.PageState["SelectedItem"];
            }

            // TODO: Assign a bindable group to this.DefaultViewModel["Group"]
            // TODO: Assign a collection of bindable items to this.DefaultViewModel["Items"]
            // TODO: Assign the selected item to this.flipView.SelectedItem
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);

            image_data = (e.Parameter as ImageData);
            bitmap_image = new BitmapImage(new Uri(image_data.images.standard_resolution.url, UriKind.Absolute));
            ImageBox.Source = bitmap_image;
            UsernameText.Text = "@" + image_data.user.username;
            LikesText.Text = image_data.likes.count + " Likes";

            ObservableCommentItems comment_collection = new ObservableCommentItems(image_data.comments.data);

            CommentList.ItemsSource = comment_collection;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
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

        private async void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            if (UniversalAppUtil._fbm == null)
            {
                UniversalAppUtil._fbm = new FacebookManager();
                await UniversalAppUtil._fbm.LoginAsync();
            }
            else if (UniversalAppUtil._fbm._fb.AccessToken == null)
                await UniversalAppUtil._fbm.LoginAsync();
            
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
    }
}
