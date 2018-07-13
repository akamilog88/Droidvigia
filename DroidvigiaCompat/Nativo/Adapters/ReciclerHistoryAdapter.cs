using Android.Support.V7.Widget;
using Android.Views;
using System;
using System.Collections.Generic;
using Android.Widget;

namespace DroidvigiaCompat
{
    public class ReciclerHistoryAdapter : RecyclerView.Adapter
    {

        private List<HistoryItem> list;
        public override int ItemCount => list.Count;
        public event EventHandler<int> ItemClick;

        public ReciclerHistoryAdapter(List<HistoryItem> items)
        {
            list = items;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ReciclerHistoryViewHolder vh = holder as ReciclerHistoryViewHolder;
            vh.Details.Text = list[position].Detail;
            vh.Action.Text = Action.GetStateName( list[position].State);//item.Time.ToShortDateString() + item.Time.ToShortTimeString();
            vh.Time.Text = list[position].Time.ToShortDateString()+ list[position].Time.ToShortTimeString();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ReciclerHistoryItem, parent, false);
            ReciclerHistoryViewHolder vh = new ReciclerHistoryViewHolder(itemView, OnClick);
            return vh;
        }
        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }

    public class ReciclerHistoryViewHolder : RecyclerView.ViewHolder {
        public TextView Details { get;private set; }
        public TextView Action { get;private set; }
        public TextView Time { get; private set; }
        
        public ReciclerHistoryViewHolder(View view,Action<int> listener):base(view)
        {
            Details = view.FindViewById<TextView>(Resource.Id.tv_history_item_details);
            Action = view.FindViewById<TextView>(Resource.Id.tv_history_item_Event);
            Time = view.FindViewById<TextView>(Resource.Id.tv_history_item_time);
            view.Click += (sender, e) => listener(base.AdapterPosition);
        }
    }
}