using System;
using System.Reflection;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using PMA.Helper;
using PMA.Model;
using PMA.Services;

namespace PMA
{
    [Activity(Label = "Apontamento", Icon = "@drawable/icon")]
    public class AppointmentActivity : Activity
    {
        private Button _appointmentButton;
        private TimePicker _timePicker;
        private TextView _endOfJourneyTextView;
        private Appointment _appointmentHelper;
        private PmaService _pmaService;
        const string IntervalTime = "INTERVAL_TIME";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Appointment);

            var token = Intent.GetStringExtra("TOKEN");

            _appointmentHelper = new Appointment();
            _pmaService = new PmaService(token);
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
            var responseResult = PointTime(currentTimeSpan);
            var finalMesage = responseResult.IsError() ? responseResult.GetErrorMessage() : "Apontamento criado com sucesso!";
            RunOnUiThread(() => Toast.MakeText(this, finalMesage, ToastLength.Long).Show());

            _appointmentHelper.NextAppointment();
            _appointmentButton.Text = _appointmentHelper.AppointmentType.ToString();

            ValidateEndOfJourney();
        }

        private string PointTime(TimeSpan currentTimeSpan)
        {
            var responseResult = string.Empty;

            switch (_appointmentHelper.AppointmentType)
            {
                case AppointmentType.Cheguei:
                    responseResult = _pmaService.StartAppointment(currentTimeSpan);
                    break;
                case AppointmentType.Intervalo:
                    SaveInterval(currentTimeSpan);
                    break;
                case AppointmentType.Voltei:
                    var intervalTime = GetInterval(currentTimeSpan);
                    responseResult = _pmaService.IntervalAppointment(intervalTime);
                    break;
                case AppointmentType.Fui:
                    responseResult = _pmaService.EndAppointment(currentTimeSpan);
                    break;
            }

            return responseResult;
        }

        private TimeSpan GetInterval(TimeSpan currentTimeSpan)
        {
            var intervalTime = TimeSpan.Parse(_appointmentHelper.SharedPreferences.GetString(IntervalTime, null));
            return currentTimeSpan - intervalTime;
        }

        private void SaveInterval(TimeSpan currentTimeSpan)
        {
            var prefEditor = _appointmentHelper.SharedPreferences.Edit();
            prefEditor.PutString(IntervalTime, currentTimeSpan.ToString());
            prefEditor.Commit();
        }
    }
}