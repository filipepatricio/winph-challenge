using ClassLibrary;
using DesafioThingPink.Models;
using DesafioThingPink.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Background;

namespace DesafioThingPink
{
    partial class MainPage
    {
        ApiRequests apiRequests = new ApiRequests();
        ObservableImageItems insta_image_collection;
        List<SearchItem> roaming_search_list;

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
    }
}
