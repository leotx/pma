using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using PMA.Helper;
using PMA.Model;
using PMA.Notification;
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(0, 0, 0, "Configurações");
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 0:
                    var loginActivity = new Intent(this, typeof(ConfigurationActivity));
                    StartActivity(loginActivity);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
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

        private static TimeSpan GetInterval(TimeSpan currentTimeSpan)
        {
            var intervalTime = TimeSpan.Parse(Preferences.Shared.GetString(Preferences.IntervalTime, null));
            return currentTimeSpan - intervalTime;
        }

        private static void SaveInterval(TimeSpan currentTimeSpan)
        {
            var prefEditor = Preferences.Shared.Edit();
            prefEditor.PutString(Preferences.IntervalTime, currentTimeSpan.ToString());
            prefEditor.Commit();
        }
    }
}