﻿using System.Linq;
using System.Xml.Linq;
using Android.App;
using Android.OS;
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

            if (loginButton != null)
            {
                loginButton.Click += async (sender, e) =>
                {
                    //StartActivity(typeof(Appointment));
                    var servicePma = new Services();
                    var response = servicePma.Login(userName.Text, password.Text);
                    GetToken(response);
                };
            }
        }

        private static void GetToken(string response)
        {
            var token = XDocument.Parse(response).Descendants("token").FirstOrDefault();
        }
    }
}