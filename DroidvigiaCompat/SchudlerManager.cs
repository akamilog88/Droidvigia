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
using Java.Util;
using DroidvigiaCompat.Utils;

namespace DroidvigiaCompat
{
	public class SchudlerManager
	{
		private Context context;

		public static Schudle GetNearestSchudleData(out DayOfWeek? dayToSchudle){
			DAL dal = new DAL ();
			var schudlesList = dal.GetAllSchudles ().Where(s=>s.Enabled).ToList();
			if (schudlesList.Count == 0){
				dayToSchudle = DateTime.Now.DayOfWeek;
				return null;
			}
			else {
				DayOfWeek day = DateTime.Now.DayOfWeek;
				int hour = DateTime.Now.Hour;
				int minute = DateTime.Now.Minute;
				//Si existe un schudle en el mismo dia con hora posterior a la actual
				var today_schudles = schudlesList.FindAll ((s)=>s.Days.Split(new string[]{";"},StringSplitOptions.RemoveEmptyEntries).ToArray().Any((d)=>d.ToDayOfWeakEnum()==day));
				var today_closer_schudle= today_schudles.Where (s=>s.Hour > hour ||( s.Hour ==  hour && s.Minute > minute)).OrderBy ((s)=>s.Hour).OrderBy ((s)=>s.Minute).ToList();
				if (today_closer_schudle != null &&today_closer_schudle.Count>0){
					dayToSchudle= DateTime.Now.DayOfWeek;
					return today_closer_schudle[0];
				}
				//si hay un solo schudle
				if(schudlesList.Count==1){
					dayToSchudle = NextDayInString (schudlesList[0].Days, DateTime.Now.DayOfWeek);
					return schudlesList [0];
				}
				//si existe un schudle un dia despues del actual antes del fin de la semana
				String day_to_right = "";
				Schudle more_left_to_right =  SchudlerManager.MoreLeftToTheRight(out day_to_right,schudlesList,DateTime.Now.DayOfWeek); 
				if (day_to_right.ToDayOfWeakEnum() != DateTime.Now.DayOfWeek && day_to_right!=""){
					dayToSchudle =(DayOfWeek) day_to_right.ToDayOfWeakEnum();
					return more_left_to_right;
				}
				//si existe un schudle un dia anterior del actual y ya se verifico q no hay ningun schudle en dia posterior se programa  para la proxima semana.
				String day_to_left = "";
				Schudle more_left_to_Left = SchudlerManager.MoreLeftToTheLeft (out day_to_left,schudlesList,DateTime.Now.DayOfWeek); 		
				dayToSchudle = day_to_left.ToDayOfWeakEnum();
				return more_left_to_Left;
			}	
		}

		private static Schudle MoreLeftToTheLeft(out String dayToSchudle,List<Schudle> schudlesList,DayOfWeek pivot){
			dayToSchudle = "";
			Schudle nearest = null;
			if(schudlesList.Count>1){
				nearest = schudlesList [0];
				DayOfWeek dayToSchudleE = NextDayInString (nearest.Days,pivot);
				dayToSchudle = StringExtensions.ToDayOfWeakStringFromEnum (dayToSchudleE);
				for (int i= 1; i<schudlesList.Count; i++) {
					string[] days = schudlesList [i].Days.Split (new string[]{";"}, StringSplitOptions.RemoveEmptyEntries).ToArray ();
					DayOfWeek? temp_nearest = NextDayInString (schudlesList[i].Days,pivot);
					if ((temp_nearest < dayToSchudleE && temp_nearest > pivot )
					    || (temp_nearest==dayToSchudleE && schudlesList [i].Hour<nearest.Hour && temp_nearest < pivot)
					    || (temp_nearest==dayToSchudleE && schudlesList [i].Hour==nearest.Hour &&schudlesList [i].Minute < nearest.Minute && temp_nearest > pivot)) {
						dayToSchudleE = (DayOfWeek)temp_nearest;
						dayToSchudle = StringExtensions.ToDayOfWeakStringFromEnum (dayToSchudleE);
						nearest = schudlesList [i];
					}	
				}
			}
			return nearest;
		}
		private static Schudle MoreLeftToTheRight(out String dayToSchudle,List<Schudle> schudlesList,DayOfWeek pivot){
			Schudle nearest = null;
			dayToSchudle = "";
			if(schudlesList.Count>1){
				nearest = schudlesList [0];
				DayOfWeek dayToSchudleE = NextDayInString (nearest.Days,pivot);
				dayToSchudle = StringExtensions.ToDayOfWeakStringFromEnum (dayToSchudleE);
				for (int i= 1; i<schudlesList.Count; i++) {
					string[] days = schudlesList [i].Days.Split (new string[]{";"}, StringSplitOptions.RemoveEmptyEntries).ToArray ();
					DayOfWeek? temp_nearest = NextDayInString (schudlesList[i].Days,pivot);
					if ((temp_nearest < dayToSchudleE && temp_nearest > pivot )
					    || (temp_nearest==dayToSchudleE && schudlesList [i].Hour<nearest.Hour && temp_nearest > pivot)
					    || (temp_nearest==dayToSchudleE && schudlesList [i].Hour==nearest.Hour &&schudlesList [i].Minute < nearest.Minute && temp_nearest > pivot)) {
						dayToSchudleE = (DayOfWeek)temp_nearest;
						dayToSchudle = StringExtensions.ToDayOfWeakStringFromEnum (dayToSchudleE);
						nearest = schudlesList [i];
					}	
				}
			}

			return nearest;
		}

		private static DayOfWeek NextDayInString(String str,DayOfWeek pivot){

			var days = str.Split (new string[]{";"}, StringSplitOptions.RemoveEmptyEntries).ToList ();
			//Si el mismo dia toma el propio dia
			//var same_day= days.FirstOrDefault (d=>d.ToDayOfWeakEnum()==pivot);
			//if (same_day != null)
			//	return pivot;

			// tomar primero el mas a izquierda de la derecha
			DayOfWeek? left_to_right = DayOfWeek.Saturday;
			for (int j=1; j<days.Count; j++) {
				if (days [j].ToDayOfWeakEnum () <= left_to_right && days [j].ToDayOfWeakEnum ()>pivot )
					left_to_right = days[j].ToDayOfWeakEnum ();
			}
			if ((DayOfWeek)left_to_right != pivot)
				return (DayOfWeek)left_to_right;
			//sino tomar el mas a izquierda de la izquierda
			DayOfWeek? left_to_left = DayOfWeek.Saturday;
			for (int j=1; j<days.Count; j++) {
				if (days [j].ToDayOfWeakEnum () <= left_to_left && days [j].ToDayOfWeakEnum ()<pivot )
					left_to_left = days[j].ToDayOfWeakEnum ();
			}
			return (DayOfWeek)left_to_left;
		}

		public SchudlerManager(Context c){
			context = c;
		}

		public void RegisterNextSchudle(){
			DayOfWeek? daytoSchudle = DateTime.Now.DayOfWeek;
			Schudle nearest=GetNearestSchudleData (out daytoSchudle);
			if(nearest!=null){
				Cancel();
				RegisterSchudle (nearest,(DayOfWeek)daytoSchudle);
			}
		}

		public void RegisterSchudle(Schudle s,DayOfWeek dayofwaek){
			AlarmManager alarm_mngr =(AlarmManager) context.GetSystemService (Context.AlarmService);
			Intent intent = new Intent ("DroidVigia.DroidVigia.DroidVigia.SchudledAlarm");		
			PendingIntent pintent = PendingIntent.GetBroadcast (context, 0, intent, PendingIntentFlags.OneShot);
			var pref_model= Global.GetAppPreferences (context);
			pref_model.Next_Schudle_Name = s.Label;
			var schudled_time = new DateTime (DateTime.Now.Year, DateTime.Now.Month,DateTime.Now.Day,s.Hour, s.Minute, 0);
			int diff_days = 0;
			if(dayofwaek > DateTime.Now.DayOfWeek)
				diff_days = dayofwaek - DateTime.Now.DayOfWeek;
			if(dayofwaek < DateTime.Now.DayOfWeek)
				diff_days = (DayOfWeek.Saturday - DateTime.Now.DayOfWeek)+((int)dayofwaek+1);
			schudled_time =schudled_time.AddDays ( diff_days );
			long millis =(long) (schudled_time - DateTime.Now).TotalMilliseconds;
			//Calendar future= Calendar.GetInstance(Java.Util.TimeZone.Default);		
			//future.Add(Java.Util.CalendarField.Millisecond, (int)millis);			
			alarm_mngr.Set (AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime()+ millis , pintent);
		}

		public void RegisterRestartServiceSchudle(){
			AlarmManager alarm_mngr =(AlarmManager) context.GetSystemService (Context.AlarmService);
			Intent intent = new Intent ("DroidVigia.DroidVigia.DroidVigia.SchudledAlarm");		
			PendingIntent pintent = PendingIntent.GetBroadcast (context, 0, intent, PendingIntentFlags.OneShot);
			var pref_model= Global.GetAppPreferences (context);
			pref_model.Next_Schudle_Name = SchuldeReciver.START_SERVICE_SCHUDLE;
			long millis =10000;		
			alarm_mngr.Set (AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime()+ millis , pintent);
		}

		public void RegisterSchudleCall(){
			AlarmManager alarm_mngr =(AlarmManager) context.GetSystemService (Context.AlarmService);	
			Intent intent = new Intent ("DroidVigia.DroidVigia.DroidVigia.SchudledCall");		
			PendingIntent pintent = PendingIntent.GetBroadcast (context, 0, intent, PendingIntentFlags.OneShot);
			long millis =20000;		
			alarm_mngr.Set (AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime()+ millis , pintent);
		}
		public void CancelSchudleCall(){
			AlarmManager alarm_mngr =(AlarmManager) context.GetSystemService (Context.AlarmService);	
			Intent intent = new Intent ("DroidVigia.DroidVigia.DroidVigia.SchudledCall");		
			PendingIntent pintent = PendingIntent.GetActivity (context, 0, intent, PendingIntentFlags.OneShot);
			alarm_mngr.Cancel (pintent);
		}

		public void Cancel(){
			AlarmManager alarm_mngr =(AlarmManager) context.GetSystemService (Context.AlarmService);
			Intent intent = new Intent ("DroidVigia.DroidVigia.DroidVigia.SchudledAlarm");
			PendingIntent pintent = PendingIntent.GetBroadcast (context, 0, intent, PendingIntentFlags.OneShot);		
			alarm_mngr.Cancel (pintent);
		}
	}
}

