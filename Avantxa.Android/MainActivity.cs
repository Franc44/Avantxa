using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android;
using AndroidX.Core.Content;
using AndroidX.Core.App;

namespace Avantxa.Droid
{
    [Activity(Label = "Avantxa", Icon = "@drawable/Logo", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private int PERMISSION_REQUEST_CODE = 1;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzQyNTQ1QDMxMzgyZTMzMmUzMFFUU1V5WDNhOXJnNE1TaXFTN3FHUFpBampQQ1V3RHZrSU16dG9vbXc4MWs9;MzQyNTQ2QDMxMzgyZTMzMmUzMFlRWVBYNTJFSlozTDdEUG5JZldVRE55NXJzVG04ZFdqcGozWERNOURGaEE9;MzQyNTQ3QDMxMzgyZTMzMmUzMFpySmx1bmJPMHJDQ3pXYVZjWm5jVGwvRE0vZHQ2YVpteEMzK3FjcjhRdlU9;MzQyNTQ4QDMxMzgyZTMzMmUzMFQ3ZCtIZjZGTnU4Z3hHaXZpTlEvTEpJVEFFS1VOclNVY0UwcHM3MDVLN3c9;MzQyNTQ5QDMxMzgyZTMzMmUzMGNxYmw4WWgrdmhKV3Q2aVhTTHNBWUxvWkMvRkFaNDBjalZLdTI3UHg4ZGc9;MzQyNTUwQDMxMzgyZTMzMmUzMEtYeHczLzI2aW9sTS9hOWduY1BhejV3R295WGhvSjQ2OFFyRVFwbnhzTnM9;MzQyNTUxQDMxMzgyZTMzMmUzMFNKRjZYMUVFWm5vRUY2SXdMU2h1a0VJUkJZUEVaMU12WGJXNERvU0orTWM9;MzQyNTUyQDMxMzgyZTMzMmUzMFpQa3FUd29pWjUxc3l2U2ltb2FFb3QzR01XZXFZNGQ2dU0zbVZFRmdkc289;MzQyNTUzQDMxMzgyZTMzMmUzMGtzcEZSd1ArdG1BdGdJcnIrMURYZFRYM3F3REptNUl4L3lLMUlQOU5JbWM9;MzQyNTU0QDMxMzgyZTMzMmUzME91NXQwMVZkN3pwMWl4a0toRENuWlNwcndIS29OWVB1Zmd1VUZTM1Y1YW89");
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                if (!PermissionGrantedForExternalStorage)
                {
                    // Code for above or equal 23 API Oriented Device 
                    // Your Permission is not granted already, so request the permission to access external storage to save the files
                    RequestPermission();
                }
            }
        }
        /// <summary>
        /// Check whether this application has permission to access the external storage
        /// </summary>
        private bool PermissionGrantedForExternalStorage
        {
            get
            {
                Permission permissionResult = ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage);
                if (permissionResult == Permission.Granted)
                {
                    // if permission is already granted return true otherwise return false
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// Request to enable permission to write the files on external storage of android device
        /// </summary>
        private void RequestPermission()
        {
            if (!ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.WriteExternalStorage))
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.WriteExternalStorage }, PERMISSION_REQUEST_CODE);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }
    }
}