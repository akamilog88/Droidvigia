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

namespace DroidToneDecoder
{
	public class DTMFTokenBuildedEventArgs:EventArgs
	{
		public DateTime StartBuild{ get; private set;}
		public DateTime EndBuild{ get; private set;}
		public String Token{ get; private set;}

		public DTMFTokenBuildedEventArgs(DateTime start_build,DateTime end_build,String token){
			StartBuild = start_build;
			EndBuild = end_build;
			Token = token;
		}
	}
}

