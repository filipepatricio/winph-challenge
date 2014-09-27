using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Xaml.Media.Imaging;
using DesafioThingPink.Models;

namespace DesafioThingPink.ViewModels
{
    class ImageItemDesign : INotifyPropertyChanged
    {

        private BitmapImage _image;
        private string _autor;
        public ImageData image_data { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public BitmapImage image { 
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                RaisePropertyChanged("image");
            }
        }
        public string autor
        {
            get
            {
                return _autor;
            }
            set
            {
                _autor = value;
                RaisePropertyChanged("autor");
            }
        }



        public ImageItemDesign(ImageData insta_image_item)
        {
            image_data = insta_image_item;
            image = new BitmapImage(new Uri(insta_image_item.images.low_resolution.url, UriKind.Absolute));
            autor = "@"+insta_image_item.user.username;

        }



    }
}
