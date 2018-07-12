using System;
using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using DroidToneDecoder;

namespace DroidvigiaCompat
{
    [Activity(Label = "@string/app_name", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, Icon = "@drawable/vigia_icon3")]
    public class MainActivity : AppCompatActivity,ISharedPreferencesOnSharedPreferenceChangeListener,IDetectorServiceBinderClient
	{
		private PreferencesModel prefs_model;
		public bool isBound{ get; set;}
		public DetectorServiceBinder Binder{ get; set;}
		private DetectorServiceConnection connector;
		ListView lv_history;
		HistoryGridAdapter history_adapter;

		DrawerLayout drawerLayout;
        	NavigationView navigationView;

        	IMenuItem previousItem;


		public MainActivity ()
		{
			isBound = false;
			Binder = null;
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
            SetContentView(Resource.Layout.main_compat);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(false);
                SupportActionBar.SetDisplayShowTitleEnabled(false);
            }

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            
            //Set hamburger items menu
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            toolbar.NavigationClick += (o, args)=>{
                drawerLayout.OpenDrawer(GravityCompat.Start);
            };

            //setup navigation view
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

            //handle navigation
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                if (previousItem != null)
                    previousItem.SetChecked(false);

                navigationView.SetCheckedItem(e.MenuItem.ItemId);

                previousItem = e.MenuItem;

                switch (e.MenuItem.ItemId)
                {
                    
                    case Resource.Id.nav_config:
                        ListItemClicked(0);
                        break;
                    case Resource.Id.nav_zones:
                        ListItemClicked(1);
                        break;
                    case Resource.Id.nav_planif:
                        ListItemClicked(2);
                        break;
                    case Resource.Id.nav_salir:
                        ListItemClicked(3);
                        break;
                }


                drawerLayout.CloseDrawers();
            };


            //if first time you will want to go ahead and click first item.
            /*if (bundle == null)
            {
                navigationView.SetCheckedItem(Resource.Id.nav_home);
                ListItemClicked(0);
            }*/

			Intent i = new Intent (this, typeof(DetectorService));
			StartService(i);
		}


        int oldPosition = -1;
        private void ListItemClicked(int position)
        {
            //this way we don't load twice, but you might want to modify this a bit.
            if (position == oldPosition)
                return;

            oldPosition = position;

            Intent i;

            bool isExiting = false;
            switch (position)
            {
                case 0:
                    i = new Intent(this, typeof(MyPreferencesActivity));
                    StartActivity(i);
                    break;
                case 1:
                    i = new Intent(this, typeof(PartitionManagmentActivity));
                    StartActivity(i);
                    break;
                case 2:
                    i = new Intent(this, typeof(SchudlerManagerActivity));
                    StartActivity(i);
                    break;
                case 3:
                    i = new Intent(this, typeof(HistoryManagmentActivity));
                    StartActivity(i);
                    break;
                case 4:
                    var service = Binder.GetService();
                    service.ForceStop();
                    Finish();
                    isExiting = true;
                    break;
            }
            
            /*if(!isExiting)
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content_frame, fragment)
                    .Commit();*/
        }

	
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			IMenuItem item1= menu.Add (0,0,0,"Configuracion");
			IMenuItem item2= menu.Add (0,1,1,"Zonas");
			IMenuItem item3= menu.Add (0,2,2,"Planificacion");
            IMenuItem item4 = menu.Add(0, 3, 3, "Historial");
            IMenuItem item5 = menu.Add(0,4, 4, "Salir");
            return base.OnCreateOptionsMenu (menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			Intent i;
			switch (item.ItemId) {
			    case 0:
                    ListItemClicked(0);
				    break;
			    case 1:
                    ListItemClicked(1);
                    break;
			    case 2:
                     ListItemClicked(2);
                     break;
                case 3:
                     ListItemClicked(3);
                     break;
                case 4:
                    ListItemClicked(4);
                    break;
            }
			return base.OnOptionsItemSelected (item);
		}	
		 
		protected override void OnStart(){
			base.OnStart ();
		}
		protected override void OnResume ()
		{
			base.OnResume ();
			connector = new DetectorServiceConnection (this);
			ToggleButton bt = FindViewById<ToggleButton> (Resource.Id.toggleButton1);

			BindService (new Intent(this.BaseContext,typeof(DetectorService)),connector, Bind.None);
			connector.BindComplete+= OnBindComplete;
			//bt.Checked = Global.GetAppPreferences(this).Ready;

			DAL dal = new DAL();
			var partitions = dal.GetAllPartitions();
			if (partitions.Find ((part)=>part.Activated) == null)
				bt.Checked = false;
			else
				bt.Checked = true;

			bt.CheckedChange += OnCheckedChange;
				//service.ToneDetected += OnToneDetected;

			lv_history = FindViewById<ListView> (Resource.Id.lv_history);
		
			history_adapter = new HistoryGridAdapter (this,dal.GetTop10Events());
			lv_history.Adapter = history_adapter;

			lv_history.ItemClick+= HistoryItemClicked;
			//bt.CheckedChange += OnToggleChecked;
			ISharedPreferences pref = Global.GetSharedPreferences (this);
			prefs_model = new PreferencesModel (pref);
			pref.RegisterOnSharedPreferenceChangeListener (this);
		}

		private void HistoryItemClicked(object sender, AdapterView.ItemClickEventArgs e){
			var dialog = new Android.Support.V7.App.AlertDialog.Builder (this)
				.SetTitle ("Detalles").SetMessage(history_adapter[e.Position].Detail);
			dialog.Show ();
		}

		private void OnBindComplete(object sender, EventArgs e){
			var service = Binder.GetService();
			service.NewHistoryData += OnNewHistoryData;
		}

		private void OnNewHistoryData(object s, ActionRegisteredEventArgs args){
			UpdateHistory (args.NewData);
		}
		void UpdateHistory(HistoryItem item){
			RunOnUiThread(()=>{
				history_adapter.AddHistoryData(item);
			});
		}
		private void OnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e){
			var dal = new DAL ();
			if (e.IsChecked) {					

				dal.ActivateAllPartitions ();	
				var h = new HistoryItem {
					Time=DateTime.Now,
					State=Action.ACTION_ID_ACTIVATED,
					PartitionName= "TODAS",
					Detail="TODAS LAS PARTICIONES ACTIVADAS"
				};				
				dal.RegiterEvent (h);
				UpdateHistory (h);
				prefs_model.Ready =true;
			} else {

				dal.DesactivateAllPartitions ();
				var h = new HistoryItem {
					Time=DateTime.Now,
					State=Action.ACTION_ID_DESACTIVATED,
					PartitionName= "TODAS",
					Detail="TODAS LAS PARTICIONES DESACTIVADAS"
				};
				dal.RegiterEvent(h);
				UpdateHistory (h);
				prefs_model.Ready = false;
			}
		}
		protected override void OnPause(){
			base.OnPause ();
			ToggleButton bt = FindViewById<ToggleButton> (Resource.Id.toggleButton1);
			//var service = Binder.GetService();
			//service.ToneDetected -= OnToneDetected;
			//bt.CheckedChange -= OnToggleChecked;]
			lv_history.ItemClick-= HistoryItemClicked;
			try{
			var service = Binder.GetService();
			service.NewHistoryData -= OnNewHistoryData;
			}catch(Exception){
			}
			ISharedPreferences pref =  Global.GetSharedPreferences (this);
			pref.UnregisterOnSharedPreferenceChangeListener (this);
			bt.CheckedChange -= OnCheckedChange;
			UnbindService (connector);
		}
		/*void OnToggleChecked( object sender, CompoundButton.CheckedChangeEventArgs e){
			if (e.IsChecked && isBound) {
				var service = Binder.GetService();
				if(!service.Ready)
					service.Ready = true;
			} else if (isBound){
				var service = Binder.GetService();
				service.Ready = false;
			}
		}*/
		void OnToneDetected(object o,DTMFDetectedArgs args){
			/*RunOnUiThread(()=>{
				var textview= FindViewById<TextView>(Resource.Id.editText1);
				if(args.TonConstant != DTMFConstanst.DSIL)
					textview.Text +=args.ToneAscii;
			});*/
		}
		public void OnSharedPreferenceChanged (ISharedPreferences sharedPreferences, string key)
		{
			if(key=="ready"){
				var bt = FindViewById<ToggleButton>(Resource.Id.toggleButton1);
				bt.Checked = sharedPreferences.GetBoolean ("ready",false);
			}			 
		}	
	}
}


