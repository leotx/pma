using System;
using Android.App;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using PMA.Helper.Broadcast;

namespace PMA.Helper
{
    public class NetworkVerify
    {
        private BroadcastNetwork _broadcastReceiver;
        private static AutoAppointment _autoAppointment;
        private const string DefaultSsid = "DXT-MOBILE";

        public void Start()
        {
            if (_broadcastReceiver != null) return;

            _broadcastReceiver = new BroadcastNetwork();
            _broadcastReceiver.ConnectionStatusChanged += OnNetworkStatusChanged;

            _autoAppointment = new AutoAppointment();

            Application.Context.RegisterReceiver(_broadcastReceiver,
                new IntentFilter(ConnectivityManager.ConnectivityAction));
        }

        static void OnNetworkStatusChanged(object sender, EventArgs e)
        {
            var wifiManager = (WifiManager)Application.Context.GetSystemService(Context.WifiService);
            var wifiSsid = wifiManager.ConnectionInfo.SSID;

            var isValidSsid = Preferences.Shared.GetBoolean(Preferences.ValidSsid, false);

            if (isValidSsid)
            {
                if (wifiSsid.Contains(DefaultSsid)) return;
                
                Notification.Notification.Notify("Você saiu da Dextra!");
                _autoAppointment.SaveLastDate();
                isValidSsid = false;
            }
            else if (wifiSsid.Contains(DefaultSsid))
            {
                Notification.Notification.Notify("Você está na Dextra!");
                _autoAppointment.VerifyAppointment();
                isValidSsid = true;
            }

            var editPreferences = Preferences.Shared.Edit();
            editPreferences.PutBoolean(Preferences.ValidSsid, isValidSsid);
            editPreferences.Commit();
        }
    }
}