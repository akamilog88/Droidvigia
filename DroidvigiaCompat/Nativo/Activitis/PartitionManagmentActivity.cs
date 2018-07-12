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


using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;

namespace DroidvigiaCompat
{
	[Activity (Label = "Particiones")]			
	public class PartitionManagmentActivity : AppCompatActivity,IMenuItemOnMenuItemClickListener
	{

		const int MENU_ITEM_CREATE_PARTITION = 1;
		const int MENU_ITEM_DELETE_PARTITION = 2;
		const int MENU_ITEM_EDIT_PARTITION = 3;
		PartitionAdapter adapter;
		int item_long_pressed_index=-1;

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

            var dal = new DAL ();            

			adapter =  new PartitionAdapter (this, dal.GetAllPartitions());

            this.listView = FindViewById<ListView>(Resource.Id.listview_shell);
            this.listView.Adapter = adapter;            

			this.listView.ItemLongClick += (sender, e) => {
				item_long_pressed_index=e.Position;
				e.Handled=false;
			};
            this.listView.ItemClick += (o,e) =>{
                Intent i = new Intent(this, typeof(ZonesListActivity));
                i.PutExtra("PartitionId", adapter[e.Position].Id);
                StartActivity(i);
            };
            toolbar.NavigationClick += (o, e) => {
                OnBackPressed();
            };

            RegisterForContextMenu (this.listView);			
		}
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			IMenuItem item1= menu.Add (0,MENU_ITEM_CREATE_PARTITION,0,"Crear Particion");
			item1.SetOnMenuItemClickListener (this);
			return base.OnCreateOptionsMenu (menu);
		}
	
		public override void OnCreateContextMenu(Android.Views.IContextMenu menu, View v,
		                                         Android.Views.IContextMenuContextMenuInfo menuInfo)
		{
			base.OnCreateContextMenu(menu, v, menuInfo);
			Java.Lang.ICharSequence str0 = new Java.Lang.String("Opciones");
			Java.Lang.ICharSequence str1 = new Java.Lang.String("Editar");
			Java.Lang.ICharSequence str2 = new Java.Lang.String("Eliminar");
			menu.SetHeaderTitle(str0);
			var item1= menu.Add(0, MENU_ITEM_EDIT_PARTITION,Android.Views.Menu.None, str1);
			item1.SetOnMenuItemClickListener (this);
			var item2= menu.Add(0, MENU_ITEM_DELETE_PARTITION, Android.Views.Menu.None, str2);			
			item2.SetOnMenuItemClickListener (this);
		}
		public bool OnMenuItemClick (IMenuItem item)
		{
			DAL dal;
			View view;
            Android.App.AlertDialog.Builder dialog;
			switch(item.ItemId){
			case ( MENU_ITEM_EDIT_PARTITION):
				view = LayoutInflater.Inflate (Resource.Layout.PartitionInfoDialog,null);	
				var tb_code_a= view.FindViewById<EditText>(Resource.Id.dialog_tb_partitionCode);
				tb_code_a.Visibility= ViewStates.Gone;
				dialog = new Android.App.AlertDialog.Builder (this)
					.SetPositiveButton ("Aceptar", new EventHandler<DialogClickEventArgs> ((object sender, DialogClickEventArgs e) =>{
						var s = sender as Dialog;
						var tb_name= s.FindViewById<EditText>(Resource.Id.dialog_tb_partitionName);
						if(tb_name.Text.Length>0){
							dal = new DAL();
							dal.ChangePartitionName(adapter[item_long_pressed_index].Id,tb_name.Text);
							adapter.NotifyDataSetChanged();
						}
					}))
						.SetTitle ("Renombrar Particion");
				dialog.SetView (view);
				dialog.Show ();

					break;
			case (MENU_ITEM_DELETE_PARTITION):
				dal = new DAL ();
				dal.DeletePartition (adapter[item_long_pressed_index].Id);
				adapter.DetachPartition (adapter[item_long_pressed_index].Id);
					break;
			case ( MENU_ITEM_CREATE_PARTITION):
				view = LayoutInflater.Inflate (Resource.Layout.PartitionInfoDialog,null);				                                     
				dialog = new Android.App.AlertDialog.Builder (this)
					.SetPositiveButton ("Crear", new EventHandler<DialogClickEventArgs> ((object sender, DialogClickEventArgs e) =>{
						var s = sender as Dialog;
						var tb_name= s.FindViewById<EditText>(Resource.Id.dialog_tb_partitionName);
						var tb_code= s.FindViewById<EditText>(Resource.Id.dialog_tb_partitionCode);
						if(tb_code.Text.IsWholeNumber() && tb_name.Text.Length>0){
							dal = new DAL();
							var p =new Partition{Name=tb_name.Text,Code=tb_code.Text,Activated=false};
							int res =dal.NewPartition(p);
							if(res>0)
								adapter.AddPartition(dal.GetPartitionByName(p.Name));
						}
				}))
				.SetTitle ("Nueva Particion");
				dialog.SetView (view);
				dialog.Show ();

				break;
			}
			return true;
		}

		//protected override void OnListItemClick (ListView l, View v, int position, long id)
		//{
		//	base.OnListItemClick (l, v, position, id);
		//	Intent i = new Intent(this,typeof(ZonesListActivity));
		//	i.PutExtra("PartitionId",adapter[position].Id);
		//	StartActivity(i);
		//}
		protected override void OnResume ()
		{
			base.OnResume ();
		}
	}
}

