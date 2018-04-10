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

namespace DroidVIGIA
{
	[Activity (Label = "Planificador")]			
	public class SchudlerManagerActivity : ListActivity,IMenuItemOnMenuItemClickListener	
	{
		const int MENU_ITEM_CREATE = 1;
		const int MENU_ITEM_DELETE = 2;
		SchuldeAdapter adapter;
		int item_long_pressed_index=-1;
		Android.Support.V4.App.DialogFragment partitions_dialog;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
				
		}

		private void ItemLongPressed(object sender, AdapterView.ItemLongClickEventArgs e) {
			item_long_pressed_index=e.Position;
			e.Handled=false;
		}
		protected override void OnResume ()
		{
			base.OnResume ();
			var dal = new DAL ();
			adapter =  new SchuldeAdapter (this, dal.GetAllSchudles());
			this.ListAdapter = adapter;
			this.ListView.ItemLongClick += ItemLongPressed;
			RegisterForContextMenu (this.ListView);		
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			this.ListView.ItemLongClick -= ItemLongPressed;
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
			var item2= menu.Add(0, MENU_ITEM_DELETE, Android.Views.Menu.None, str2);				;
			item2.SetOnMenuItemClickListener (this);
		}
		protected override void OnListItemClick (ListView l, View v, int position, long id)
		{
			base.OnListItemClick (l, v, position, id);
			Intent i = new Intent(this,typeof(SchudleDataActivity));
			i.PutExtra(SchudleDataActivity.SCHUDLE_ID_EXTRA,adapter[position].Label);
			StartActivity(i);
		}
	}
}

