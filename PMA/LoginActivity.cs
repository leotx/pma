using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using PMA.Helper;
using PMA.Notification;
using PMA.Services;

namespace PMA
{
    [Activity(Label = "PMA", MainLauncher = true, Icon = "@drawable/icon",
        Theme = "@android:style/Theme.Holo.NoActionBar.TranslucentDecor")]
    public class LoginActivity : Activity
    {
        private LinearLayout _linearLayout;
        private Button _loginButton;
        private EditText _passwordText;
        private EditText _usernameText;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Login);

            StartService(new Intent(this, typeof (NotificationService)));

            _usernameText = FindViewById<EditText>(Resource.Id.etUserName);
            _passwordText = FindViewById<EditText>(Resource.Id.etPass);
            _loginButton = FindViewById<Button>(Resource.Id.btnLogin);
            _linearLayout = FindViewById<LinearLayout>(Resource.Id.linearLayout);
            _linearLayout.Visibility = ViewStates.Invisible;

            _loginButton.Click += LoginClick;
            LoadPreferences();
        }

        private void LoginClick(object sender, EventArgs e)
        {
            var inputManager = (InputMethodManager) GetSystemService(InputMethodService);
            inputManager.HideSoftInputFromWindow(CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);

            StartTask();
        }

        private void StartTask()
        {
            _linearLayout.Visibility = ViewStates.Visible;
            var task = new Task(Login);
            task.Start();
            task.ContinueWith(x => { _linearLayout.Visibility = ViewStates.Gone; });
        }

        private void Login()
        {
            var servicePma = new PmaService();
            var response = servicePma.Login(_usernameText.Text.Trim(), _passwordText.Text.Trim());

            var tokenData = response.GetToken();
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
            var prefEditor = Preferences.Shared.Edit();
            prefEditor.PutString(Preferences.Username, _usernameText.Text);
            prefEditor.PutString(Preferences.Password, _passwordText.Text);
            prefEditor.Commit();
        }

        private void LoadPreferences()
        {
            var username = Preferences.Shared.GetString(Preferences.Username, null);
            var password = Preferences.Shared.GetString(Preferences.Password, null);

            if (username == null || password == null) return;

            _usernameText.Text = username;
            _passwordText.Text = password;
            StartTask();
        }
    }
}