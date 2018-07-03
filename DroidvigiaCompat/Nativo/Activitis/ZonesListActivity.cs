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
using DroidvigiaCompat.Utils;

namespace DroidvigiaCompat
{
	[Activity (Label = "Zonas")]			
	public class ZonesListActivity : ListActivity,IMenuItemOnMenuItemClickListener
	{
		const int MENU_ITEM_CREATE_ZONE = 1;
		const int MENU_ITEM_DELETE_ZONE = 2;
		const int MENU_ITEM_EDIT_ZONE = 3;
		ZonesAdapter adapter;
		int PartitionId = -1;
		int item_long_pressed_index=-1;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			// Create your application here
			PartitionId = this.Intent.GetIntExtra ("PartitionId",-1);
			if (PartitionId >= 0) {
				DAL dal = new DAL ();
				var zones = dal.GetPartitionsZones (PartitionId);	
				adapter=new ZonesAdapter(this,zones);
				this.ListAdapter = adapter;

				this.ListView.ItemLongClick += (sender, e) => {
					item_long_pressed_index=e.Position;
					e.Handled=false;
				};
				RegisterForContextMenu (this.ListView);			
			}
		}	
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			IMenuItem item1= menu.Add (0,MENU_ITEM_CREATE_ZONE,0,"Crear Zona");
			item1.SetOnMenuItemClickListener (this);
			return base.OnCreateOptionsMenu (menu);
		}
		public override void OnCreateContextMenu(Android.Views.IContextMenu menu, View v,
		                                         Android.Views.IContextMenuContextMenuInfo menuInfo)
		{
			base.OnCreateContextMenu(menu, v, menuInfo);
			Java.Lang.ICharSequence str0 = new Java.Lang.String("Opciones");		
			Java.Lang.ICharSequence str2 = new Java.Lang.String("Eliminar");
			menu.SetHeaderTitle(str0);		
			var item2= menu.Add(0, MENU_ITEM_DELETE_ZONE, Android.Views.Menu.None, str2);				;
			item2.SetOnMenuItemClickListener (this);
		}
		public bool OnMenuItemClick (IMenuItem item)
		{
			DAL dal;
			View view;
			AlertDialog.Builder dialog;
			switch(item.ItemId){			
				case (MENU_ITEM_DELETE_ZONE):
				dal = new DAL ();
				dal.DeleteZone (adapter[item_long_pressed_index].Id);
				adapter.DetachZone (adapter[item_long_pressed_index].Id);
				break;
				case ( MENU_ITEM_CREATE_ZONE):
				view = LayoutInflater.Inflate (Resource.Layout.ZoneInfoDialog,null);				                                     
				dialog = new AlertDialog.Builder (this)
					.SetPositiveButton ("Crear", new EventHandler<DialogClickEventArgs> ((object sender, DialogClickEventArgs e) =>{
						var s = sender as Dialog;
						var tb_name= s.FindViewById<EditText>(Resource.Id.dialog_tb_zoneName);
						var tb_code= s.FindViewById<EditText>(Resource.Id.dialog_tb_zoneCode);
						var chkb= s.FindViewById<CheckBox>(Resource.Id.dialog_chk_zone_inmediate);
						if(tb_code.Text.IsDTMFSecuence() && tb_name.Text.Length>0){
							dal = new DAL();
							var z =new Zone{Name=tb_name.Text,Code=tb_code.Text,Inmediate=chkb.Checked};
							int res =dal.NewZone(z,PartitionId);
							if(res>0)
								adapter.AddZone(dal.GetZoneByName(z.Name));
						}
					})).SetTitle ("Nueva Zona");
				dialog.SetView (view);
				dialog.Show ();

				break;
			}
			return true;
		}
	}
}

