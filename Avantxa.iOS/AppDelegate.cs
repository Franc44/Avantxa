using Foundation;
using UIKit;
using Syncfusion.SfCalendar.XForms.iOS;
using Firebase.CloudMessaging;
using UserNotifications;
using System;

namespace Avantxa.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public void DidRefreshRegistrationToken(Messaging messaging, string fcmToken)
        {
            System.Diagnostics.Debug.WriteLine($"FCM Token: {fcmToken}");
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzQyNTQ1QDMxMzgyZTMzMmUzMFFUU1V5WDNhOXJnNE1TaXFTN3FHUFpBampQQ1V3RHZrSU16dG9vbXc4MWs9;MzQyNTQ2QDMxMzgyZTMzMmUzMFlRWVBYNTJFSlozTDdEUG5JZldVRE55NXJzVG04ZFdqcGozWERNOURGaEE9;MzQyNTQ3QDMxMzgyZTMzMmUzMFpySmx1bmJPMHJDQ3pXYVZjWm5jVGwvRE0vZHQ2YVpteEMzK3FjcjhRdlU9;MzQyNTQ4QDMxMzgyZTMzMmUzMFQ3ZCtIZjZGTnU4Z3hHaXZpTlEvTEpJVEFFS1VOclNVY0UwcHM3MDVLN3c9;MzQyNTQ5QDMxMzgyZTMzMmUzMGNxYmw4WWgrdmhKV3Q2aVhTTHNBWUxvWkMvRkFaNDBjalZLdTI3UHg4ZGc9;MzQyNTUwQDMxMzgyZTMzMmUzMEtYeHczLzI2aW9sTS9hOWduY1BhejV3R295WGhvSjQ2OFFyRVFwbnhzTnM9;MzQyNTUxQDMxMzgyZTMzMmUzMFNKRjZYMUVFWm5vRUY2SXdMU2h1a0VJUkJZUEVaMU12WGJXNERvU0orTWM9;MzQyNTUyQDMxMzgyZTMzMmUzMFpQa3FUd29pWjUxc3l2U2ltb2FFb3QzR01XZXFZNGQ2dU0zbVZFRmdkc289;MzQyNTUzQDMxMzgyZTMzMmUzMGtzcEZSd1ArdG1BdGdJcnIrMURYZFRYM3F3REptNUl4L3lLMUlQOU5JbWM9;MzQyNTU0QDMxMzgyZTMzMmUzME91NXQwMVZkN3pwMWl4a0toRENuWlNwcndIS29OWVB1Zmd1VUZTM1Y1YW89");
            Rg.Plugins.Popup.Popup.Init();
            global::Xamarin.Forms.Forms.Init();
            Syncfusion.SfPdfViewer.XForms.iOS.SfPdfDocumentViewRenderer.Init();
            Syncfusion.SfRangeSlider.XForms.iOS.SfRangeSliderRenderer.Init();
            SfCalendarRenderer.Init();
            Firebase.Core.App.Configure();
            LoadApplication(new App());

            //NotificacioneS FCM
            // Register your app for remote notifications.
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // iOS 10 or later
                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) => {
                    Console.WriteLine(granted);
                });

                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();

                // For iOS 10 data message (sent via FCM)
                Messaging.SharedInstance.Delegate = this as IMessagingDelegate;
                app.RegisterForRemoteNotifications();
            }
            else
            {
                // iOS 9 or before
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }

            UIApplication.SharedApplication.RegisterForRemoteNotifications();

            return base.FinishedLaunching(app, options);
        }
    }
}
