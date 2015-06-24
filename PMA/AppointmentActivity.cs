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
    [Activity(Label = "Apontamento")]
    public class AppointmentActivity : Activity
    {
        private Button _appointmentButton;
        private TimePicker _timePicker;
        private AppointmentType _typeOfAppointment;
        private TextView _endOfJourneyTextView;
        private ISharedPreferences _sharedPreferences;
        private Services.PmaService _pmaService;
        const string TypeOfAppointment = "TYPE_APPOINTMENT";
        const string DateOfAppointment = "DATE_APPOINTMENT";
        const string IntervalTime = "INTERVAL_TIME";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Appointment);

            var token = Intent.GetStringExtra("TOKEN");

            _pmaService = new Services.PmaService(token);
            _sharedPreferences = Application.Context.GetSharedPreferences("PMA", FileCreationMode.Private);
            _appointmentButton = FindViewById<Button>(Resource.Id.btnAppointment);
            _timePicker = FindViewById<TimePicker>(Resource.Id.timeAppointment);
            _endOfJourneyTextView = FindViewById<TextView>(Resource.Id.txtEndOfJourney);
            _endOfJourneyTextView.Visibility = ViewStates.Invisible;
            _appointmentButton.Click += AppointmentClick;
            ValidateTypeOfAppointment();
        }

        private void ValidateTypeOfAppointment()
        {
            var dateOfAppointment = new DateTime(_sharedPreferences.GetLong(DateOfAppointment, 0));

            if (dateOfAppointment.Date == DateTime.Now.Date)
            {
                _typeOfAppointment = (AppointmentType)_sharedPreferences.GetInt(TypeOfAppointment, 0);
                ValidateEndOfJourney();
            }
            else
            {
                _typeOfAppointment = AppointmentType.Cheguei;
                SavePreferences();
            }

            _appointmentButton.Text = _typeOfAppointment.ToString();
        }

        protected override void OnRestart()
        {
            base.OnRestart();

            var loginActivity = new Intent(this, typeof(LoginActivity));
            StartActivity(loginActivity);
        }

        private void ValidateEndOfJourney()
        {
            if (_typeOfAppointment != AppointmentType.Fim) return;

            _endOfJourneyTextView.Visibility = ViewStates.Visible;
            _appointmentButton.Visibility = ViewStates.Invisible;
            _timePicker.Visibility = ViewStates.Invisible;
        }

        private void SavePreferences()
        {
            var prefEditor = _sharedPreferences.Edit();
            prefEditor.PutInt(TypeOfAppointment, (int)_typeOfAppointment);
            prefEditor.PutLong(DateOfAppointment, DateTime.Now.Ticks);
            prefEditor.Commit();
        }

        void AppointmentClick(object sender, EventArgs e)
        {
            var currentTimeSpan = new TimeSpan(_timePicker.CurrentHour.IntValue(), _timePicker.CurrentMinute.IntValue(), 0);

            var responseResult = string.Empty;
            switch (_typeOfAppointment)
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

            var finalMesage = responseResult.IsError() ? responseResult.GetErrorMessage() : "Apontamento criado com sucesso!";
            RunOnUiThread(() => Toast.MakeText(this, finalMesage, ToastLength.Long).Show());

            _typeOfAppointment = NextAppointment(_typeOfAppointment);
            _appointmentButton.Text = _typeOfAppointment.ToString();
            SavePreferences();
            ValidateEndOfJourney();
        }

        private TimeSpan GetInterval(TimeSpan currentTimeSpan)
        {
            var intervalTime = TimeSpan.Parse(_sharedPreferences.GetString(IntervalTime, null));
            return currentTimeSpan - intervalTime;
        }

        private void SaveInterval(TimeSpan currentTimeSpan)
        {
            var prefEditor = _sharedPreferences.Edit();
            prefEditor.PutString(IntervalTime, currentTimeSpan.ToString());
            prefEditor.Commit();
        }

        private static AppointmentType NextAppointment(AppointmentType typeAppointment)
        {
            var arrayList = (AppointmentType[])Enum.GetValues(typeof(AppointmentType));
            var indexArray = Array.IndexOf(arrayList, typeAppointment) + 1;
            return (arrayList.Length == indexArray) ? arrayList[0] : arrayList[indexArray];
        }
    }
}