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

namespace DroidvigiaCompat
{
	[BroadcastReceiver]
	[IntentFilter(new string[]{"DroidVigia.DroidVigia.DroidVigia.SchudledCall"})]
	public class SchudledCallReciver : BroadcastReceiver
	{
		public override void OnReceive (Context context, Intent intent)
		{
			var prefs_model = Global.GetAppPreferences (context);
			if (prefs_model.Last_Call_State == Android.Telephony.CallState.Idle) {
				Intent callIntent = new Intent (Intent.ActionCall, Android.Net.Uri.Parse ("tel:"+prefs_model.Schudled_Call_Number));
				callIntent.AddFlags (ActivityFlags.NewTask);
				context.StartActivity (callIntent);
				prefs_model.Schudled_Call = false;
				prefs_model.Schudled_Call_Number = "";
			} else {
				SchudlerManager schudler = new SchudlerManager (context);
				schudler.RegisterSchudleCall ();
			}
		}
	}
}

