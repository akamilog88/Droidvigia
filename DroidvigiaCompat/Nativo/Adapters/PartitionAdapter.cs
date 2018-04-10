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
	public class PartitionAdapter:BaseAdapter<Partition>
	{
		Activity context;
		PreferencesModel prefs_model;
		public List<Partition> Partitions;

		public PartitionAdapter (Activity context, List<Partition> partitions):base()
		{
			this.context = context;
			this.Partitions = partitions;
			prefs_model = Global.GetAppPreferences (this.context);
		}

		public override bool AreAllItemsEnabled ()
		{
			return true;
		}
		public override int Count
		{
			get { return this.Partitions.Count; }
		}
		public override Partition this[int position]
		{
			get { return this.Partitions[position]; }
		}
		public void AddPartition(Partition p){
			Partitions.Add (p);
			NotifyDataSetInvalidated ();
		}
		public void DetachPartition(int id){
			var result = Partitions.Remove(Partitions.Find ((p)=>p.Id==id));
			if(result)
				NotifyDataSetInvalidated ();
		}
		public override View GetView(int position, View convertView, ViewGroup
		                             parent)
		{
			//Get our object for this position
			var item = this.Partitions[position];
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
			text2.Text = "Code:"+item.Code;
			text1.Text = item.Name;
			tbutton.Checked=item.Activated;
			tbutton.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) => {
				DAL dal = new DAL();
				var partition = this.Partitions[position];
				if(e.IsChecked){
					partition.Activated=true;
					dal.UpdatePartitionState(partition.Id,true);
					dal.RegiterEvent(new HistoryItem{Time=DateTime.Now,State=Action.ACTION_ID_ACTIVATED,PartitionName=partition.Name,Detail=""});
					if(!prefs_model.Ready)
						prefs_model.Ready = true;
				}else{
					partition.Activated=false;
					dal.UpdatePartitionState(partition.Id,false);
					dal.RegiterEvent(new HistoryItem{Time=DateTime.Now,State=Action.ACTION_ID_DESACTIVATED,PartitionName=partition.Name,Detail=""});
					if (Partitions.Find ((part)=>part.Activated) == null)
						prefs_model.Ready = false;
				}
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

