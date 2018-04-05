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
using DroidVIGIA.Utils;
using Android.Support.V4.Content;
using Android.Support.V4.App;

using Android.Telephony;
using Android.Support.V7.App;

namespace DroidvigiaBasicoCompat.Fragments
{
    public class Fragment2 : Android.Support.V4.App.Fragment
    {
        const string OWNER_CONTACT_NUMBER = "54466823";

        public PreferencesModel prefmodel { get; private set; }
        private AutoCompleteTextView tb_numbers;

        private CheckBox chkb_LLamar;
        private CheckBox chkb_Message;
        private CheckBox chkb_MMS;
        private CheckBox chkb_dosilence;


        private EditText tb_serial_number;


        private AplicationLicenseStatus license_status;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }


        public static Fragment2 NewInstance()
        {
            var frag2 = new Fragment2 { Arguments = new Bundle() };
            return frag2;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);

            return inflater.Inflate(Resource.Layout.fragment2, null) as ViewGroup;            
        }

        public override void OnResume()
        {
            base.OnResume();

            var prefmodel = Global.GetAppPreferences(this.Activity);

            tb_serial_number =  this.Activity.FindViewById<EditText>(Resource.Id.tb_serial);
            tb_serial_number.Text = prefmodel.Serial;

            tb_numbers =  this.Activity.FindViewById<AutoCompleteTextView>(Resource.Id.autoCompleteTextView1);
            tb_numbers.Text = prefmodel.numbers;

            chkb_dosilence =  this.Activity.FindViewById<CheckBox>(Resource.Id.ckb_silencioso);
            chkb_dosilence.Checked = prefmodel.DoSilence;

            chkb_LLamar =  this.Activity.FindViewById<CheckBox>(Resource.Id.ckb_LLamar);
            chkb_LLamar.Checked = prefmodel.DoCall;

            chkb_Message =  this.Activity.FindViewById<CheckBox>(Resource.Id.ckb_Mensaje);
            chkb_Message.Checked = prefmodel.DoMessage;

            chkb_MMS =  this.Activity.FindViewById<CheckBox>(Resource.Id.ckb_MMS);
            chkb_MMS.Checked = prefmodel.DoMMS;

            var sl_time_to_out =  this.Activity.FindViewById<SeekBar>(Resource.Id.sl_time_to_go);
            sl_time_to_out.Progress = prefmodel.Time_To_GO;

            var sl_time_to_in =  this.Activity.FindViewById<SeekBar>(Resource.Id.sl_time_to_in);
            sl_time_to_in.Progress = prefmodel.Time_To_IN;

            Button bt_solitarActivacion =  this.Activity.FindViewById<Button>(Resource.Id.bt_solitarActivacion);
            bt_solitarActivacion.Click += OnGenerateRequest_Click;

            Button bt_enviarSolicitud =  this.Activity.FindViewById<Button>(Resource.Id.bt_enviarSolicitud);
            bt_enviarSolicitud.Click += OnSendRequest_Click;

            Button btProbar =  this.Activity.FindViewById<Button>(Resource.Id.btProbar);
            btProbar.Click += Probar_OnClick;

            Button bt_savePin =  this.Activity.FindViewById<Button>(Resource.Id.bt_savePin);
            bt_savePin.Click += SavePin_OnClick;

            var chckb_teclado_enabled =  this.Activity.FindViewById<CheckBox>(Resource.Id.chck_teclado_enabled);
            chckb_teclado_enabled.Checked = prefmodel.Teclado_Enabled;
            chckb_teclado_enabled.CheckedChange += OnCheckedChanged;

            var tb_appStatus =  this.Activity.FindViewById<TextView>(Resource.Id.tb_appStatus);
            tb_appStatus.Text = (!LicenseUtility.IsValidSerial(prefmodel.Serial, prefmodel.GetInfoForRequest(this.Context)) ? GetString(Resource.String.tb_appStatus) + ": No Registrada." : GetString(Resource.String.tb_appStatus) + ": Registrada.");

        }

        //public override void on

        private void OnCheckedChanged(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var prefmodel = Global.GetAppPreferences(this.Activity);
            if (e.IsChecked)
                prefmodel.Teclado_Enabled = true;
            else
                prefmodel.Teclado_Enabled = false;
        }
       
        public override void OnPause()
        {
            base.OnPause();

            var prefmodel = Global.GetAppPreferences(this.Activity);

            Button btProbar =  this.View.FindViewById<Button>(Resource.Id.btProbar);
            btProbar.Click -= Probar_OnClick;
            Button bt_savePin =  this.View.FindViewById<Button>(Resource.Id.bt_savePin);
            bt_savePin.Click -= SavePin_OnClick;

            Button bt_solitarActivacion =  this.View.FindViewById<Button>(Resource.Id.bt_solitarActivacion);
            bt_solitarActivacion.Click -= OnGenerateRequest_Click;

            Button bt_enviarSolicitud =  this.View.FindViewById<Button>(Resource.Id.bt_enviarSolicitud);
            bt_enviarSolicitud.Click -= OnSendRequest_Click;

            var sl_time_to_out =  this.View.FindViewById<SeekBar>(Resource.Id.sl_time_to_go);
            prefmodel.Time_To_GO = sl_time_to_out.Progress;

            var sl_time_to_in =  this.View.FindViewById<SeekBar>(Resource.Id.sl_time_to_in);
            prefmodel.Time_To_IN = sl_time_to_in.Progress;

            var tb_serial =  this.View.FindViewById<EditText>(Resource.Id.tb_serial);
            prefmodel.Serial = tb_serial.Text;
            var chckb_teclado_enabled =  this.View.FindViewById<CheckBox>(Resource.Id.chck_teclado_enabled);
            chckb_teclado_enabled.CheckedChange -= OnCheckedChanged;
            base.OnDetach();
        }

        void OnGenerateRequest_Click(object sender, EventArgs e)
        {
            var prefmodel = Global.GetAppPreferences(this.Activity);

            var tb_serial =  this.Activity.FindViewById<EditText>(Resource.Id.tb_serial);
            prefmodel.Serial = tb_serial.Text;
            
            var f =new MyFragmentDialog();            
            f.Show((this.Activity as FragmentActivity).SupportFragmentManager , "DatosCliente");
            f.OnGenerateComplete += delegate {
                var local_prefmodel = Global.GetAppPreferences(this.Activity);
                tb_serial.Text = local_prefmodel.GetInfoForRequest(this.Activity, true);
            };
        }
        void OnSendRequest_Click(object sender, EventArgs e)
        {
            var prefmodel = Global.GetAppPreferences(this.Activity);

            SmsManager sms_mngr = SmsManager.Default;
            string request = prefmodel.GetInfoForRequest(this.Context, false);
            if (request.Length > 0)
            {
                sms_mngr.SendTextMessage(OWNER_CONTACT_NUMBER, null, request, null, null);
            }
        }      

        void Probar_OnClick(object sender, EventArgs e)
        {
            var prefmodel = Global.GetAppPreferences(this.Activity);

            prefmodel.SetNumberString(tb_numbers.Text);
            prefmodel.DoSilence = this.chkb_dosilence.Checked;
            prefmodel.DoMMS = this.chkb_MMS.Checked;
            prefmodel.DoMessage = this.chkb_Message.Checked;
            prefmodel.DoCall = this.chkb_LLamar.Checked;
            if (!prefmodel.IsValidNumbersString())
            {
                Toast.MakeText(this.Context, "Secuencia de Numeros Incorrecta", ToastLength.Long);
            }
            else
            {
                Toast.MakeText(this.Context, "Preferencias Guardadas", ToastLength.Long);
                Intent i = new Intent("DroidVIGIA.DroidVIGIA.DroidVigiaBasic.SendAlarmBroadcastReciver");
                this.Activity.SendBroadcast(i);
            }
        }

        void SavePin_OnClick(object sender, EventArgs e)
        {
            var prefmodel = Global.GetAppPreferences(this.Activity);

            var edt_pin =  this.View.FindViewById<EditText>(Resource.Id.tb_actPass);
            prefmodel.Pin = edt_pin.Text;
        }        
    }

    public class MyFragmentDialog : Android.Support.V4.App.DialogFragment
    {
        public event EventHandler OnGenerateComplete;

        public MyFragmentDialog()
        {
            this.ShowsDialog = true;
        }
        

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            Dialog d = new Dialog(this.Activity);
            d.SetTitle("Datos de cliente");
            d.SetContentView(Resource.Layout.UserInfoDialog);
            d.OwnerActivity = this.Activity;
            d.SetCanceledOnTouchOutside(true);
            var bt_aceptar = d.FindViewById<Button>(Resource.Id.dialog_bt_aceptar);
            bt_aceptar.Click += (object sender, EventArgs e) => {
                var tb_clienteName = d.FindViewById<EditText>(Resource.Id.dialog_tb_clientName);
                var rg = d.FindViewById<RadioGroup>(Resource.Id.dialog_rg_tipCliente);
                var rb_checked = d.FindViewById<RadioButton>(rg.CheckedRadioButtonId);
                if (tb_clienteName.Text != "")
                {
                    //SmsManager sms_mngr = SmsManager.Default;
                    var prefmodel = Global.GetAppPreferences(this.Activity);
                    prefmodel.Client_Name = tb_clienteName.Text;
                    prefmodel.Client_Type = rb_checked.Text;
                    //string request = prefmodel.GetInfoForRequest(this, true);                    
                    //tb_serial_number.Text = request;
                    //sms_mngr.SendTextMessage (OWNER_CONTACT_NUMBER, null, GetInfoForRequest(), null, null);
                    OnGenerateComplete.Invoke(this,null);
                    d.Dismiss();
                }
            };
            return d;
            //return base.OnCreateDialog(savedInstanceState);
        }
    }
}