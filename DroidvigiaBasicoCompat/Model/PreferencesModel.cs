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
using Android.Telephony;
using DroidVIGIA.Utils;

namespace DroidvigiaBasicoCompat
{
    public class PreferencesModel
    {
        public const String KEY_NAME_READY = "ready";
        public const String KEY_NAME_TECLADO_ENABLED = "teclado_enabled";
        public const String KEY_NAME_FIRED = "fired";
        public const String KEY_NAME_NUMBERS = "numbers";
        public const String KEY_NAME_DOCALL = "do_call";
        public const String KEY_NAME_DOMESSAGE = "do_message";
        public const String KEY_NAME_DOMMS = "do_mms";
        public const String KEY_NAME_DOSILENCE = "do_silence";
        public const String KEY_NAME_PIN = "pin";
        public const String KEY_NAME_SERIAL = "serial";
        public const String KEY_NAME_SIM_NUMBER = "sim_number";
        public const String KEY_NAME_CLIENTNAME = "client_name";
        public const String KEY_NAME_CLIENTTYPE = "client_type";
        public const String KEY_NAME_REQUESTDATE = "request_date";
        public const String KEY_NAME_NOTIFICANDO = "notificando";
        public const String KEY_NAME_CALL_PHONE_INDEX = "call_phone_index";
        public const String KEY_NAME_LAST_CALL_STATE = "last_call_state";
        public const String KEY_NAME_SCHUDLED_CALL = "schudled_call";
        public const String KEY_NAME_SCHUDLED_CALL_NUMBER = "schudled_call_number";
        public const String KEY_NAME_TIME_TO_GO = "time_to_go";
        public const String KEY_NAME_TIME_TO_IN = "time_to_in";
        public const String KEY_NAME_NEXT_SCHUDLE_NAME = "next_schudle_name";
        public const String KEY_NAME_HAS_AUDIO_FOCUS = "has_audio_focus";

        public String numbers;

        public event EventHandler<EventArgs> PreferencesValueChanged;
        ISharedPreferences shared_prefs;
        ISharedPreferencesEditor editor;
        bool loaded = false;

        public bool SetNumberString(String numstring)
        {
            string orig_val = numbers;
            numbers = numstring;
            if (IsValidNumbersString())
            {
                editor = shared_prefs.Edit();
                editor.PutString(KEY_NAME_NUMBERS, numbers);
                editor.Commit();
                if (loaded && PreferencesValueChanged != null)
                    PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_NUMBERS));
                return true;
            }
            numbers = orig_val;
            return false;
        }

        public List<String> notificationNumbers
        {
            get
            {
                List<String> result = numbers.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                return result;
            }
        }

        private int time_to_go;
        public int Time_To_GO
        {
            get { return time_to_go; }
            set
            {
                if (time_to_go != value)
                {
                    time_to_go = value;

                    editor = shared_prefs.Edit();
                    editor.PutInt(KEY_NAME_TIME_TO_GO, value);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_TIME_TO_GO));
                }
            }
        }
        private int time_to_in;
        public int Time_To_IN
        {
            get { return time_to_in; }
            set
            {
                if (time_to_in != value)
                {
                    time_to_in = value;

                    editor = shared_prefs.Edit();
                    editor.PutInt(KEY_NAME_TIME_TO_IN, value);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_TIME_TO_IN));
                }
            }
        }

        private String pin = "0000";
        public String Pin
        {
            get { return pin; }
            set
            {
                if (pin != value && value != "")
                {
                    pin = value;

                    editor = shared_prefs.Edit();
                    editor.PutString(KEY_NAME_PIN, value);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_PIN));
                }
            }
        }
        private String sim_number = "";
        public String Sim_Number
        {
            get { return sim_number; }
            set
            {
                if (sim_number != value && value != "")
                {
                    sim_number = value;

                    editor = shared_prefs.Edit();
                    editor.PutString(KEY_NAME_SIM_NUMBER, value);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_SIM_NUMBER));
                }
            }
        }
        private bool schudled_call = false;
        public bool Schudled_Call
        {
            get { return schudled_call; }
            set
            {
                if (schudled_call != value)
                {
                    schudled_call = value;

                    editor = shared_prefs.Edit();
                    editor.PutBoolean(KEY_NAME_SCHUDLED_CALL, value);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_SCHUDLED_CALL));
                }
            }
        }
        private bool has_aucio_focus = false;
        public bool Has_Audio_Focus
        {
            get { return has_aucio_focus; }
            set
            {
                if (has_aucio_focus != value)
                {
                    has_aucio_focus = value;

                    editor = shared_prefs.Edit();
                    editor.PutBoolean(KEY_NAME_HAS_AUDIO_FOCUS, value);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_HAS_AUDIO_FOCUS));
                }
            }
        }
        private String schudled_call_number = "";
        public String Schudled_Call_Number
        {
            get { return schudled_call_number; }
            set
            {
                if (schudled_call_number != value && value != "")
                {
                    schudled_call_number = value;

                    editor = shared_prefs.Edit();
                    editor.PutString(KEY_NAME_SCHUDLED_CALL_NUMBER, value);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_SCHUDLED_CALL_NUMBER));
                }
            }
        }


        private String next_schudle_name = "";
        public String Next_Schudle_Name
        {
            get { return next_schudle_name; }
            set
            {
                if (next_schudle_name != value && value != "")
                {
                    next_schudle_name = value;

                    editor = shared_prefs.Edit();
                    editor.PutString(KEY_NAME_NEXT_SCHUDLE_NAME, value);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_NEXT_SCHUDLE_NAME));
                }
            }
        }
        private int call_phone_index = 0;
        public int Call_Phone_Index
        {
            get { return call_phone_index; }
            set
            {
                if (call_phone_index != value)
                {
                    call_phone_index = value;

                    editor = shared_prefs.Edit();
                    editor.PutInt(KEY_NAME_CALL_PHONE_INDEX, value);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_CALL_PHONE_INDEX));
                }
            }
        }
        private bool do_call = true;
        public bool DoCall
        {
            get { return do_call; }
            set
            {
                if (do_call != value)
                {
                    do_call = value;

                    editor = shared_prefs.Edit();
                    editor.PutBoolean(KEY_NAME_DOCALL, value);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_DOCALL));
                }
            }
        }
        private bool teclado_enabled = true;
        public bool Teclado_Enabled
        {
            get { return teclado_enabled; }
            set
            {
                if (teclado_enabled != value)
                {
                    teclado_enabled = value;

                    editor = shared_prefs.Edit();
                    editor.PutBoolean(KEY_NAME_TECLADO_ENABLED, value);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_TECLADO_ENABLED));
                }
            }
        }
        private bool notificando = false;
        public bool Notificando
        {
            get { return notificando; }
            set
            {
                if (notificando != value)
                {
                    notificando = value;

                    editor = shared_prefs.Edit();
                    editor.PutBoolean(KEY_NAME_NOTIFICANDO, value);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_NOTIFICANDO));
                }
            }
        }
        private bool do_silence = true;
        public bool DoSilence
        {
            get { return do_silence; }
            set
            {
                if (do_silence != value)
                {
                    do_silence = value;

                    editor = shared_prefs.Edit();
                    editor.PutBoolean(KEY_NAME_DOSILENCE, value);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_DOSILENCE));
                }
            }
        }
        private bool do_message = false;
        public bool DoMessage
        {
            get { return do_message; }
            set
            {
                if (do_message != value)
                {
                    do_message = value;

                    editor = shared_prefs.Edit();
                    editor.PutBoolean(KEY_NAME_DOMESSAGE, value);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_DOMESSAGE));
                }
            }
        }
        private bool do_mms = false;
        public bool DoMMS
        {
            get { return do_mms; }
            set
            {
                if (do_mms != value)
                {
                    do_mms = value;

                    editor = shared_prefs.Edit();
                    editor.PutBoolean(KEY_NAME_DOMMS, value);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_DOMMS));
                }
            }
        }

        private string serial = "";
        public string Serial
        {
            get { return serial; }
            set
            {
                if (serial != value && value != "")
                {
                    serial = value;

                    editor = shared_prefs.Edit();
                    editor.PutString(KEY_NAME_SERIAL, value);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_SERIAL));
                }
            }
        }
        private string client_name = "";
        public string Client_Name
        {
            get { return client_name; }
            set
            {
                if (client_name != value && value != "")
                {
                    client_name = value;

                    editor = shared_prefs.Edit();
                    editor.PutString(KEY_NAME_CLIENTNAME, value);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_CLIENTNAME));
                }
            }
        }
        private CallState last_call_state;
        public CallState Last_Call_State
        {
            get { return last_call_state; }
            set
            {
                if (last_call_state != value)
                {
                    last_call_state = value;

                    editor = shared_prefs.Edit();
                    editor.PutString(KEY_NAME_LAST_CALL_STATE, CallState_ToString(value));
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_LAST_CALL_STATE));
                }
            }
        }

        private String CallState_ToString(CallState state)
        {
            switch (state)
            {
                case CallState.Idle:
                    return "IDLE";
                    break;
                case CallState.Offhook:
                    return "OFFHOOK";
                    break;
                case CallState.Ringing:
                    return "RINGING";
                    break;
            }
            return "IDLE";
        }
        private CallState CallState_FromString(String state)
        {
            switch (state)
            {
                case "IDLE":
                    return CallState.Idle;
                    break;
                case "OFFHOOK":
                    return CallState.Offhook;
                    break;
                case "RINGING":
                    return CallState.Ringing;
                    break;
            }
            return CallState.Idle;
        }
        private DateTime request_date = new DateTime();
        public DateTime RequestDate
        {
            get { return request_date; }
            set
            {
                if (request_date != value)
                {
                    request_date = value;

                    editor = shared_prefs.Edit();
                    editor.PutString(KEY_NAME_REQUESTDATE, request_date.ToShortDateString());
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_REQUESTDATE));
                }
            }
        }
        private string client_type = "";
        public string Client_Type
        {
            get { return client_type; }
            set
            {
                if (client_type != value && value != "")
                {
                    client_type = value;

                    editor = shared_prefs.Edit();
                    editor.PutString(KEY_NAME_CLIENTTYPE, value);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_CLIENTTYPE));
                }
            }
        }
        private bool fired = false;
        public bool Fired
        {
            get { return fired; }
            set
            {
                if (fired != value)
                {
                    fired = value;
                    var editor = shared_prefs.Edit();
                    editor.PutBoolean(KEY_NAME_FIRED, fired);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_FIRED));
                }
            }
        }

        private bool ready = false;
        public bool Ready
        {
            get { return ready; }
            set
            {
                if (ready != value)
                {
                    if (!value)
                        Fired = false;
                    ready = value;
                    var editor = shared_prefs.Edit();
                    editor.PutBoolean(KEY_NAME_READY, ready);
                    editor.Commit();
                    if (loaded && PreferencesValueChanged != null)
                        PreferencesValueChanged(this, new PropertyChangedEventArgs(KEY_NAME_READY));
                    /*if (ready) 
						activation_time = DateTime.Now;*/
                }
            }
        }

        public PreferencesModel(ISharedPreferences shared_prefs)
        {
            this.shared_prefs = shared_prefs;
            LoadPreferences();
            loaded = true;
        }

        public void LoadPreferences()
        {
            Pin = shared_prefs.GetString(KEY_NAME_PIN, "0000");
            DoCall = shared_prefs.GetBoolean(KEY_NAME_DOCALL, true);
            DoMessage = shared_prefs.GetBoolean(KEY_NAME_DOMESSAGE, false);
            DoMMS = shared_prefs.GetBoolean(KEY_NAME_DOMMS, false);
            DoSilence = shared_prefs.GetBoolean(KEY_NAME_DOSILENCE, false);
            Serial = shared_prefs.GetString(KEY_NAME_SERIAL, "");
            client_name = shared_prefs.GetString(KEY_NAME_CLIENTNAME, "");
            client_type = shared_prefs.GetString(KEY_NAME_CLIENTTYPE, "");
            string sdate = shared_prefs.GetString(KEY_NAME_REQUESTDATE, "");
            Fired = shared_prefs.GetBoolean(KEY_NAME_FIRED, false);
            Ready = shared_prefs.GetBoolean(KEY_NAME_READY, false);
            Teclado_Enabled = shared_prefs.GetBoolean(KEY_NAME_TECLADO_ENABLED, false);
            Notificando = shared_prefs.GetBoolean(KEY_NAME_NOTIFICANDO, false);
            Call_Phone_Index = shared_prefs.GetInt(KEY_NAME_CALL_PHONE_INDEX, 0);
            Last_Call_State = CallState_FromString(shared_prefs.GetString(KEY_NAME_LAST_CALL_STATE, "IDLE"));
            Schudled_Call = shared_prefs.GetBoolean(KEY_NAME_SCHUDLED_CALL, false);
            Has_Audio_Focus = shared_prefs.GetBoolean(KEY_NAME_HAS_AUDIO_FOCUS, false);
            Schudled_Call_Number = shared_prefs.GetString(KEY_NAME_SCHUDLED_CALL_NUMBER, "");
            Time_To_GO = shared_prefs.GetInt(KEY_NAME_TIME_TO_GO, 0);
            Time_To_IN = shared_prefs.GetInt(KEY_NAME_TIME_TO_IN, 0);
            Next_Schudle_Name = shared_prefs.GetString(KEY_NAME_NEXT_SCHUDLE_NAME, "");
            Sim_Number = shared_prefs.GetString(KEY_NAME_SIM_NUMBER, "");
            DateTime rdate;
            if (DateTime.TryParse(sdate, out rdate))
            {
                RequestDate = rdate;
            }
            SetNumberString(shared_prefs.GetString("numbers", ""));
        }
        public void ReloadProperty(string prop_name)
        {
            switch (prop_name)
            {
                case KEY_NAME_NUMBERS:
                    numbers = shared_prefs.GetString(KEY_NAME_NUMBERS, "");
                    break;
                case KEY_NAME_PIN:
                    pin = shared_prefs.GetString(KEY_NAME_PIN, "0000");
                    break;
                case KEY_NAME_DOCALL:
                    do_call = shared_prefs.GetBoolean(KEY_NAME_DOCALL, true);
                    break;
                case KEY_NAME_DOMESSAGE:
                    do_message = shared_prefs.GetBoolean(KEY_NAME_DOMESSAGE, false);
                    break;
                case KEY_NAME_DOMMS:
                    do_mms = shared_prefs.GetBoolean(KEY_NAME_DOMMS, false);
                    break;
                case KEY_NAME_DOSILENCE:
                    do_silence = shared_prefs.GetBoolean(KEY_NAME_DOSILENCE, false);
                    break;
                case KEY_NAME_SERIAL:
                    serial = shared_prefs.GetString(Serial, "");
                    break;
                case KEY_NAME_CLIENTNAME:
                    client_name = shared_prefs.GetString(KEY_NAME_CLIENTNAME, "");
                    break;
                case KEY_NAME_CLIENTTYPE:
                    client_type = shared_prefs.GetString(KEY_NAME_CLIENTTYPE, "");
                    break;
                case KEY_NAME_REQUESTDATE:
                    request_date = DateTime.Parse(shared_prefs.GetString(KEY_NAME_REQUESTDATE, ""));
                    break;
                case KEY_NAME_READY:
                    ready = shared_prefs.GetBoolean(KEY_NAME_READY, false);
                    break;
                case KEY_NAME_FIRED:
                    fired = shared_prefs.GetBoolean(KEY_NAME_FIRED, false);
                    break;
                case KEY_NAME_CALL_PHONE_INDEX:
                    call_phone_index = shared_prefs.GetInt(KEY_NAME_CALL_PHONE_INDEX, 0);
                    break;
                case KEY_NAME_NOTIFICANDO:
                    notificando = shared_prefs.GetBoolean(KEY_NAME_NOTIFICANDO, false);
                    break;
                case KEY_NAME_TECLADO_ENABLED:
                    teclado_enabled = shared_prefs.GetBoolean(KEY_NAME_TECLADO_ENABLED, false);
                    break;
                case KEY_NAME_SCHUDLED_CALL:
                    schudled_call = shared_prefs.GetBoolean(KEY_NAME_SCHUDLED_CALL, false);
                    break;
                case KEY_NAME_SCHUDLED_CALL_NUMBER:
                    schudled_call_number = shared_prefs.GetString(KEY_NAME_SCHUDLED_CALL_NUMBER, "");
                    break;
                case KEY_NAME_LAST_CALL_STATE:
                    Last_Call_State = CallState_FromString(shared_prefs.GetString(KEY_NAME_LAST_CALL_STATE, "IDLE"));
                    break;
                case KEY_NAME_TIME_TO_GO:
                    Time_To_GO = shared_prefs.GetInt(KEY_NAME_TIME_TO_GO, 0);
                    break;
                case KEY_NAME_TIME_TO_IN:
                    Time_To_IN = shared_prefs.GetInt(KEY_NAME_TIME_TO_IN, 0);
                    break;
                case KEY_NAME_NEXT_SCHUDLE_NAME:
                    Next_Schudle_Name = shared_prefs.GetString(KEY_NAME_NEXT_SCHUDLE_NAME, "");
                    break;
                case KEY_NAME_HAS_AUDIO_FOCUS:
                    Has_Audio_Focus = shared_prefs.GetBoolean(KEY_NAME_HAS_AUDIO_FOCUS, false);
                    break;
                case KEY_NAME_SIM_NUMBER:
                    Sim_Number = shared_prefs.GetString(KEY_NAME_SIM_NUMBER, "");
                    break;
            }
        }
        public bool IsValidNumbersString()
        {
            List<String> result = numbers.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            bool r = true;
            result.ForEach((s) => {
                if (!s.IsWholeNumber())
                {
                    r = false;
                }
            });
            return r;
        }
        public String GetInfoForRequest(Context context, bool is_new_request = false)
        {
            TelephonyManager mngr = (TelephonyManager)context.GetSystemService(Context.TelephonyService);
            String device_id = mngr.DeviceId;
            String sim_serial = mngr.SimSerialNumber;
            String sim_number = mngr.Line1Number;

            if (is_new_request)
                RequestDate = DateTime.Now;

            String result = "<CInfo>"
                + "<Sim>" + sim_serial + "</Sim>"
                    + "<LNum>" + sim_number + "</LNum>"
                    + "<Date>" + RequestDate.ToShortDateString() + "</Date>"
                    + "<CName>" + Client_Name + "</CName>"
                    + "<CType>" + Client_Type + "</CType>"
                    + "</CInfo>";

            return result;
        }
    }
}