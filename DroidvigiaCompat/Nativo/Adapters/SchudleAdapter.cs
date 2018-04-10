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
	public class SchuldeAdapter:BaseAdapter<Schudle>
	{
		Activity context;
		PreferencesModel prefs_model;
		public List<Schudle> Schudles;

		public SchuldeAdapter (Activity context, List<Schudle> schudles):base()
		{
			this.context = context;
			this.Schudles = schudles;
			prefs_model = Global.GetAppPreferences (this.context);
		}

		public override bool AreAllItemsEnabled ()
		{
			return true;
		}
		public override int Count
		{
			get { return this.Schudles.Count; }
		}
		public override Schudle this[int position]
		{
			get { return this.Schudles[position]; }
		}
		public void AddSchudle(Schudle s){
			Schudles.Add (s);
			NotifyDataSetInvalidated ();
		}
		public void DetachSchudle(string label){
			var result = Schudles.Remove(Schudles.Find ((s)=>s.Label==label));
			if(result)
				NotifyDataSetInvalidated ();
		}
		public override View GetView(int position, View convertView, ViewGroup
		                             parent)
		{
			//Get our object for this position
			var item = this.Schudles[position];
			//Try to reuse convertView if it’s not null, otherwise inflate it from
			//our item layout
			// This gives us some performance gains by not always inflating a new
			//view
			// This will sound familiar to MonoTouch developers with
			//UITableViewCell.DequeueReusableCell()
			var view = convertView;
			if (convertView == null || !(convertView is RelativeLayout))
				view = context.LayoutInflater.Inflate(Resource.Layout.PartitionItemLayout,
				                                      parent, false);
			//Find references to each subview in the list item’s view

			var text1 = view.FindViewById(Resource.Id.partitionLayout_textView1) as TextView;
			var text2 = view.FindViewById(Resource.Id.partitionLayout_textView2) as TextView;

			var tbutton = view.FindViewById(Resource.Id.partitionLayout_toggleButton1) as ToggleButton;

			//Assign this item’s values to the various subviews
			text2.Text = item.Label;
			text1.Text = item.Hour+":"+item.Minute;
			tbutton.Checked=item.Enabled;
			tbutton.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) => {
				DAL dal = new DAL();
				var schudle = this.Schudles[position];
				if(e.IsChecked){
					schudle.Enabled=true;
					dal.UpdateSchudleState(item.Label,true);							
				}else{
					schudle.Enabled=false;	
					dal.UpdateSchudleState(item.Label,false);
				}
				DayOfWeek? daytoSchudle = DateTime.Now.DayOfWeek;
				Schudle nearest =SchudlerManager.GetNearestSchudleData (out daytoSchudle);
				SchudlerManager schudler = new SchudlerManager (context);
				schudler.Cancel();
				if(nearest!=null)
					schudler.RegisterSchudle (nearest,(DayOfWeek)daytoSchudle);	
			};
			//Finally return the view
			return view;
		}
		public override long GetItemId(int position)
		{
			return position;
		}
	}
}

