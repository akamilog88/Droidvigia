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
	[Activity (Label = "NewSchudle")]			
	public class SchudleDataActivity : AppCompatActivity
	{
		public const string SCHUDLE_ID_EXTRA="label";
		private string schudle_label="";
		private Schudle s;
		private List<String> Partitions;
		private string[] days = StringExtensions.DaysOfWeak;
		private bool[] selected_partitions;
		private bool[] selected_days;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.NewSchudleLayout);
			schudle_label = this.Intent.GetStringExtra (SCHUDLE_ID_EXTRA);

			var tp_time = FindViewById<TimePicker> (Resource.Id.tp_shudle_time);
			tp_time.SetIs24HourView (Java.Lang.Boolean.True);

			DAL dal = new DAL();		
			if (schudle_label != "" && schudle_label != null) {
				var bt_aceptar = FindViewById<Button> (Resource.Id.bt_NewSchudle);
				bt_aceptar.Enabled = false;
				s = dal.GetSchudleByName (schudle_label);
				Partitions = s.PartitionNames.Split (new string[]{";"}, StringSplitOptions.RemoveEmptyEntries).ToList();
				selected_partitions = new bool[Partitions.Count()] ;
				var list_particiones =dal.GetAllPartitions ();
				list_particiones.ForEach(p=>{
					if(s.PartitionNames.Contains(p.Name))
						selected_partitions[Partitions.IndexOf(p.Name)]=true;
				});
				selected_days = new bool[days.Count()] ;
				days.ToList().ForEach(d=>{
					if(s.Days.Contains(d))
						selected_days[days.ToList().IndexOf(d)]=true;
				});
				var rb_armar_alarma = FindViewById<RadioButton> (Resource.Id.rb_AlarmReady);
				var rb_desarmar_alarma = FindViewById<RadioButton> (Resource.Id.rb_AlarmNotReady);

				if (s.ActionId == Action.ACTION_ID_ACTIVATED)
					rb_armar_alarma.Checked = true;
				else
					rb_desarmar_alarma.Checked = true;

				tp_time.CurrentHour = new Java.Lang.Integer (s.Hour);
				tp_time.CurrentMinute = new Java.Lang.Integer (s.Minute);

				var tb_Etiqueta = FindViewById<EditText> (Resource.Id.tb_Schudle_Label);
				tb_Etiqueta.Text = s.Label;

			} else {
				Partitions =dal.GetAllPartitions ().Select(p=>p.Name).ToList();
				selected_partitions = Partitions.Select (p=>false).ToArray();
				selected_days = days.Select (p=>false).ToArray();
				tp_time.CurrentHour = new Java.Lang.Integer (0);
				tp_time.CurrentMinute = new Java.Lang.Integer (0);
			}	

			// Create your application here
		}
		protected override void OnResume ()
		{
			base.OnResume ();
			var bt_aceptar = FindViewById<Button> (Resource.Id.bt_NewSchudle);
			bt_aceptar.Click += OnAceptar;
			var bt_pick_particiones = FindViewById<Button> (Resource.Id.bt_Schudle_Particiones);
			bt_pick_particiones.Click += OnPickParticionesClick;
			var bt_pick_days = FindViewById<Button> (Resource.Id.bt_schudle_Dias);
			bt_pick_days.Click += OnPickDays;

		}
		private void OnPickParticionesClick (object sender, EventArgs e){	
			var dialog = new Android.Support.V7.App.AlertDialog.Builder (this)
			.SetTitle ("Particiones")
					.SetMultiChoiceItems (Partitions.ToArray(), selected_partitions, new EventHandler<DialogMultiChoiceClickEventArgs> ((object o, DialogMultiChoiceClickEventArgs args) => {
						if(args.IsChecked)
							selected_partitions[args.Which]=true;
						else
							selected_partitions[args.Which]=false;
					}));
			dialog.Show ();
		}
		private void OnPickDays (object sender, EventArgs e){
			var dialog = new Android.Support.V7.App.AlertDialog.Builder (this)
				.SetTitle ("Repetir Dia")
					.SetMultiChoiceItems (days, selected_days, new EventHandler<DialogMultiChoiceClickEventArgs> ((object o, DialogMultiChoiceClickEventArgs args) => {
						if(args.IsChecked)
							selected_days[args.Which]=true;
						else
							selected_days[args.Which]=false;
					}));
			dialog.Show ();
		}

		private void OnAceptar (object sender, EventArgs e){
			s = new Schudle ();
			var tb_Etiqueta = FindViewById<EditText> (Resource.Id.tb_Schudle_Label);
			var tp_time = FindViewById<TimePicker> (Resource.Id.tp_shudle_time);
			var rb_armar_alarma = FindViewById<RadioButton> (Resource.Id.rb_AlarmReady);
			s.Label = tb_Etiqueta.Text;
			s.PartitionNames = PartitionsStringFromList ();
			s.Days = DaysStringFromList ();
			s.Hour = (int)tp_time.CurrentHour;
			s.Minute = (int)tp_time.CurrentMinute;
			s.ActionId = rb_armar_alarma.Checked ? Action.ACTION_ID_ACTIVATED : Action.ACTION_ID_DESACTIVATED;
			if(s.Label!=""&&s.Days!=""&&s.PartitionNames!=""){
				DAL dal = new DAL ();
				dal.NewSchudle (s);
				Toast.MakeText (this, "Tarea Creada", ToastLength.Short);
                //this.MoveTaskToBack (true);
                this.OnBackPressed();
			}
		}	
		String PartitionsStringFromList(){
			string result="";
			for (int i =0; i<Partitions.Count(); i++) {
				if(selected_partitions[i]){
					result += Partitions [i]+";";
				}
			}
			return result;
		}
		String DaysStringFromList(){
			string result="";
			for (int i =0; i<days.Count(); i++) {
				if(selected_days[i]){
					result += days [i]+";";
				}
			}
			return result;
		}
	}
}

