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
using Windows.ApplicationModel.Activation;

namespace DesafioThingPink
{
    partial class ImageDetail
    {

        ImageData image_data;
        BitmapImage bitmap_image = new BitmapImage();

        private async Task SaveImageOnLibrary()
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
                UniversalAppUtil.ShowMessage("Imagem guardada com sucesso");
            }
            catch
            {
                UniversalAppUtil.ShowMessage("Ocorreu um erro ao guardar a imagem");
            }
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
