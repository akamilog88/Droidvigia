using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Telephony;
using DroidVIGIA.Utils;


namespace DroidvigiaBasicoCompat
{
    [Service]
    public class DetectorService : Service, ISharedPreferencesOnSharedPreferenceChangeListener, AudioManager.IOnAudioFocusChangeListener
    {
        private bool initialized;
        private HeadSetConnectedReciver headset_reciver;
        private NotificationManager notification_mngr;
        private Notification notification;
        private PendingIntent notification_pendingIntent;
        private MyPhoneStateListener phone_state_listener;

        public const int NOTIFICATION_ID = 111;
        public const int ALARM_NOTIFICATION_ID = 222;

        bool notificando;

        public bool Notificando
        {
            get { return notificando; }
            private set
            {
                var prefs_model = Global.GetAppPreferences(this);
                if (value != notificando)
                {
                    notificando = value;
                    if (notificando)
                    {
                        AlarmManager alarm_mngr = (AlarmManager)GetSystemService(Context.AlarmService);
                        Intent intent = new Intent("DroidVIGIA.DroidVIGIA.DroidVigiaBasic.SendAlarmBroadcastReciver");
                        PendingIntent pintent = PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);
                        SendBroadcast(intent);
                        long millis = 60000;
                        //alarm_mngr.SetRepeating (AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime()+ millis , pintent);
                        alarm_mngr.SetRepeating(AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime(), millis, pintent);
                    }
                }
                if (!notificando)
                {
                    AlarmManager alarm_mngr = (AlarmManager)GetSystemService(Context.AlarmService);
                    Intent intent = new Intent("DroidVIGIA.DroidVIGIA.DroidVigiaBasic.SendAlarmBroadcastReciver");
                    PendingIntent pintent = PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);
                    alarm_mngr.Cancel(pintent);
                }
            }
        }

        private void Init()
        {
            if (initialized)
                Realese();

            headset_reciver = new HeadSetConnectedReciver();
            this.RegisterReceiver(headset_reciver, new IntentFilter("android.intent.action.HEADSET_PLUG"));

            var audio_mngr = (AudioManager)GetSystemService(Context.AudioService);
            audio_mngr.RequestAudioFocus(this, Stream.Music, AudioFocus.Gain);
            var component_name = new ComponentName(this, Java.Lang.Class.FromType(typeof(MediaButtonReciver)));
            audio_mngr.RegisterMediaButtonEventReceiver(component_name);

            ISharedPreferences shared_prefs = Global.GetSharedPreferences(this);
            shared_prefs.RegisterOnSharedPreferenceChangeListener(this);
            PreferencesModel prefs_model = Global.GetAppPreferences(this);
            if (prefs_model.Notificando)
            {
                Notificando = true;
            }

            TelephonyManager tmgr = (TelephonyManager)this.GetSystemService(Service.TelephonyService);
            if (phone_state_listener == null)
                phone_state_listener = new MyPhoneStateListener(this);
            tmgr.Listen(phone_state_listener, PhoneStateListenerFlags.CallState);
            initialized = true;
        }
        private void Realese()
        {
            try
            {
                this.UnregisterReceiver(headset_reciver);
            }
            catch (Exception)
            {
            }
            Notificando = false;
            var audio_mngr = (AudioManager)GetSystemService(Context.AudioService);

            var component_name = new ComponentName(this, Java.Lang.Class.FromType(typeof(MediaButtonReciver)));
            audio_mngr.UnregisterMediaButtonEventReceiver(component_name);

            TelephonyManager tmgr = (TelephonyManager)this.GetSystemService(Service.TelephonyService);
            tmgr.Listen(phone_state_listener, PhoneStateListenerFlags.None);
            var shared_prefs = Global.GetSharedPreferences(this);
            shared_prefs.UnregisterOnSharedPreferenceChangeListener(this);

            NotificationManager n_mngr = (NotificationManager)GetSystemService(Service.NotificationService);
            n_mngr.CancelAll();
        }
        public override IBinder OnBind(Intent intent)
        {
            return new DetectorServiceBinder(this);
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override void OnDestroy()
        {
            Realese();
            base.OnDestroy();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            base.OnStartCommand(intent, flags, startId);

            notification_mngr = (NotificationManager)GetSystemService(Context.NotificationService);
            notification = new Notification(Resource.Drawable.vigia_icon2, "Servicio de deteccion en ejecucion.");
            Intent notificationIntent = new Intent(this, typeof(MainActivity));
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, 0);
            notification_pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            notification.SetLatestEventInfo(this, "Servicio Iniciado", "Servicio de deteccion iniciado.Hora:" + DateTime.Now.ToShortTimeString(), notification_pendingIntent);
            StartForeground(NOTIFICATION_ID, notification);
            Init();
            return StartCommandResult.Sticky;
        }
        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            PreferencesModel prefs_model = Global.GetAppPreferences(this);

            if (key == PreferencesModel.KEY_NAME_NOTIFICANDO)
            {
                Notificando = sharedPreferences.GetBoolean(PreferencesModel.KEY_NAME_NOTIFICANDO, false);
            }
        }
        public void OnAudioFocusChange(AudioFocus focusChange)
        {
            switch (focusChange)
            {
                case AudioFocus.Gain:
                    Global.GetAppPreferences(this).Has_Audio_Focus = true;
                    break;
                case AudioFocus.GainTransient:
                    break;
                case AudioFocus.GainTransientMayDuck:
                    break;
                case AudioFocus.Loss:
                    Global.GetAppPreferences(this).Has_Audio_Focus = false;
                    break;
                case AudioFocus.LossTransient:
                    Global.GetAppPreferences(this).Has_Audio_Focus = false;
                    break;
                case AudioFocus.LossTransientCanDuck:
                    Global.GetAppPreferences(this).Has_Audio_Focus = false;
                    break;
            }
        }

        void NotifyEvent(string display, string message)
        {
            notification.Defaults |= NotificationDefaults.Sound;
            notification.SetLatestEventInfo(this, display, message, notification_pendingIntent);
            notification_mngr.Notify(NOTIFICATION_ID, notification);
        }
    }
    public class DetectorServiceBinder : Binder
    {
        private DetectorService _service;
        public DetectorServiceBinder(DetectorService service)
        {
            _service = service;
        }
        public DetectorService GetService()
        {
            return _service;
        }
    }
    public class DetectorServiceConnection : Java.Lang.Object, IServiceConnection
    {
        private MainActivity actvt;
        public event EventHandler<EventArgs> BindComplete;

        public DetectorServiceConnection(MainActivity act)
        {
            actvt = act;
        }
        public DetectorServiceConnection()
        {
            //actvt = act;	
        }
        #region IServiceConnection implementation

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            DetectorServiceBinder binder = service as DetectorServiceBinder;
            if (binder != null)
            {
                actvt.Binder = binder;
                actvt.isBound = true;
                if (BindComplete != null)
                    BindComplete.Invoke(this, null);
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            actvt.Binder = null;
            actvt.isBound = false;
            actvt = null;
        }
        #endregion
    }
    public class MyPhoneStateListener : PhoneStateListener
    {
        private Context context;
        public MyPhoneStateListener(Context _context)
        {
            context = _context;
        }
        public override void OnCallStateChanged(CallState state, String incomingNumber)
        {
            //Device call state: Ringing. A new call arrived and is ringing or waiting. In the latter case, another call is already active. 

            PreferencesModel prefmodel = Global.GetAppPreferences(context);

            if (state == CallState.Ringing)
            {
                if (prefmodel.Last_Call_State == CallState.Idle
                    && (prefmodel.numbers.Contains(incomingNumber.CleanNumber())))
                {
                    prefmodel.Fired = false;
                    prefmodel.Call_Phone_Index = 0;
                    prefmodel.Notificando = false;
                }
                prefmodel.Last_Call_State = state;
            }
            //Device call state: Off-hook. At least one call exists that is dialing, active, or on hold, and no calls are ringing or waiting. 
            if (state == CallState.Offhook)
            {
                prefmodel.Last_Call_State = state;
            }
            //Device call state: No activity. 
            if (state == CallState.Idle)
            {
                if (prefmodel.Last_Call_State == CallState.Offhook)
                {
                    //Intent i = new Intent (context, typeof(DetectorService));
                    //context.StopService (i);					
                    //context.StartService(i);
                }
                prefmodel.Last_Call_State = state;
            }
        }
    }
}

