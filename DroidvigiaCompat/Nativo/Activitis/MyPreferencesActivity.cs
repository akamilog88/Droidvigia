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
using Android.Support.V4.Content;
using Android.Telephony;


using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;

namespace DroidvigiaCompat
{
	[Activity (Label = "PreferencesActivity")]			
	public class MyPreferencesActivity : AppCompatActivity
    {
		const string OWNER_CONTACT_NUMBER="54466823";

		public PreferencesModel prefmodel{ get; private set;}
		private AutoCompleteTextView tb_numbers;

		private CheckBox chkb_LLamar;
		private CheckBox chkb_Message;
		private CheckBox chkb_MMS;
		private CheckBox chkb_dosilence;


		private EditText tb_serial_number;


		private AplicationLicenseStatus license_status; 

		ToneDetectedReciver tone_detected_reciver;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Preferences);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
                SupportActionBar.SetDisplayShowTitleEnabled(false);
            }

            toolbar.NavigationClick += (o, e) => {
                OnBackPressed();
            };
            // Create your application here
        }

		protected override void OnResume ()
		{
			base.OnResume ();

			CreatePrefModel ();
					   
			tb_serial_number = FindViewById<EditText> (Resource.Id.tb_serial);
			tb_serial_number.Text = prefmodel.Serial;

			tb_numbers = FindViewById<AutoCompleteTextView> (Resource.Id.autoCompleteTextView1);
			tb_numbers.Text = prefmodel.numbers;

			chkb_dosilence = FindViewById<CheckBox> (Resource.Id.ckb_silencioso);
			chkb_dosilence.Checked = prefmodel.DoSilence;

			chkb_LLamar = FindViewById<CheckBox> (Resource.Id.ckb_LLamar);
			chkb_LLamar.Checked = prefmodel.DoCall;

			chkb_Message = FindViewById<CheckBox> (Resource.Id.ckb_Mensaje);
			chkb_Message.Checked = prefmodel.DoMessage;

			chkb_MMS = FindViewById<CheckBox> (Resource.Id.ckb_MMS);
			chkb_MMS.Checked = prefmodel.DoMMS;

			var sl_time_to_out = FindViewById<SeekBar> (Resource.Id.sl_time_to_go);
			sl_time_to_out.Progress= prefmodel.Time_To_GO;

			var sl_time_to_in = FindViewById<SeekBar> (Resource.Id.sl_time_to_in);
			sl_time_to_in.Progress= prefmodel.Time_To_IN;

			Button bt_solitarActivacion = FindViewById<Button> (Resource.Id.bt_solitarActivacion);
			bt_solitarActivacion.Click += OnGenerateRequest_Click;

			Button bt_enviarSolicitud = FindViewById<Button> (Resource.Id.bt_enviarSolicitud);
			bt_enviarSolicitud.Click += OnSendRequest_Click;
		
			Button btProbar = FindViewById<Button>(Resource.Id.btProbar);
			btProbar.Click += Probar_OnClick;

			Button bt_savePin = FindViewById<Button> (Resource.Id.bt_savePin);
			bt_savePin.Click += SavePin_OnClick;

			var chckb_teclado_enabled= FindViewById<CheckBox> (Resource.Id.chck_teclado_enabled);
			chckb_teclado_enabled.Checked = prefmodel.Teclado_Enabled;
			chckb_teclado_enabled.CheckedChange += OnCheckedChanged;

			var tb_appStatus = FindViewById<TextView> (Resource.Id.tb_appStatus);
			tb_appStatus.Text=(!LicenseUtility.IsValidSerial (prefmodel.Serial, prefmodel.GetInfoForRequest (this)) ? GetString( Resource.String.tb_appStatus) + ": No Registrada." : GetString( Resource.String.tb_appStatus) + ": Registrada.");
		}

		private void OnCheckedChanged(object sender, CompoundButton.CheckedChangeEventArgs e){
			if (e.IsChecked)
				prefmodel.Teclado_Enabled = true;
			else
				prefmodel.Teclado_Enabled = false;
		}
		protected override void OnPause ()
		{
			base.OnPause ();
			Button btProbar = FindViewById<Button>(Resource.Id.btProbar);
			btProbar.Click -= Probar_OnClick;
			Button bt_savePin = FindViewById<Button> (Resource.Id.bt_savePin);
			bt_savePin.Click -= SavePin_OnClick;

			Button bt_solitarActivacion = FindViewById<Button> (Resource.Id.bt_solitarActivacion);
			bt_solitarActivacion.Click -= OnGenerateRequest_Click;

			Button bt_enviarSolicitud = FindViewById<Button> (Resource.Id.bt_enviarSolicitud);
			bt_enviarSolicitud.Click -= OnSendRequest_Click;

			var sl_time_to_out = FindViewById<SeekBar> (Resource.Id.sl_time_to_go);
			prefmodel.Time_To_GO= sl_time_to_out.Progress;

			var sl_time_to_in = FindViewById<SeekBar> (Resource.Id.sl_time_to_in);
			prefmodel.Time_To_IN= sl_time_to_in.Progress;

			var tb_serial = FindViewById<EditText> (Resource.Id.tb_serial);
			prefmodel.Serial = tb_serial.Text;
			var chckb_teclado_enabled = FindViewById<CheckBox> (Resource.Id.chck_teclado_enabled);
			chckb_teclado_enabled.CheckedChange -= OnCheckedChanged;
		}
		void OnGenerateRequest_Click(object sender, EventArgs e){
			ShowDialog (1);
		}
		void OnSendRequest_Click(object sender, EventArgs e){
			SmsManager sms_mngr = SmsManager.Default;
			string request = prefmodel.GetInfoForRequest (this,false);
			if(request.Length >0){
				sms_mngr.SendTextMessage (OWNER_CONTACT_NUMBER, null, request, null, null);
			}
		}

		protected override Dialog OnCreateDialog (int id)
		{
			base.OnCreateDialog (id);		
			Dialog d = new Dialog (this);
			d.SetTitle ("Datos de cliente");
			d.SetContentView (Resource.Layout.UserInfoDialog);
			d.OwnerActivity = this;
			d.SetCanceledOnTouchOutside (true);
			var bt_aceptar = d.FindViewById<Button> (Resource.Id.dialog_bt_aceptar);
			bt_aceptar.Click += (object sender, EventArgs e) => {
				var tb_clienteName = d.FindViewById<EditText> (Resource.Id.dialog_tb_clientName);
				var rg = d.FindViewById<RadioGroup> (Resource.Id.dialog_rg_tipCliente);
				var rb_checked = d.FindViewById<RadioButton> (rg.CheckedRadioButtonId);
				if (tb_clienteName.Text != "") {
					SmsManager sms_mngr = SmsManager.Default;
					prefmodel.Client_Name = tb_clienteName.Text;
					prefmodel.Client_Type = rb_checked.Text;
					string request = prefmodel.GetInfoForRequest (this, true);
					tb_serial_number.Text = request;
					//sms_mngr.SendTextMessage (OWNER_CONTACT_NUMBER, null, GetInfoForRequest(), null, null);
					d.Dismiss ();
				}
			};				
			return d;
		}

		void Probar_OnClick(object sender, EventArgs e){
			prefmodel.SetNumberString( tb_numbers.Text);
			prefmodel.DoSilence = this.chkb_dosilence.Checked;
			prefmodel.DoMMS = this.chkb_MMS.Checked;
			prefmodel.DoMessage = this.chkb_Message.Checked;
			prefmodel.DoCall = this.chkb_LLamar.Checked;
			if(!prefmodel.IsValidNumbersString()){
				Toast.MakeText(this,"Secuencia de Numeros Incorrecta",ToastLength.Long);
			}else{
				Toast.MakeText(this,"Preferencias Guardadas",ToastLength.Long);

				tone_detected_reciver = new ToneDetectedReciver();
				LocalBroadcastManager bc_mngr=	LocalBroadcastManager.GetInstance (this);
				bc_mngr.RegisterReceiver(tone_detected_reciver,new IntentFilter("DroidVigia.DroidVigia.DroidVigia.ToneDetected"));
				var intent = new Intent("DroidVigia.DroidVigia.DroidVigia.ToneDetected");
				intent.PutExtra("message","Prueba de Envio");
				bc_mngr.SendBroadcastSync(intent);
				bc_mngr.UnregisterReceiver(tone_detected_reciver);
				tone_detected_reciver = null;
			}
		}

		void SavePin_OnClick(object sender, EventArgs e){
			var edt_pin = FindViewById<EditText> (Resource.Id.tb_actPass);
			prefmodel.Pin=edt_pin.Text;

		}

		void CreatePrefModel(){
			prefmodel = Global.GetAppPreferences (this);
		}
	}
}

