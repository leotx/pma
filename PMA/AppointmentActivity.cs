using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using PMA.Helper;
using PMA.Model;

namespace PMA
{
    [Activity(Label = "Apontamento", Icon = "@drawable/icon")]
    public class AppointmentActivity : Activity
    {
        private Button _appointmentButton;
        private TimePicker _timePicker;
        private TextView _endOfJourneyTextView;
        private Appointment _appointmentHelper;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Appointment);

            var token = Intent.GetStringExtra("TOKEN");

            _appointmentHelper = new Appointment(token);
            _appointmentButton = FindViewById<Button>(Resource.Id.btnAppointment);
            _timePicker = FindViewById<TimePicker>(Resource.Id.timeAppointment);
            _endOfJourneyTextView = FindViewById<TextView>(Resource.Id.txtEndOfJourney);
            _endOfJourneyTextView.Visibility = ViewStates.Invisible;
            _appointmentButton.Click += AppointmentClick;

            ValidateAppointment();
        }

        private void ValidateAppointment()
        {
            _appointmentHelper.ValidateAppointment();
            _appointmentButton.Text = _appointmentHelper.AppointmentType.ToString();
            ValidateEndOfJourney();
        }

        protected override void OnRestart()
        {
            base.OnRestart();

            var loginActivity = new Intent(this, typeof(LoginActivity));
            StartActivity(loginActivity);
        }

        private void ValidateEndOfJourney()
        {
            if (_appointmentHelper.AppointmentType != AppointmentType.Fim) return;

            _endOfJourneyTextView.Visibility = ViewStates.Visible;
            _appointmentButton.Visibility = ViewStates.Invisible;
            _timePicker.Visibility = ViewStates.Invisible;
        }

        void AppointmentClick(object sender, EventArgs e)
        {
            var currentTimeSpan = new TimeSpan(_timePicker.CurrentHour.IntValue(), _timePicker.CurrentMinute.IntValue(), 0);
            var responseResult = _appointmentHelper.PointTime(currentTimeSpan);
            var finalMesage = responseResult.IsError() ? responseResult.GetErrorMessage() : "Apontamento criado com sucesso!";
            RunOnUiThread(() => Toast.MakeText(this, finalMesage, ToastLength.Long).Show());

            _appointmentHelper.NextAppointment();
            _appointmentButton.Text = _appointmentHelper.AppointmentType.ToString();

            ValidateEndOfJourney();
        }
    }
}