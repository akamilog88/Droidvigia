using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DroidvigiaCompat{
public class HistoryGridAdapter :BaseAdapter<HistoryItem>{

	Activity context;
	public List<HistoryItem> Items;

	public HistoryGridAdapter (Activity context, List<HistoryItem> items):base()
	{
		this.context = context;
		this.Items = items;		
	}

	public override bool AreAllItemsEnabled ()
	{
		return true;
	}
	public override int Count
	{
		get { return this.Items.Count; }
	}
	public override HistoryItem this[int position]
	{
		get { return this.Items[position]; }
	}	
	public void AddHistoryData(HistoryItem i){
		Items.Add (i);
		NotifyDataSetInvalidated ();
	}

	public override View GetView(int position, View convertView, ViewGroup
	                             parent)
	{
		//Get our object for this position
		var view = convertView;		
		var item = this.Items[position];
		
		if (convertView == null || !(convertView is RelativeLayout))
			view = context.LayoutInflater.Inflate(Resource.Layout.HistoryItem,
			                                      parent, false);	

		var text1 = view.FindViewById(Resource.Id.lb_history_item1) as TextView;
		var text2 = view.FindViewById(Resource.Id.lb_history_item2) as TextView;
		var text3 = view.FindViewById(Resource.Id.lb_history_item3) as TextView;

		text1.Text = item.Time.ToShortDateString() + item.Time.ToShortTimeString();
		text2.Text = item.PartitionName;
		text3.Text = Action.GetStateName( item.State);
	
		return view;
	}
	public override long GetItemId(int position)
	{
		return position;
	}
	}
}