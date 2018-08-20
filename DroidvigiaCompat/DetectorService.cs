using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Speech.Tts;
using Android.Support.V4.Content;
using Android.Telephony;
using Android.Media;
using DroidToneDecoder;
using DroidvigiaCompat.Utils;
using Wpam.Recognizer;

namespace DroidvigiaCompat
{
	[Service]
	public class DetectorService:Service,TextToSpeech.IOnInitListener,ISharedPreferencesOnSharedPreferenceChangeListener, AudioManager.IOnAudioFocusChangeListener,IControllerClient
	{
		//DTMFTokenParser parser;
		private MyPhoneStateListener phone_state_listener;
        //private DTMFClipListener dtmfClipListener;
        private DTMFTokenParser tokenParser;
		private AudioClipRecorder clipRecorder;
		ToneDetectedReciver tone_detected_reciver;
		private Task recorderThread;
		private TextToSpeech speech_synt;
		private Notification notification;
		private PendingIntent notification_pendingIntent;
		private NotificationManager notification_mngr;
		private bool initialized=false;
		private DTMFTokenBuildedEventArgs Last_Token;
		//private DateTime activation_time; 
		private PreferencesModel prefs_model;
		//bool modoComplemento;
		System.Threading.Timer notification_timer;

        Controller _jdtmfController;

        public event EventHandler<DTMFTokenBuildedEventArgs> NewToken;
        public event EventHandler<DTMFDetectedArgs> NewKey;

        bool notificando;
		public bool Notificando{ get{return notificando;}
			private set{
				if(value != notificando){
					notificando =  value;
					prefs_model.Notificando = notificando;
					if (notificando && prefs_model.Ready) {
						int due_time = 0;
						if (prefs_model.Fired)
							due_time = 60000;
						var intent = new Intent("DroidVigia.DroidVigia.DroidVigia.ToneDetected");
							notification_timer = new System.Threading.Timer ((o)=>{
							SendBroadcast(intent);
						}, null,due_time, 60000);	
					}
				}
				if (!notificando && notification_timer != null) {
					notification_timer.Dispose ();
					notification_timer=null;
					NotificationManager n_mngr = (NotificationManager)GetSystemService (Service.NotificationService);
					n_mngr.CancelAll ();					
				}
			}}

		private bool schudled_callBack;
		public bool Schudled_CallBack{
			get{return schudled_callBack;}
			set{
				if (schudled_callBack != value) {
					schudled_callBack = value;
					prefs_model.Schudled_Call = value;
					SchudlerManager schudler = new SchudlerManager (this);
					if (schudled_callBack) {
						schudler.RegisterSchudleCall ();
					} else {
						schudler.CancelSchudleCall ();
					}
				}
			}
		}
		public event EventHandler<ActionRegisteredEventArgs> NewHistoryData;

		public const int NOTIFICATION_ID=111;

		public bool speech_engine_inited;

		private object sync_object = new object ();

		public bool Detecting{ get; private set;}

        public int AudioSource => throw new NotImplementedException();

        public DetectorService ()
		{
		
		}
		private void Realese(){		
			TelephonyManager tmgr = (TelephonyManager) this.GetSystemService(Service.TelephonyService);
			if(phone_state_listener!=null)
				tmgr.Listen (phone_state_listener, PhoneStateListenerFlags.None);
			var shared_prefs = Global.GetSharedPreferences (this);
			shared_prefs.UnregisterOnSharedPreferenceChangeListener (this);

            //var audio_mngr = (AudioManager)GetSystemService(Context.AudioService);
            //AudioFocusRequest result = audio_mngr.AbandonAudioFocus(this);

            if (notification_timer!=null){
				notification_timer.Dispose ();
				notification_timer=null;					
			}		
			NotificationManager n_mngr = (NotificationManager)GetSystemService (Service.NotificationService);
			n_mngr.CancelAll ();
            Detecting = false;
            //if(_jdtmfController!=null)
            //    _jdtmfController.Dispose();
            initialized = false;
        }

		public void ForceStop(){
			Realese ();
            StopForeground(true);
			StopSelf ();            
		}
		public void RestartDetect(){
            StopDetect ();
            StartDetect ();
            //Realese();
            //Init();
		}

		private void Init(){
			if (initialized) {
				Realese ();
			}
            tokenParser = new DTMFTokenParser();

            ISharedPreferences shared_prefs = Global.GetSharedPreferences (this);
			shared_prefs.RegisterOnSharedPreferenceChangeListener (this);
			prefs_model = new PreferencesModel (shared_prefs);

            var audio_mngr = (AudioManager)GetSystemService(Context.AudioService);
            AudioFocusRequest result = audio_mngr.RequestAudioFocus(this, Stream.Music, AudioFocus.Gain);

            if (prefs_model.Notificando) {
				Notificando = true;
			}
			//modoComplemento = prefs_model.Modo_Complemento;
			
			TelephonyManager tmgr = (TelephonyManager) this.GetSystemService(Service.TelephonyService);
			if (phone_state_listener == null)
				phone_state_listener = new MyPhoneStateListener (this);
			tmgr.Listen (phone_state_listener, PhoneStateListenerFlags.CallState);
			initialized = true;
		    
			Schudled_CallBack = prefs_model.Schudled_Call;           

            if ((prefs_model.Teclado_Enabled || prefs_model.Ready) && !Detecting){
                new System.Threading.Timer((o) => {
                    StartDetect();
                }, null, 2000, -1);				
			}         

        }
        public void StartDetect()
        {            
            Start();
        }
        public void StopDetect()
        {
            Stop();
        }
        #region JDTMF Controller START/STOP
        public void OnKey(char p0)
        {
            if (Detecting)
            {
                tokenParser.Digest(DTMFDetectedArgs.ToneCodeFromKey(p0));
                if (NewKey != null)
                {
                    try
                    {
                        NewKey.BeginInvoke(null, new DTMFDetectedArgs(p0, 0.064), null, null);
                    }
                    catch (Exception e) { }
                }
            }
        }

        public void Start()
        {
            _jdtmfController = new Controller(this);

            if (!_jdtmfController.IsStarted) {              

                tokenParser.NewToken += OnNewToken;
                _jdtmfController.ChangeState();
                Detecting = true;
            }               
        }

        public void Stop()
        {
            if (_jdtmfController == null)
                return;
            if (_jdtmfController.IsStarted)
            {
                tokenParser.NewToken -= OnNewToken;
                _jdtmfController.ChangeState();
                Detecting = false;
            }
        }
        #endregion

        #region DTMFCLIPLISTENER Start/Stop
        //public void StartDetect(){		
        //	dtmfClipListener = new DTMFClipListener (true);
        //	dtmfClipListener.DetectSilence = true;	
        //	//dtmfClipListener.ToneDetected += OnToneDetected;

        //	//if(!modoComplemento)
        //	dtmfClipListener.NewToken += OnNewToken;
        //	//else
        //	//	dtmfClipListener.NewToken += OnNewTokenModoComplemento;
        //	Detecting = true;
        //	recorderThread = new Task(
        //		()=>
        //		{
        //		clipRecorder = new AudioClipRecorder(dtmfClipListener,recorderThread);
        //		clipRecorder.startRecording (AudioClipRecorder.RECORDER_SAMPLERATE_CD, Android.Media.Encoding.Pcm16bit);
        //	},TaskCreationOptions.LongRunning);
        //	recorderThread.Start();
        //}

        //public void StopDetect(){
        //	try{
        //	//if (modoComplemento)
        //		//dtmfClipListener.NewToken -= OnNewTokenModoComplemento;
        //	//else
        //		dtmfClipListener.NewToken -= OnNewToken;
        //	}catch(Exception){;}
        //	dtmfClipListener = null;
        //	tone_detected_reciver = null;
        //	Detecting = false;
        //	if(clipRecorder!=null)
        //		clipRecorder.stopRecording ();
        //	recorderThread = null;

        //}
        #endregion
        public void OnAudioFocusChange (AudioFocus focusChange)
		{
            if (prefs_model.Teclado_Enabled || prefs_model.Ready)
            {
                switch (focusChange)
                {
                    case AudioFocus.Loss:
                        StopDetect();
                        break;
                    case AudioFocus.LossTransient:
                        StopDetect();
                        break;
                    case AudioFocus.LossTransientCanDuck:
                        StopDetect();
                        break;
                    case AudioFocus.Gain:
                        StartDetect();
                        break;
                    case AudioFocus.GainTransient:
                        StartDetect();
                        break;
                    case AudioFocus.GainTransientExclusive:
                        StartDetect();
                        break;
                    case AudioFocus.GainTransientMayDuck:
                        StartDetect();
                        break;
                    default:
                        StopDetect();
                        break;
                }
            }
		}
        private void OnNewToken(object sender, DTMFTokenBuildedEventArgs edt)
        {
            lock (sync_object)
            {
                DAL dal = new DAL();
                List<Partition> particiones = dal.GetAllPartitions();
                if (edt.Token == prefs_model.Pin)
                {
                    prefs_model.Ready = !prefs_model.Ready;

                    if (prefs_model.Ready)
                    {
                        /*if(speech_engine_inited)
									speech_synt.Speak("Alarma activada",QueueMode.Flush,null);*/
                        particiones.ForEach((p) => dal.UpdatePartitionState(p.Id, true));
                        NotifyEvent("Alarma Activada", "Todas las particiones activadas");
                        var h = new HistoryItem
                        {
                            Time = DateTime.Now,
                            State = Action.ACTION_ID_ACTIVATED,
                            PartitionName = "TODAS",
                            Detail = "TODAS LAS PARTICIONES ACTIVADAS"
                        };
                        dal.RegiterEvent(h);
                        NotifyNewEvent(h);
                        //Ready=true;
                    }
                    else
                    {
                        /*if(speech_engine_inited)
									speech_synt.Speak("Alarma desactivada",QueueMode.Flush,null);*/
                        particiones.ForEach((p) => dal.UpdatePartitionState(p.Id, false));
                        NotifyEvent("Alarma Desactivada", "Todas las particiones desactivadas");
                        var h = new HistoryItem
                        {
                            Time = DateTime.Now,
                            State = Action.ACTION_ID_DESACTIVATED,
                            PartitionName = "TODAS",
                            Detail = "TODAS LAS PARTICIONES DESACTIVADAS"
                        };
                        dal.RegiterEvent(h);
                        NotifyNewEvent(h);
                    }
                }
                else
                {
                    var p = particiones.Find((part) => part.Code == edt.Token);
                    if (p != null)
                    {
                        string detail = "";
                        p.Zones = dal.GetPartitionsZones(p.Id);
                        if (p.Activated)
                        {
                            detail = "Particion Desactivada:" + "Particion " + p.Name + " Desactivada";
                            NotifyEvent("Particion Desactivada", "Particion " + p.Name + " Desactivada");
                            var h = new HistoryItem { Time = DateTime.Now, State = Action.ACTION_ID_DESACTIVATED, PartitionName = p.Name, Detail = detail };
                            dal.RegiterEvent(h);
                            NotifyNewEvent(h);
                            p.Activated = false;
                            dal.UpdatePartitionState(p.Id, false);
                            if (particiones.Find((part) => part.Activated) == null)
                                prefs_model.Ready = false;
                        }
                        else
                        {
                            detail = "Particion Activada:" + "Particion " + p.Name + " Activada";
                            NotifyEvent("Particion Activada", "Particion " + p.Name + " Activada");
                            p.Activated = true;
                            dal.UpdatePartitionState(p.Id, false);
                            var h = new HistoryItem { Time = DateTime.Now, State = Action.ACTION_ID_ACTIVATED, PartitionName = p.Name, Detail = detail };
                            dal.RegiterEvent(h);
                            NotifyNewEvent(h);
                            prefs_model.Ready = false;
                        }
                    }
                    else
                    {
                        var z = (edt.Token.All(cp => cp == edt.Token[0])) ? dal.GetZoneByCode(edt.Token[0].ToString()) : null;
                        if (z != null)
                        {
                            p = particiones.Find((part) => part.Id == z.PartitionId);
                            if (!z.Fired && p.Activated)
                            {
                                if ((DateTime.Now - p.ChangeStateTime) >= new TimeSpan( 0,0,prefs_model.Time_To_GO))
								{
									var due_time =(z.Inmediate ? 0 : prefs_model.Time_To_IN * 1000);
									var timer = new System.Threading.Timer ((o)=>{
										Partition particion = o as Partition;
										var zone = particion.Zones[0];

										particion = dal.GetPartitionByName(p.Name);
										if(particion.Activated){										
											prefs_model.Fired = true;
											zone.Fired=true;
											dal.UpdateZoneState (zone.Id, true);
											string message = "Sensor: "+ z.Name + ".Partición: "+ particion.Name +".Hora:"+ DateTime.Now.ToShortTimeString();
											var h =new HistoryItem{Time=DateTime.Now,State=Action.ACTION_ID_FIRED,PartitionName=particion.Name,Detail=message};
											dal.RegiterEvent(h);
											NotifyNewEvent (h);
											Intent intent = new Intent("DroidVigia.DroidVigia.DroidVigia.ToneDetected");
											intent.PutExtra("message",message);
											SendBroadcast(intent);
											if(!Notificando)
												Notificando=true;
										}
									},new Partition{Id=p.Id,Name=p.Name,Activated=p.Activated,ChangeStateTime=p.ChangeStateTime,Zones=new List<Zone>{z}} ,due_time, -1);
								} 
                            }
                        }
                    }
                }
            }
            if (NewToken != null)
            {
                try
                {
                    NewToken.BeginInvoke(null, edt, null, null);
                }
                catch { }
            }
        }

        #region implemented abstract members of Service
        public override IBinder OnBind (Intent intent)
		{
			return new DetectorServiceBinder(this);
		}
		private void NotifyNewEvent(HistoryItem h){
			if (NewHistoryData != null)
				NewHistoryData.BeginInvoke (this, new ActionRegisteredEventArgs (h), null, null);
		}
	  
		
		void NotifyEvent(string display,string message){
			notification.Defaults |= NotificationDefaults.Sound;
			notification.SetLatestEventInfo (this, display,message, notification_pendingIntent);
			notification_mngr.Notify (NOTIFICATION_ID, notification);
		}
		public override void OnCreate ()
		{
			base.OnCreate ();
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
			//StopDetect ();
			Realese ();
		}
		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId) {
			base.OnStartCommand (intent, flags, startId);
		
			// For each start request, send a message to start a job and deliver the
			// start ID so we know which request we're stopping when we finish the job
			// If we get killed, after returning from here, restart
			
            notification_mngr = (NotificationManager)GetSystemService (Context.NotificationService);
			notification = new Notification(Resource.Drawable.vigia_icon2,"Servicio de deteccion en ejecucion.");
			Intent notificationIntent = new Intent(this, typeof(MainActivity));
			//PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, 0);
			notification_pendingIntent = PendingIntent.GetActivity (this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
			notification.SetLatestEventInfo (this, "Servicio Iniciado", "Servicio de deteccion iniciado.Hora:" + DateTime.Now.ToShortTimeString(), notification_pendingIntent);
			StartForeground(NOTIFICATION_ID, notification);		
			Init ();
			SchudlerManager schudler = new SchudlerManager (this);
			schudler.RegisterNextSchudle ();
			//speech_synt = new TextToSpeech (this, this);
			return StartCommandResult.Sticky;
		}

		#endregion
	    //Speech Recognition Engine Init
		public void OnInit (OperationResult status)
		{
			//if (OperationResult.Success == status)
			//	speech_engine_inited = true;
		}
		public void OnSharedPreferenceChanged (ISharedPreferences sharedPreferences, string key)
		{
			var dal = new DAL ();	
			if(key==PreferencesModel.KEY_NAME_READY){
				var ready = sharedPreferences.GetBoolean ("ready",false);
				if (ready) {
					NotifyEvent ("Alarma Activada","");

					var h = new HistoryItem {
						Time=DateTime.Now,
						State=Action.ACTION_ID_ACTIVATED,
						PartitionName= "detector",
						Detail="ALARMA ACTIVADA"
					};				
					dal.RegiterEvent (h);
					NotifyNewEvent (h);
                    //StopDetect ();
                    //StartDetect ();
                    RestartDetect();
				} else {
					NotifyEvent ("Alarma Desactivada","");
					//dal.DesactivateAllPartitions ();
					var h = new HistoryItem {
						Time=DateTime.Now,
						State=Action.ACTION_ID_DESACTIVATED,
						PartitionName= "detector",
						Detail="ALARMA DESACTIVADA"
					};
					dal.RegiterEvent(h);
					NotifyNewEvent (h);
					prefs_model.Fired=false;
					prefs_model.Call_Phone_Index = 0;
					Notificando = false;
					if(!prefs_model.Teclado_Enabled)
						StopDetect ();									
				}
			}	
			prefs_model.ReloadProperty (key);
			if (key == PreferencesModel.KEY_NAME_TECLADO_ENABLED) {
				StopSelf ();
			}
		}
      
    }
    #region IServiceConnection implementation
    public class DetectorServiceBinder:Binder
	{
		private DetectorService _service;
		public DetectorServiceBinder (DetectorService service)
		{
			_service = service;
		}
		public DetectorService GetService(){
			return _service;
		}
	}
   
    public class DetectorServiceConnection:Java.Lang.Object,IServiceConnection
	{
		private IDetectorServiceBinderClient binderClient;
		public event EventHandler<EventArgs> BindComplete;
		public event EventHandler<EventArgs> ServiceDisconected;

		public DetectorServiceConnection (IDetectorServiceBinderClient client)
		{
			binderClient = client;	
		}

		

		public void OnServiceConnected (ComponentName name, IBinder service)
		{
			DetectorServiceBinder binder = service as DetectorServiceBinder;
			if(binder!=null){
				binderClient.Binder = binder;
				binderClient.isBound = true;
				if (BindComplete != null)
					BindComplete.Invoke (this, null);
			}
		}

		public void OnServiceDisconnected (ComponentName name)
		{			
			if(ServiceDisconected!=null)
				ServiceDisconected.Invoke (this, null);
			binderClient.Binder = null;
			binderClient.isBound = false;
			binderClient = null;
		}
		
	}
    #endregion
    public class MyPhoneStateListener  : PhoneStateListener,IDetectorServiceBinderClient {
		private Context context;
		private CallState _last_CallState;
		public bool isBound { get; set;}
		public DetectorServiceBinder Binder { get; set;}
		private DetectorServiceConnection seviceConection{ get; set;}

		public MyPhoneStateListener(Context _context){
			context = _context;
			isBound = false;
			Binder = null;
			_last_CallState = CallState.Idle;
			seviceConection = new DetectorServiceConnection (this);
			context.BindService (new Intent(context,typeof(DetectorService)),seviceConection, Bind.None);
		}

		public override void OnCallStateChanged(CallState state, String incomingNumber) {
			//Device call state: Ringing. A new call arrived and is ringing or waiting. In the latter case, another call is already active. 

			PreferencesModel prefmodel = Global.GetAppPreferences (context);
			DAL dal = new DAL ();
			if (state == CallState.Ringing) {
				if (prefmodel.Last_Call_State == CallState.Idle 				   
				    && (prefmodel.numbers.Contains(incomingNumber.CleanNumber()))) {
					var h = new HistoryItem {
						Time=DateTime.Now,
						State=Action.ACTION_ID_INCOMING_CALL,
						PartitionName= "",
						Detail="LLAMADA ENTRANTE:" + incomingNumber
					};
					dal.RegiterEvent (h);	
					prefmodel.Ready = !prefmodel.Ready;
					if (prefmodel.Ready) {
						prefmodel.Schudled_Call = true;
						prefmodel.Schudled_Call_Number = incomingNumber;
						dal.ActivateAllPartitions ();
					} else {
						dal.DesactivateAllPartitions ();
					}

                    SchudlerManager s_mngr = new SchudlerManager(context);
                    s_mngr.RegisterRestartServiceSchudle();

                    Intent i = new Intent (context, typeof(DetectorService));
					if(Binder!=null){
						var service= Binder.GetService ();
						service.ForceStop();
					}					
				}
                //if (Binder != null)
                //{
                //    var service = Binder.GetService();
                //    service.Stop();
                //}
                prefmodel.Last_Call_State = state;
			}
			//Device call state: Off-hook. At least one call exists that is dialing, active, or on hold, and no calls are ringing or waiting. 
			if(state== CallState.Offhook){
				prefmodel.Last_Call_State = state;
                //if (Binder != null)
                //{
                //    var service = Binder.GetService();
                //    service.Stop();
                //}
            }
			//Device call state: No activity. 
			if(state== CallState.Idle){
				if (prefmodel.Last_Call_State == CallState.Offhook) {
					//Intent i = new Intent (context, typeof(DetectorService));
					//context.StopService (i);					
					//context.StartService(i);
					//if(Binder!=null){
					//	var service= Binder.GetService ();
					//	service.Start();
					//}
				}
				prefmodel.Last_Call_State = state;
			}
		}
	}
}