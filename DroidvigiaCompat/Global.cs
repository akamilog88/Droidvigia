using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Telephony;

namespace DroidvigiaCompat
{
	enum AplicationLicenseStatus{No_Activated,Expired,Activated};
	public class Global
	{
		private static Global _instance=null;
		public static CallState lastState;

		private static PreferencesModel pref_model;
		public static PreferencesModel Prefrences{ get{return pref_model;}
			private set{
				if (pref_model != value) {
					pref_model = value;
				}
			}
		}
		public const String PREFERENCES_NAME = "DroidvigiaCompat.DroidvigiaCompat.DroidvigiaCompat";

		public  static Global getInstance(){
			if (_instance == null)
				_instance = new Global ();
			return _instance;
		}
		private Global(){
			//Prefrences = new PreferencesModel (GetSharedPreferences);
		}
		public static ISharedPreferences GetSharedPreferences(Context c){
			return c.GetSharedPreferences (Global.PREFERENCES_NAME, FileCreationMode.Private);
		}
		public static PreferencesModel GetAppPreferences(Context c){
			return new PreferencesModel (GetSharedPreferences(c));
		}

		private static object tone_reciver_lock;
		public static object Get_Tone_Reciver_Lock(){
			if(tone_reciver_lock==null){
				tone_reciver_lock = new object ();
			}
			return tone_reciver_lock;
		}
	}
}
