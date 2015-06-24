using System;
using Android.App;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;

namespace PMA
{
    public class NetworkVerify
    {
        private BroadcastNetwork _broadcastReceiver;
        private const string DefaultSsid = "DXT-MOBILE";
        private static bool _isValidSsid;

        public void Start()
        {
            if (_broadcastReceiver != null) return;

            _broadcastReceiver = new BroadcastNetwork();
            _broadcastReceiver.ConnectionStatusChanged += OnNetworkStatusChanged;

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
                
                Notification.Notify("Voc� saiu da Dextra!");
                _isValidSsid = false;
            }
            else if (wifiSsid.Contains(DefaultSsid))
            {
                Notification.Notify("Voc� est� na Dextra!");
                _isValidSsid = true;
            }
        }
    }
}