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
	public class ZonesAdapter:BaseAdapter<Zone>
	{
		Activity context;
		public List<Zone> Zones;

		public ZonesAdapter (Activity context, List<Zone> zones):base()
		{
			this.context = context;
			this.Zones = zones;
		}

		public override bool AreAllItemsEnabled ()
		{
			return true;
		}
		public override int Count
		{
			get { return this.Zones.Count; }
		}
		public override Zone this[int position]
		{
			get { return this.Zones[position]; }
		}
		public void AddZone(Zone z){
			Zones.Add (z);
			NotifyDataSetInvalidated ();
		}
		public void DetachZone(int id){
			var result = Zones.Remove(Zones.Find ((z)=>z.Id==id));
			if(result)
				NotifyDataSetInvalidated ();
		}
		public override View GetView(int position, View convertView, ViewGroup
		                             parent)
		{
			//Get our object for this position
			var item = this.Zones[position];
			//Try to reuse convertView if it’s not null, otherwise inflate it from
			//our item layout
			// This gives us some performance gains by not always inflating a new
			//view
			// This will sound familiar to MonoTouch developers with
			//UITableViewCell.DequeueReusableCell()
			var view = convertView;
			if (convertView == null || !(convertView is RelativeLayout))
				view = context.LayoutInflater.Inflate (Android.Resource.Layout.SimpleListItem2, parent, false);
				//view = context.LayoutInflater.Inflate(Resource.Layout.PartitionItemLayout,
				                                      //parent, false);
			//Find references to each subview in the list item’s view

			var text1 = view.FindViewById(Android.Resource.Id.Text1) as TextView;
			var text2 = view.FindViewById(Android.Resource.Id.Text2) as TextView;
		
			//Assign this item’s values to the various subviews
			text2.Text = "Code:"+item.Code+ ";" +" Aviso:" +(item.Inmediate? "Inmediato.":"Retardado.");
			text1.Text = item.Name;
			//Finally return the view
			return view;
		}
		public override long GetItemId(int position)
		{
			return position;
		}
	}
}
