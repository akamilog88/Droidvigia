
using System;
using System.Threading;
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
using Android.Support.V4.Content;
using Android.Util;


namespace DroidvigiaCompat
{
	[BroadcastReceiver]
	[IntentFilter(new string[]{"DroidVigia.DroidVigia.DroidVigia.ToneDetected"})]
	public class ToneDetectedReciver : BroadcastReceiver
	{
		PreferencesModel prefmodel;
		public const int ALARM_NOTIFICATION_ID=222;
		public override void OnReceive (Context context, Intent intent)
		{
			//Toast.MakeText (context, "Received intent!", ToastLength.Short).Show();
			lock (Global.Get_Tone_Reciver_Lock()) {

				ISharedPreferences pref = context.GetSharedPreferences ("DroidvigiaCompat.DroidvigiaCompat.DroidvigiaCompat", FileCreationMode.Private);
				prefmodel = new PreferencesModel (pref);

				String nums = prefmodel.numbers;
				bool DoCall = prefmodel.DoCall;
				bool DoMessage = prefmodel.DoMessage;
				bool DoMMS = prefmodel.DoMMS;
				bool DoSilence = prefmodel.DoSilence;
				string message = intent.GetStringExtra ("message");

			//if (LicenseUtility.IsValidSerial (prefmodel.Serial, prefmodel.GetInfoForRequest (context))) {
				if (!DoSilence) {
					NotificationManager n_mngr = (NotificationManager)context.GetSystemService (Service.NotificationService);
					Notification n = new Notification (Resource.Drawable.Icon, message, DateTime.UtcNow.Millisecond);
					Intent notificationIntent = new Intent (context, typeof(MainActivity));
					n_mngr.Cancel (ALARM_NOTIFICATION_ID);
					//PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, 0);
					n.Defaults |= NotificationDefaults.All; 
					n.Flags |= NotificationFlags.Insistent;
					n.SetLatestEventInfo (context, "Alarma!", message, null);
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
					bool call_done = false;
					//Message Portion				
					if (DoMessage && message != null) {
						SmsManager sms_mngr = SmsManager.Default;
						int i = l_nums.Count;
						for (int j=l_nums.Count-1; j>=0; j--) {
							if (j == 0 && DoCall) {
								PendingIntent sentIntent = PendingIntent.GetActivity (context, 0, callIntent, PendingIntentFlags.OneShot);
								sms_mngr.SendTextMessage (l_nums[j], null, message, sentIntent, null);
								call_done = true;								
							} else {
								sms_mngr.SendTextMessage (l_nums[j], null, message, null, null);								
							}											
						}						
					}	
					if (DoCall && prefmodel.Last_Call_State == CallState.Idle && !call_done)
						context.StartActivity (callIntent);					
				}
				//Intent i2 = new Intent (context, typeof(DetectorService));
				//context.StopService (i2);					
				//context.StartService(i2);
			//}/**/
		}
	}
}
}

