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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace DesafioThingPink
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImageDetail : Page
    {
        ImageData image_data;

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
            Image.Source = new BitmapImage(new Uri(image_data.images.standard_resolution.url, UriKind.Absolute));
            UsernameText.Text = "@" + image_data.user.username;
            LikesText.Text = image_data.likes.count + " Likes";

            ObservableCommentItems comment_collection = new ObservableCommentItems(image_data.comments.data);

            CommentList.ItemsSource = comment_collection;
            
        }

        private void BackTapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
