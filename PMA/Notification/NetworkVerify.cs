using System;
using Android.App;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using PMA.Helper;

namespace PMA.Notification
{
    public class NetworkVerify
    {
        private BroadcastNetwork _broadcastReceiver;
        private static AutoAppointment _autoAppointment;
        private const string DefaultSsid = "DXT-MOBILE";
        private static bool _isValidSsid;

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

            if (_isValidSsid)
            {
                if (wifiSsid.Contains(DefaultSsid)) return;
                
                Notification.Notify("Você saiu da Dextra!");
                AutoAppointment.SaveLastDate();
                _isValidSsid = false;
            }
            else if (wifiSsid.Contains(DefaultSsid))
            {
                Notification.Notify("Você está na Dextra!");
                _autoAppointment.VerifyAppointment();
                _isValidSsid = true;
            }
        }
    }
}