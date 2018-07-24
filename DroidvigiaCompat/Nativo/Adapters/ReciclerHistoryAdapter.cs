using Android.Support.V7.Widget;
using Android.Views;
using System;
using System.Threading;
using Android.Content;
using System.Collections.Generic;
using Android.Widget;
using Android.Support.V4.Widget;

namespace DroidvigiaCompat
{
    public class ReciclerHistoryAdapter : RecyclerView.Adapter
    {
        private Context context;
        private List<HistoryItem> list;
        private SwipeRefreshLayout swiper;

        private int offset;
        public override int ItemCount => list.Count;
        public event EventHandler<int> ItemClick;

        public RecyclerView recycler;

        public ReciclerHistoryAdapter(Context context,SwipeRefreshLayout swiper,List<HistoryItem> items, RecyclerView recycler)
        {
            this.context = context;
            this.swiper = swiper;
            list = items;            
            swiper.Refresh += Swiper_Refresh;
            offset = 0;
            this.recycler = recycler;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ReciclerHistoryViewHolder vh = holder as ReciclerHistoryViewHolder;         
            vh.Action.Text = "Evento: "+ Action.GetStateName( list[position].State);//item.Time.ToShortDateString() + item.Time.ToShortTimeString();            
            vh.Time.Text = list[position].Time.ToShortDateString()+ " " + list[position].Time.ToShortTimeString();
            vh.Details.Text = list[position].Detail;
        }

        private void Swiper_Refresh(object sender, EventArgs e)
        {
            swiper.Refreshing = true;
            new System.Threading.Tasks.TaskFactory().StartNew(() => {
                offset++;
                DAL dal = new DAL();
                var next10 = dal.GetTop10Events(offset * 10);               
                this.list.AddRange(next10);                
                this.NotifyItemRangeInserted(0, next10.Count);
                this.NotifyItemRangeChanged(0, next10.Count);
                recycler.SmoothScrollToPosition(this.list.Count /2); 
               
                swiper.Refreshing = false;                  
            });
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