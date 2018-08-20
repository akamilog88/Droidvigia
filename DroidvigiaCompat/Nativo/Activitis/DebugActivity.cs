using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using DroidToneDecoder;

namespace DroidvigiaCompat
{
    [Activity(Label = "DebugActivity")]
    public class DebugActivity : AppCompatActivity, IDetectorServiceBinderClient
    {
        public bool isBound { get ; set ; }
        public DetectorServiceBinder Binder { get ; set; }

        private DetectorServiceConnection connector;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DebugActivityLayout);
            // Create your application here
        }

        protected override void OnResume()
        {
            base.OnResume();
           
            connector = new DetectorServiceConnection(this);
            BindService(new Intent(this.BaseContext, typeof(DetectorService)), connector, Bind.None);
            connector.BindComplete += (s,e)=> {
                var service = Binder.GetService();
                service.NewKey += OnKey;
                service.NewToken += OnToken;
                isBound = true;
            };
        }

        public void OnKey(object s, DTMFDetectedArgs k_evt) {
            var debug_text = FindViewById<EditText>(Resource.Id.editText_Debug);
            if (k_evt.TonConstant != DTMFConstanst.DSIL)
                RunOnUiThread(() => {
                    debug_text.Text += "K" + k_evt.ToneAscii;
                });
        }
        public void OnToken(object s, DTMFTokenBuildedEventArgs t_evt)
        {
            var debug_text = FindViewById<EditText>(Resource.Id.editText_Debug);
            RunOnUiThread(() => {
                debug_text.Text += "T" + t_evt.Token;
            });
        }
        protected override void OnPause()
        {
            base.OnPause();
            if (Binder != null)
            {
                var service = Binder.GetService();
                service.NewKey -= OnKey;
                service.NewToken -= OnToken;
                UnbindService(connector);
            }
        }
    }
}