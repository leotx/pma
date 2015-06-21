﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace PMA
{
    [Activity(Label = "PMA", MainLauncher = true, Icon = "@drawable/icon")]
    public class LoginActivity : Activity
    {
        private EditText _passwordText;
        private EditText _usernameText;
        private Button _loginButton;
        private ProgressBar _progressBar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Login);

            _usernameText = FindViewById<EditText>(Resource.Id.etUserName);
            _passwordText = FindViewById<EditText>(Resource.Id.etPass);
            _loginButton = FindViewById<Button>(Resource.Id.btnLogin);
            _progressBar = FindViewById<ProgressBar>(Resource.Id.pbLogin);
            _progressBar.Visibility = ViewStates.Invisible;
            
            _loginButton.Click += LoginClick;
            LoadPreferences();
        }

        void LoginClick(object sender, EventArgs e)
        {
            var inputManager = (InputMethodManager)GetSystemService(InputMethodService);
            inputManager.HideSoftInputFromWindow(CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);

            StartTask();
        }

        private void StartTask()
        {
            _progressBar.Visibility = ViewStates.Visible;
            var task = new Task(Login);
            task.Start();
            task.ContinueWith(x => { _progressBar.Visibility = ViewStates.Gone; });
        }

        private void Login()
        {
            var servicePma = new Services.PmaService();
            var response = servicePma.Login(_usernameText.Text.Trim(), _passwordText.Text.Trim());

            var tokenData = GetToken(response);
            if (tokenData != null)
            {
                SavePreferences();
                var appointmentActivity = new Intent(this, typeof (AppointmentActivity));
                appointmentActivity.PutExtra("TOKEN", tokenData);
                StartActivity(appointmentActivity);
            }
            else
            {
                RunOnUiThread(() => Toast.MakeText(this, "Erro ao efetuar login.", ToastLength.Long).Show());
            }
        }

        private void SavePreferences()
        {
            var prefs = Application.Context.GetSharedPreferences("PMA", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutString("USERNAME", _usernameText.Text);
            prefEditor.PutString("PASSWORD", _passwordText.Text);
            prefEditor.Commit();
        }

        private static string GetToken(string response)
        {
            var token = XDocument.Parse(response).Descendants("token").FirstOrDefault();
            return token != null ? token.Value : null;
        }

        private void LoadPreferences()
        {
            var prefs = Application.Context.GetSharedPreferences("PMA", FileCreationMode.Private);
            var username = prefs.GetString("USERNAME", null);
            var password = prefs.GetString("PASSWORD", null);

            if (username == null || password == null) return;
            
            _usernameText.Text = username;
            _passwordText.Text = password;
            StartTask();
        }
    }
}