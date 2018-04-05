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

namespace DroidvigiaBasicoCompat
{
    [BroadcastReceiver]
    [IntentFilter(new string[] { "android.intent.action.HEADSET_PLUG" })]
    public class HeadSetConnectedReciver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == Intent.ActionHeadsetPlug)
            {
                int state = intent.GetIntExtra("state", -1);
                int microfone = intent.GetIntExtra("microphone", -1);

                Intent i;
                PreferencesModel prefmodel = Global.GetAppPreferences(context);
                switch (state)
                {
                    case 0:
                        Global.GetAppPreferences(context).Has_Audio_Focus = false;
                        //i = new Intent (context, typeof(DetectorService));
                        //context.StopService(i);
                        //Log.d(TAG, "Headset is unplugged");
                        break;
                    case 1:
                        if (!Global.GetAppPreferences(context).Has_Audio_Focus)
                        {
                            i = new Intent(context, typeof(MainActivity));
                            i.SetFlags(ActivityFlags.NewTask);
                            context.StartActivity(i);
                            Global.GetAppPreferences(context).Has_Audio_Focus = true;
                            //Log.d(TAG, "Headset is plugged");
                        }
                        break;
                }
            }
        }
    }

}

