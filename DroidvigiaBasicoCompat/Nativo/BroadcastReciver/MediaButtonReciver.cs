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
using Android.Telephony;

namespace DroidvigiaBasicoCompat
{
	[BroadcastReceiver]
	public class MediaButtonReciver : BroadcastReceiver
	{	
		public override void OnReceive (Context context, Intent intent)
		{
			PowerManager p_mngr = (PowerManager)context.GetSystemService (Context.PowerService);
			PowerManager.WakeLock w = p_mngr.NewWakeLock (WakeLockFlags.Full, "Call_lock");
			w.Acquire ();
			Toast.MakeText (context, "Received intent!", ToastLength.Short).Show ();
			var pref_model = Global.GetAppPreferences (context);
			pref_model.Notificando = true;
			w.Release ();
		}
	}
}


