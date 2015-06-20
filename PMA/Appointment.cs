using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace PMA
{
    [Activity(Label = "Apontamento")]
    public class Appointment : Activity
    {
        private Button _appointmentButton;
        private TipoApontamento _typeOfAppointment;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Appointment);

            _appointmentButton = FindViewById<Button>(Resource.Id.btnAppointment);
            _appointmentButton.Click += AppointmentClick;
            ValidateTypeOfAppointment();

            var token = Intent.GetStringExtra("TOKEN");
        }

        private void ValidateTypeOfAppointment()
        {
            var prefs = Application.Context.GetSharedPreferences("PMA", FileCreationMode.Private);
            var dateOfAppointment = new DateTime(prefs.GetLong("DATE", 0));

            if (dateOfAppointment.Date == DateTime.Now.Date)
            {
                var typeOfAppointment = (TipoApontamento)prefs.GetInt("TYPE_APPOINTMENT", 0);
                _appointmentButton.Text = typeOfAppointment.ToString();
            }
            else
            {
                _typeOfAppointment = TipoApontamento.Cheguei;
                _appointmentButton.Text = _typeOfAppointment.ToString();
            }
        }

        void AppointmentClick(object sender, EventArgs e)
        {
            _typeOfAppointment = NextAppointment(_typeOfAppointment);
            _appointmentButton.Text = _typeOfAppointment.ToString();
        }

        private static TipoApontamento NextAppointment(TipoApontamento typeAppointment)
        {
            var arrayList = (TipoApontamento[])Enum.GetValues(typeof(TipoApontamento));
            var indexArray = Array.IndexOf(arrayList, typeAppointment) + 1;
            return (arrayList.Length == indexArray) ? arrayList[0] : arrayList[indexArray];
        }
    }
}