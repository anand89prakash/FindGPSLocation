using System;

using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace FindGeoLocation
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public TextView location;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);
            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            location = FindViewById<TextView>(Resource.Id.location);
            fab.Click += FabOnClick;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public async void FabOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var hasPermission = await CheckPermissions(Permission.Location);
                if (!hasPermission)
                    return;

                var locator = CrossGeolocator.Current;
                location.Text = "Getting location...";

                var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(30), null, false);

                if (position == null)
                {
                    location.Text = "Not found gps :(";
                    return;
                }
                location.Text = string.Format("Time: {0} \nLat: {1} \nLong: {2}",
                    position.Timestamp, position.Latitude, position.Longitude);

            }
            catch (Exception ex)
            {
                location.Text = "Exception";
            }

        }

        public async Task<bool> CheckPermissions(Permission permission)
        {
            var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
            bool request = false;
            if (permissionStatus == PermissionStatus.Denied)
            {
                request = true;
            }
            if (request || permissionStatus != PermissionStatus.Granted)
            {
                await CrossPermissions.Current.RequestPermissionsAsync(permission);
            }
            return true;
        }
       
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }

   
}

