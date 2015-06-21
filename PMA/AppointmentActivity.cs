using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace PMA
{
    [Activity(Label = "Apontamento")]
    public class AppointmentActivity : Activity
    {
        private Button _appointmentButton;
        private TimePicker _timePicker;
        private TipoApontamento _typeOfAppointment;
        private ISharedPreferences _sharedPreferences;
        private Services _pmaService;
        const string TypeOfAppointment = "TYPE_APPOINTMENT";
        const string DateOfAppointment = "DATE_APPOINTMENT";
        const string IntervalTime = "INTERVAL_TIME";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Appointment);

            var token = Intent.GetStringExtra("TOKEN");

            _pmaService = new Services(token);
            _sharedPreferences = Application.Context.GetSharedPreferences("PMA", FileCreationMode.Private);
            _appointmentButton = FindViewById<Button>(Resource.Id.btnAppointment);
            _timePicker = FindViewById<TimePicker>(Resource.Id.timeAppointment);
            _appointmentButton.Click += AppointmentClick;
            ValidateTypeOfAppointment();
        }

        private void ValidateTypeOfAppointment()
        {
            var dateOfAppointment = new DateTime(_sharedPreferences.GetLong(DateOfAppointment, 0));

            if (dateOfAppointment.Date == DateTime.Now.Date)
            {
                var typeOfAppointment = (TipoApontamento)_sharedPreferences.GetInt(TypeOfAppointment, 0);
                _appointmentButton.Text = typeOfAppointment.ToString();
            }
            else
            {
                _typeOfAppointment = TipoApontamento.Cheguei;
                _appointmentButton.Text = _typeOfAppointment.ToString();
                SavePreferences();
            }
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
            _typeOfAppointment = NextAppointment(_typeOfAppointment);
            _appointmentButton.Text = _typeOfAppointment.ToString();
            var dailyAppointment = _pmaService.FindDailyAppointment();
            var currentTimeSpan = new TimeSpan(_timePicker.CurrentHour.IntValue(), _timePicker.CurrentMinute.IntValue(), 0);

            switch (_typeOfAppointment)
            {
                case TipoApontamento.Cheguei:
                    _pmaService.CreateDailyAppointment(currentTimeSpan);
                    break;
                case TipoApontamento.Intervalo:
                    SaveInterval(currentTimeSpan);
                    break;
                case TipoApontamento.Voltei:
                    var intervalTime = GetInterval(currentTimeSpan);
                    _pmaService.CreateDailyAppointment(dailyAppointment.StartHour, intervalTime);
                    break;
                case TipoApontamento.Fui:
                    _pmaService.CreateDailyAppointment(dailyAppointment.StartHour, dailyAppointment.RestHour, currentTimeSpan);
                    break;
            }

            SavePreferences();
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

        private static TipoApontamento NextAppointment(TipoApontamento typeAppointment)
        {
            var arrayList = (TipoApontamento[])Enum.GetValues(typeof(TipoApontamento));
            var indexArray = Array.IndexOf(arrayList, typeAppointment) + 1;
            return (arrayList.Length == indexArray) ? arrayList[0] : arrayList[indexArray];
        }
    }
}