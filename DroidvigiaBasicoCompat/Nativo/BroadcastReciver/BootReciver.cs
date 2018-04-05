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
	[IntentFilter(new string[]{"android.intent.action.BOOT_COMPLETED"})]
	public class BootReciver : BroadcastReceiver
	{
		public override void OnReceive (Context context, Intent intent)
		{
			//Toast.MakeText (context, "Received intent!", ToastLength.Short).Show ();
			if(intent.Action == Intent.ActionBootCompleted){
				var i = new Intent (context, typeof(MainActivity));
				i.SetFlags (ActivityFlags.NewTask);
				context.StartActivity (i);		
			}
		}
	}
}

