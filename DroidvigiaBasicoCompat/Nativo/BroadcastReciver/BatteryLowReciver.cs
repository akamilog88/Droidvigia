using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DroidvigiaBasicoCompat
{
	[BroadcastReceiver]
	[IntentFilter(new string[]{"android.intent.action.BATERY_LOW"})]
	public class BatteryLowReciver : BroadcastReceiver
	{
		public override void OnReceive (Context context, Intent intent)
		{
			PowerManager p_mngr = (PowerManager)context.GetSystemService (Context.PowerService);
			PowerManager.WakeLock w = p_mngr.NewWakeLock (WakeLockFlags.Full, "Call_lock");
			w.Acquire ();		
			var pref_model = Global.GetAppPreferences (context);
			pref_model.Call_Phone_Index = 0;
			Intent i = new Intent ("DroidVIGIA.DroidVIGIA.DroidVigiaBasic.SendAlarmBroadcastReciver");
			context.SendBroadcast (i);
			w.Release ();
		}
	}
}

