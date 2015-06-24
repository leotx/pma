using System;
using Android.Content;

namespace PMA
{
    [BroadcastReceiver]
    public class BroadcastNetwork : BroadcastReceiver
    {
        public event EventHandler ConnectionStatusChanged;

        public override void OnReceive(Context context, Intent intent)
        {
            ConnectionStatusChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}