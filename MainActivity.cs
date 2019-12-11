using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace vocab_tester
{
    [Activity(Label = "vocabulary tester", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FindViewById<Button>(Resource.Id.btnClose).Click += BtnClose_Click;
            FindViewById<Button>(Resource.Id.btnDictionary).Visibility = ViewStates.Gone;
            FindViewById<Button>(Resource.Id.btnDictionary).Click += BtnDictionary_Click;
            FindViewById<Button>(Resource.Id.btnTest).Click += BtnTest_Click;

        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(TestParamsActivity));
            StartActivity(intent);
        }

        private void BtnDictionary_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(DictionaryActivity));  
            StartActivity(intent);
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            FinishAffinity();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_dictionary_synchro)
            {
                var intent = new Intent(this, typeof(DictionarySynchroActivity));
                StartActivity(intent);
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}

