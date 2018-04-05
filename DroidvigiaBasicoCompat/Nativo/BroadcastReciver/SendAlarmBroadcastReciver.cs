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
	[IntentFilter(new string[]{"DroidVIGIA.DroidVIGIA.DroidVigiaBasic.SendAlarmBroadcastReciver"})]
	public class SendAlarmBroadcastReciver : BroadcastReceiver
	{
		public override void OnReceive (Context context, Intent intent)
		{
			PreferencesModel prefmodel;
			const int ALARM_NOTIFICATION_ID = 222;		
			prefmodel = Global.GetAppPreferences (context);

			String nums = prefmodel.numbers;
			bool DoCall = prefmodel.DoCall;
			bool DoMessage = prefmodel.DoMessage;
			bool DoMMS = prefmodel.DoMMS;
			bool DoSilence = prefmodel.DoSilence;

			if (LicenseUtility.IsValidSerial (prefmodel.Serial, prefmodel.GetInfoForRequest (context))) {
				if (!DoSilence) {
					NotificationManager n_mngr = (NotificationManager)context.GetSystemService (Service.NotificationService);
					Notification n = new Notification (Resource.Drawable.Icon, "Alarma!.Hora de Envio:" + DateTime.Now.ToLongDateString (), DateTime.UtcNow.Millisecond);
					Intent notificationIntent = new Intent (context, typeof(MainActivity));
					//PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, 0);
					n.Defaults |= NotificationDefaults.All; 
					n.Flags |= NotificationFlags.Insistent;
					n.SetLatestEventInfo (context, "Alarma!", "Alarma!.Hora de Envio:" + DateTime.Now.ToLongDateString (), null);
					n_mngr.Notify (ALARM_NOTIFICATION_ID, n);
				}
				List<String> l_nums = prefmodel.notificationNumbers;
				if (l_nums.Count > 0) {	

					string current_number = l_nums [prefmodel.Call_Phone_Index];
					if (prefmodel.Call_Phone_Index == l_nums.Count - 1) 
						prefmodel.Call_Phone_Index = 0;
					else 
						prefmodel.Call_Phone_Index++;

					Intent callIntent = new Intent (Intent.ActionCall, Android.Net.Uri.Parse ("tel:"+current_number));
					//callIntent.AddFlags (ActivityFlags.FromBackground);
					callIntent.AddFlags (ActivityFlags.NewTask);				
					PowerManager pwr_mngr = (PowerManager)context.GetSystemService (Context.PowerService);
					PowerManager.WakeLock w = pwr_mngr.NewWakeLock (WakeLockFlags.Full, "Schudle");
					w.Acquire (10000);
					if (DoCall)
						context.StartActivity (callIntent);		
				}	
			}/**/
		}
	}
}

