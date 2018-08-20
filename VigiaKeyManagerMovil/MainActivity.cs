using Android.App;
using Android.Widget;
using Android.OS;

namespace VigiaKeyManagerMovil
{
    [Activity(Label = "VigiaKeyManagerMovil", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            var gen_button = FindViewById<Button>(Resource.Id.bt_genFirma);
            
            gen_button.Click += Gen_button_Click;
        }

        private void Gen_button_Click(object sender, System.EventArgs e)
        {
            var tb_request = FindViewById<EditText>(Resource.Id.tb_request);
            var tb_firma = FindViewById<EditText>(Resource.Id.tb_firma);
            if (tb_request.Text != "")
            {
                var firma = VigiaCryptoFunctions.LicenseUtility.GenFirma(tb_request.Text);
                tb_firma.Text = firma;
            }
        }
    }
}

