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

            switch (_typeOfAppointment)
            {
                case TipoApontamento.Cheguei:
                    _pmaService.CreateDailyAppointment(new TimeSpan(_timePicker.CurrentHour.IntValue(), _timePicker.CurrentMinute.IntValue(), 0));
                    break;
                case TipoApontamento.Intervalo:
                    break;
                case TipoApontamento.Voltei:
                    break;
                case TipoApontamento.Fui:
                    break;
            }

            SavePreferences();
        }

        private static TipoApontamento NextAppointment(TipoApontamento typeAppointment)
        {
            var arrayList = (TipoApontamento[])Enum.GetValues(typeof(TipoApontamento));
            var indexArray = Array.IndexOf(arrayList, typeAppointment) + 1;
            return (arrayList.Length == indexArray) ? arrayList[0] : arrayList[indexArray];
        }
    }
}