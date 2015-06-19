using Android.App;
using Android.OS;

namespace PMA
{
    [Activity(Label = "Apontamento")]	
    public class Appointment : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Appointment);
        }
    }
}