using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using PMA.Helper;
using PMA.Notification;

namespace PMA
{
    [Activity(Label = "PMA", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Holo.NoActionBar.TranslucentDecor")]
    public class LoginActivity : Activity
    {
        private EditText _passwordText;
        private EditText _usernameText;
        private Button _loginButton;
        private LinearLayout _linearLayout;
        private const string Username = "USERNAME";
        private const string Password = "PASSWORD";
        private ISharedPreferences _sharedPreferences;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Login);

            StartService(new Intent(this, typeof(NotificationService)));

            _usernameText = FindViewById<EditText>(Resource.Id.etUserName);
            _passwordText = FindViewById<EditText>(Resource.Id.etPass);
            _loginButton = FindViewById<Button>(Resource.Id.btnLogin);
            _linearLayout = FindViewById<LinearLayout>(Resource.Id.linearLayout);
            _linearLayout.Visibility = ViewStates.Invisible;
            _sharedPreferences = Application.Context.GetSharedPreferences(Assembly.GetExecutingAssembly().GetName().Name, FileCreationMode.Private);

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
            _linearLayout.Visibility = ViewStates.Visible;
            var task = new Task(Login);
            task.Start();
            task.ContinueWith(x => { _linearLayout.Visibility = ViewStates.Gone; });
        }

        private void Login()
        {
            var servicePma = new Services.PmaService();
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
            var prefEditor = _sharedPreferences.Edit();
            prefEditor.PutString(Username, _usernameText.Text);
            prefEditor.PutString(Password, _passwordText.Text);
            prefEditor.Commit();
        }

        private void LoadPreferences()
        {
            var username = _sharedPreferences.GetString(Username, null);
            var password = _sharedPreferences.GetString(Password, null);

            if (username == null || password == null) return;
            
            _usernameText.Text = username;
            _passwordText.Text = password;
            StartTask();
        }
    }
}