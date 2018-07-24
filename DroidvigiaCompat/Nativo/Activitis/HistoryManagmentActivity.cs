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

using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;

namespace DroidvigiaCompat
{
    [Activity(Label = "HistoryManagmentActivity")]
    public class HistoryManagmentActivity : AppCompatActivity, IMenuItemOnMenuItemClickListener
    {
        const int MENU_ITEM_CLEAR_HISTORY= 0;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.HistoryManagmentLayout);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
                SupportActionBar.SetDisplayShowTitleEnabled(false);
            }
            DAL dal = new DAL();
            var recicler = this.FindViewById<RecyclerView>(Resource.Id.recicler_history);

            var layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            layoutManager.ReverseLayout =true;
            layoutManager.StackFromEnd = true;

            ReciclerHistoryAdapter reciclerHistoryAdapter = new ReciclerHistoryAdapter(this, this.FindViewById<SwipeRefreshLayout>(Resource.Id.swiper_history), dal.GetTop10Events(),recicler);
                        
            recicler.HasFixedSize = true;
            recicler.SetAdapter(reciclerHistoryAdapter);
            recicler.SetLayoutManager(layoutManager);
            
            toolbar.NavigationClick += (o, e) => {
                OnBackPressed();
            };

        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            switch (item.ItemId) {
                case MENU_ITEM_CLEAR_HISTORY:
                    ConfirmClearHistoryDialog();
                    break;
            }
            return true;
        }

        private void ConfirmClearHistoryDialog()
        {
            var dialog = new Android.Support.V7.App.AlertDialog.Builder(this)
                .SetTitle("Confirmar").SetMessage("Esta seguro que desea eliminar el historial de sucesos.")
                .SetCancelable(true)
                .SetPositiveButton("Eliminar",new EventHandler<DialogClickEventArgs>((o,evt)=> {
                        DAL dal = new DAL();
                        dal.ClearHistory();                    
                }));
            dialog.Show();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {            
            IMenuItem item1 = menu.Add(0, MENU_ITEM_CLEAR_HISTORY, 0, "Limpiar Historial");
            item1.SetOnMenuItemClickListener(this);
            return base.OnCreateOptionsMenu(menu);
        }
    }
}