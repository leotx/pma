using System.Linq;
using System.Xml.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace PMA
{
    [Activity(Label = "PMA", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            var userName = FindViewById<EditText>(Resource.Id.etUserName);
            var password = FindViewById<EditText>(Resource.Id.etPass);
            var loginButton = FindViewById<Button>(Resource.Id.btnSingIn);
            var progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            progressBar.Visibility = ViewStates.Invisible;

            if (loginButton != null)
            {
                loginButton.Click += async (sender, e) =>
                {
                    progressBar.Visibility = ViewStates.Visible;
                    var servicePma = new Services();
                    var response = servicePma.Login(userName.Text, password.Text);
                    GetToken(response);
                    progressBar.Enabled = false;
                    StartActivity(typeof(Appointment));
                    progressBar.Visibility = ViewStates.Invisible;
                };
            }
        }

        private static void GetToken(string response)
        {
            var token = XDocument.Parse(response).Descendants("token").FirstOrDefault();
        }
    }
}