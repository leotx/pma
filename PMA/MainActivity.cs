using System;
using System.Linq;
using System.Xml.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace PMA
{
    [Activity(Label = "PMA", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private EditText _passwordText;
        private EditText _usernameText;
        private Button _loginButton;
        private ProgressBar _progressBar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            _usernameText = FindViewById<EditText>(Resource.Id.etUserName);
            _passwordText = FindViewById<EditText>(Resource.Id.etPass);
            _loginButton = FindViewById<Button>(Resource.Id.btnSingIn);
            _progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            _progressBar.Visibility = ViewStates.Invisible;
            
            _loginButton.Click += LoginClick;
            LoadPreferences();
        }

        void LoginClick(object sender, EventArgs e)
        {
            Login();
        }

        private void Login()
        {
            _progressBar.Visibility = ViewStates.Visible;
            var servicePma = new Services();
            var response = servicePma.Login(_usernameText.Text, _passwordText.Text);
            GetToken(response);
            _progressBar.Enabled = false;
            StartActivity(typeof(Appointment));
            _progressBar.Visibility = ViewStates.Invisible;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            var prefs = Application.Context.GetSharedPreferences("PMA", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutString("USERNAME", _usernameText.Text);
            prefEditor.PutString("PASSWORD", _passwordText.Text);
            prefEditor.Commit();
        }

        private static void GetToken(string response)
        {
            var token = XDocument.Parse(response).Descendants("token").FirstOrDefault();
        }

        protected void LoadPreferences()
        {
            var prefs = Application.Context.GetSharedPreferences("PMA", FileCreationMode.Private);
            var username = prefs.GetString("USERNAME", null);
            var password = prefs.GetString("PASSWORD", null);

            if (username == null || password == null) return;
            
            _usernameText.Text = username;
            _passwordText.Text = password;
            Login();
        }
    }
}