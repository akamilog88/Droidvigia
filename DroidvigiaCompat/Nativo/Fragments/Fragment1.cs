using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;


namespace DroidvigiaCompat.Fragments
{
    public class Fragment1 : Fragment, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Create your fragment here
        }

        public static Fragment1 NewInstance()
        {
            var frag1 = new Fragment1 { Arguments = new Bundle() };
            return frag1;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.fragment1, null);            
        }
        public override void OnResume()
        {
            base.OnResume();
            ISharedPreferences pref = Global.GetSharedPreferences(this.Activity);
            pref.RegisterOnSharedPreferenceChangeListener(this);

            Switch bt = this.Activity.FindViewById<Switch>(Resource.Id.switch_notificacion);
            var prefmodel = Global.GetAppPreferences(this.Activity);
            bt.Checked = prefmodel.Notificando;
            bt.CheckedChange += OnCheckedChange;

            Switch bt_audio_focus = this.Activity.FindViewById<Switch>(Resource.Id.switch_deteccion);
            //bt_audio_focus.Checked = prefmodel.Has_Audio_Focus;
        }
        private void OnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            PreferencesModel prefs_model = Global.GetAppPreferences(this.Activity);
            if (e.IsChecked)
            {
                prefs_model.Notificando = true;
            }
            else
            {
                prefs_model.Notificando = false;
            }
        }
        public override void OnPause()
        {
            base.OnPause();
            Switch bt = this.Activity.FindViewById<Switch>(Resource.Id.switch_notificacion);
            bt.CheckedChange -= OnCheckedChange;

            ISharedPreferences pref = Global.GetSharedPreferences(this.Activity);
            pref.UnregisterOnSharedPreferenceChangeListener(this);
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            /*if (key == PreferencesModel.KEY_NAME_HAS_AUDIO_FOCUS)
            {
                var bt = this.Activity.FindViewById<Switch>(Resource.Id.switch_deteccion);
                bt.Checked = sharedPreferences.GetBoolean(PreferencesModel.KEY_NAME_HAS_AUDIO_FOCUS, false);
            }*/
        }
    }
}