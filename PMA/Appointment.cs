using Android.App;
using Android.OS;
using Android.Widget;

namespace PMA
{
    [Activity(Label = "Apontamento")]	
    public class Appointment : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Appointment);

            var token = Intent.GetStringExtra("TOKEN");
            RunOnUiThread(() => Toast.MakeText(this, token, ToastLength.Long).Show());
        }
    }
}