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
using Android.Support.V4.App;


using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;

namespace DroidvigiaCompat
{
	[Activity (Label = "Planificador")]			
	public class SchudlerManagerActivity : AppCompatActivity, IMenuItemOnMenuItemClickListener	
	{
		const int MENU_ITEM_CREATE = 1;
		const int MENU_ITEM_DELETE = 2;
		SchuldeAdapter adapter;
		int item_long_pressed_index=-1;
		Android.Support.V4.App.DialogFragment partitions_dialog;

        ListView listView;
        

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

            SetContentView(Resource.Layout.listview_shell_activity);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
                SupportActionBar.SetDisplayShowTitleEnabled(false);
            }
            var dal = new DAL();
            adapter = new SchuldeAdapter(this, dal.GetAllSchudles());            

            this.listView = FindViewById<ListView>(Resource.Id.listview_shell);
            this.listView.Adapter = adapter;

            this.listView.ItemClick += (o, e) => {
                Intent i = new Intent(this, typeof(SchudleDataActivity));
                i.PutExtra(SchudleDataActivity.SCHUDLE_ID_EXTRA, adapter[e.Position].Label);
                StartActivity(i);
            };

            this.listView.ItemLongClick += (sender, e) => {
                item_long_pressed_index = e.Position;
                e.Handled = false;
            };
            toolbar.NavigationClick += (o, e) => {                
                    OnBackPressed();                
            };          

           
            RegisterForContextMenu(this.listView);
        }        

		protected override void OnPause ()
		{
			base.OnPause ();			
		}
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			IMenuItem item1= menu.Add (0,MENU_ITEM_CREATE,0,"Crear Planificacion");
			item1.SetOnMenuItemClickListener (this);
			return base.OnCreateOptionsMenu (menu);
		}

		public bool OnMenuItemClick (IMenuItem item)
		{
				
			switch(item.ItemId){
			case (MENU_ITEM_DELETE):
				DAL dal = new DAL ();	
				dal.DeleteSchudle (adapter[item_long_pressed_index].Label);

				adapter.DetachSchudle (adapter[item_long_pressed_index].Label);
				DayOfWeek? daytoSchudle = DateTime.Now.DayOfWeek;
				Schudle nearest =SchudlerManager.GetNearestSchudleData (out daytoSchudle);
				SchudlerManager schudler = new SchudlerManager (this);
				schudler.Cancel();
				if(nearest!=null){
					schudler.RegisterSchudle (nearest,(DayOfWeek)daytoSchudle);
				}

				break;
			case ( MENU_ITEM_CREATE):
				Intent i = new Intent (this, typeof(SchudleDataActivity));
				StartActivity (i);
				break;
			}
			return true;
		}

		public override void OnCreateContextMenu(Android.Views.IContextMenu menu, View v,
		                                         Android.Views.IContextMenuContextMenuInfo menuInfo)
		{
			base.OnCreateContextMenu(menu, v, menuInfo);
			Java.Lang.ICharSequence str0 = new Java.Lang.String("Opciones");	
			Java.Lang.ICharSequence str2 = new Java.Lang.String("Eliminar");
			menu.SetHeaderTitle(str0);		
			var item2= menu.Add(0, MENU_ITEM_DELETE, Android.Views.Menu.None, str2);				
			item2.SetOnMenuItemClickListener (this);
		}	
	}
}

