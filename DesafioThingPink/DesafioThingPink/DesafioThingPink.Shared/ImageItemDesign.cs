using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Xaml.Media.Imaging;

namespace DesafioThingPink
{
    class ImageItemDesign : INotifyPropertyChanged
    {

        private BitmapImage _image;
        private string _autor;

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



        public ImageItemDesign(Datum insta_image_item)
        {

            image = new BitmapImage(new Uri(insta_image_item.images.low_resolution.url, UriKind.Absolute));
            autor = insta_image_item.user.username;

            //http://stackoverflow.com/questions/249760/how-to-convert-unix-timestamp-to-datetime-and-vice-versa

        }



    }
}
