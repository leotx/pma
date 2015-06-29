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
        private BroadcastNetwork BroadcastReceiver { get; set; }
        private static AutoAppointment AutoAppointment { get; set; }
        private const string DefaultSsid = "DXT-MOBILE";

        public void Start()
        {
            if (BroadcastReceiver != null) return;

            BroadcastReceiver = new BroadcastNetwork();
            BroadcastReceiver.ConnectionStatusChanged += OnNetworkStatusChanged;

            AutoAppointment = new AutoAppointment();

            Application.Context.RegisterReceiver(BroadcastReceiver,
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
                AutoAppointment.SaveLastDate();
                isValidSsid = false;
            }
            else if (wifiSsid.Contains(DefaultSsid))
            {
                Notification.Notification.Notify("Você está na Dextra!");
                AutoAppointment.VerifyAppointment();
                isValidSsid = true;
            }

            var editPreferences = Preferences.Shared.Edit();
            editPreferences.PutBoolean(Preferences.ValidSsid, isValidSsid);
            editPreferences.Commit();
        }
    }
}