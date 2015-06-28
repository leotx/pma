using System;
using System.Threading.Tasks;
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
        private static AutoAppointment _autoPoint;
        private const string DefaultSsid = "4P705";
        private static bool _isValidSsid;

        public void Start()
        {
            if (_broadcastReceiver != null) return;

            _broadcastReceiver = new BroadcastNetwork();
            _broadcastReceiver.ConnectionStatusChanged += OnNetworkStatusChanged;

            _autoPoint = new AutoAppointment();

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
                _autoPoint.SaveLastNotification();
                _isValidSsid = false;
            }
            else if (wifiSsid.Contains(DefaultSsid))
            {
                Notification.Notify("Você está na Dextra!");
                _autoPoint.VerifyAppointment();
                _isValidSsid = true;
            }
        }
    }
}