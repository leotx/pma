using System;
using Android.App;
using Android.OS;
using Android.Widget;
using PMA.Helper;

namespace PMA
{
    [Activity(Label = "Configurações", Icon = "@drawable/icon")]
    public class ConfigurationActivity : Activity
    {
        private CheckBox _autoAppointmentCheckBox;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Configuration);

            _autoAppointmentCheckBox = FindViewById<CheckBox>(Resource.Id.chkAutoAppointment);
            _autoAppointmentCheckBox.Click += AutoAppointmentCheckBoxOnClick;
            _autoAppointmentCheckBox.Checked = Preferences.Shared.GetBoolean(Preferences.AutoPointActivated, false);
        }

        private void AutoAppointmentCheckBoxOnClick(object sender, EventArgs eventArgs)
        {
            var editPreferences = Preferences.Shared.Edit();
            editPreferences.PutBoolean(Preferences.AutoPointActivated, _autoAppointmentCheckBox.Checked);
            editPreferences.Commit();
        }
    }
}