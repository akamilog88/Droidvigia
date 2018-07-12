using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DroidToneDecoder;

namespace DroidToneDecoder
{
	public class DTMFTokenParser
	{
		public const int TOKEN_DELIMITER= DTMFConstanst.DSTAR;
		public const int MAX_CHARS = 6;
		readonly TimeSpan AUTO_DELIMITER_TIMESPAN =new TimeSpan(0,0,0,3,0);
		readonly TimeSpan MAX_BUILD_TIMESPAN = new TimeSpan(0,0,0,6,0);

		private string current_token="";
		private int last_dtmf_const=-1;

		private DateTime first_dtmf_time;
		private DateTime last_dtmf_time;

		private object sync_object;

		public event EventHandler<DTMFTokenBuildedEventArgs> NewToken;

		public DTMFTokenParser(){
			//sync_object= new object ();
			Reset ();
		}

		public void Digest(int dtmf_char_const = DTMFConstanst.DSIL){
			//lock(sync_object){
				if(dtmf_char_const>=0){
					DateTime current = DateTime.UtcNow;
					TimeSpan delta_time = current - last_dtmf_time;
					if ((dtmf_char_const == TOKEN_DELIMITER || current_token.Length == MAX_CHARS ||
				       ((DateTime.UtcNow - first_dtmf_time) > MAX_BUILD_TIMESPAN)  ||
					   (dtmf_char_const == DTMFConstanst.DSIL && delta_time > AUTO_DELIMITER_TIMESPAN))
					    && current_token.Length > 0)
					{
					if (NewToken != null)
						//NewToken.BeginInvoke (this, new DTMFTokenBuildedEventArgs(first_dtmf_time,first_dtmf_time + delta_time,current_token),null,null);
						//Tokens.Add (new Tuple<string, DateTime, DateTime>(current_token,first_dtmf_time,last_dtmf_time));
						NewToken (this, new DTMFTokenBuildedEventArgs (first_dtmf_time, first_dtmf_time + delta_time, current_token));
						Reset ();
						return;
					}
					if (dtmf_char_const == DTMFConstanst.DSIL) {
						last_dtmf_const = -1;
					} else {
					if (current_token.Length == 0)
						first_dtmf_time = current;
					}
					if (last_dtmf_const != dtmf_char_const && dtmf_char_const != DTMFConstanst.DSIL && dtmf_char_const >= 0 && dtmf_char_const!=TOKEN_DELIMITER) {
						current_token += DTMFConstanst.dtran [dtmf_char_const];
						last_dtmf_const = dtmf_char_const;
						last_dtmf_time = current;
					} 					
				}
			//}
		}

		public void Reset(){
			current_token = "";
			last_dtmf_const = -1;

			DateTime temp = DateTime.UtcNow;
			first_dtmf_time = temp;
			last_dtmf_time = temp;
		}
	}
}

