using ClassLibrary;
using DesafioThingPink.Models;
using DesafioThingPink.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;

namespace DesafioThingPink
{
    partial class MainPage
    {
        ApiRequests apiRequests = new ApiRequests();
        ObservableImageItems insta_image_collection;
        List<SearchItem> roaming_search_list;

        ResourceLoader resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

        private const string taskName = "BackgroundTask";
        private const string taskEntryPoint = "BackgroundTasks.BackgroundTask";

        private async void RegisterBackgroundTask()
        {
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == taskName)
                    {
                        task.Value.Unregister(true);
                    }
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.TaskEntryPoint = taskEntryPoint;
                taskBuilder.SetTrigger(new TimeTrigger(15, false));
                var registration = taskBuilder.Register();
            }
        }

        private void RefreshMap(List<SearchItem> roaming_search_list)
        {
            MyMap.MapServiceToken = (string)App.Current.Resources["MapServiceToken"];
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
    }
}
