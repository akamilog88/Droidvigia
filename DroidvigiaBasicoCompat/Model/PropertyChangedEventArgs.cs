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
	public class PropertyChangedEventArgs:EventArgs
	{
		public String PropertyName{ get; private set;}
		public PropertyChangedEventArgs(string name){
			PropertyName = name;
		}
	}
}

