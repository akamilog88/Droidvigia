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
	[IntentFilter(new string[]{"DroidVigia.DroidVigia.DroidVigia.SchudledAlarm"})]
	public class SchuldeReciver : BroadcastReceiver
	{
		public const int ALARM_NOTIFICATION_ID=333;
		public const string START_SERVICE_SCHUDLE="restart_sevice_schudle";

		public override void OnReceive (Context context, Intent intent)
		{
			DAL dal = new DAL ();
			var pref_model = Global.GetAppPreferences (context);
			string label = pref_model.Next_Schudle_Name;
			if(label!=START_SERVICE_SCHUDLE){
				var schudle =dal.GetSchudleByName (label);
				var partitions_name =schudle.PartitionNames.Split (new string[]{";"}, StringSplitOptions.RemoveEmptyEntries).ToList ();
				var partitions = new List<Partition> ();
				partitions_name.ForEach (pn=> {partitions.Add(dal.GetPartitionByName(pn));});

				if (schudle.ActionId == Action.ACTION_ID_ACTIVATED) {			
					partitions.ForEach (p=>{
						dal.UpdatePartitionState(p.Id,true);
						string message = "Tarea Programada:"+label;
						var h =new HistoryItem{Time=DateTime.Now,State=Action.ACTION_ID_ACTIVATED,PartitionName=p.Name,Detail=message};
						dal.RegiterEvent(h);
					});
				} else {
					partitions.ForEach (p=>{
						dal.UpdatePartitionState(p.Id,false);
						string message = "Tarea Programada:"+label;
						var h =new HistoryItem{Time=DateTime.Now,State=Action.ACTION_ID_DESACTIVATED,PartitionName=p.Name,Detail=message};
						dal.RegiterEvent(h);
					});
				}
				partitions = dal.GetAllPartitions ();
				bool is_pactivated = partitions.Find ((part)=>part.Activated) != null;
				if (!is_pactivated)
					pref_model.Ready = false;
				else if (!pref_model.Ready && is_pactivated)
					pref_model.Ready = true;

				NotificationManager n_mngr = (NotificationManager)context.GetSystemService (Service.NotificationService);
				Notification n = new Notification (Resource.Drawable.Icon,"", DateTime.UtcNow.Millisecond);
				Intent notificationIntent = new Intent(context, typeof(MainActivity));
				//PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, 0);
				n.Defaults |= NotificationDefaults.All; 
				n.Flags |= NotificationFlags.OnlyAlertOnce;
				n.SetLatestEventInfo (context, "Tarea Planificada", "Nombre: " + schudle.Label  , null);
				n_mngr.Notify (ALARM_NOTIFICATION_ID, n);

				DayOfWeek? daytoSchudle = DateTime.Now.DayOfWeek;
				Schudle nearest =SchudlerManager.GetNearestSchudleData (out daytoSchudle);
				SchudlerManager schudler = new SchudlerManager (context);
				schudler.Cancel();
				if(nearest!=null){
					schudler.RegisterSchudle (nearest,(DayOfWeek)daytoSchudle);
				}
			}
			PowerManager pwr_mngr =(PowerManager) context.GetSystemService (Context.PowerService);
			PowerManager.WakeLock w=  pwr_mngr.NewWakeLock (WakeLockFlags.Full, "Schudle" );
			w.Acquire (5000);
			Intent i = new Intent (context, typeof(DetectorService));							
			context.StartService(i);
			//Intent i = new Intent (context, typeof(MainActivity));
			//context.StartActivity (i);
			//context.StopService (i);			
			//context.StartService(i);

			//PowerManager pwr_mngr =(PowerManager) context.GetSystemService (Context.PowerService);
			//pwr_mngr.NewWakeLock (WakeLockFlags.Full, "Schudle" );
		}
	}
}

