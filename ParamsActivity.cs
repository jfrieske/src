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
using Xamarin.Essentials;

namespace vocab_tester
{
    [Activity(Label = "ParamsActivity")]
    public class ParamsActivity : Activity
    {
        private List<KeyValuePair<string, string>> localesList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_params);

            PopulateLocalesAsync();

            FindViewById<Button>(Resource.Id.btnSave).Click += BtnSave_Click;
            FindViewById<Button>(Resource.Id.btnClose).Click += BtnClose_Click;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {

        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            SetResult(Result.Ok);
            Finish();
        }

        private async void PopulateLocalesAsync()
        {
            Spinner spinner = FindViewById<Spinner>(Resource.Id.spinLocales);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinLocales_ItemSelected);
            

            localesList = new List<KeyValuePair<string, string>>();
            var locales = await TextToSpeech.GetLocalesAsync();
            foreach (Locale locale in locales)
            {
                localesList.Add(new KeyValuePair<string, string>(locale.Name, locale.Name));
            }
            List<string> localeNames = new List<string>();
            foreach (var item in localesList)
            {
                localeNames.Add(item.Key);
            }
            var adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, localeNames);
            adapter.SetDropDownViewResource(Resource.Layout.spinner_item);
            spinner.Adapter = adapter;

        }
        private void SpinLocales_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string toast = string.Format("The mean temperature for planet {0} is {1}",
                spinner.GetItemAtPosition(e.Position), localesList[e.Position].Value);
            Toast.MakeText(this, toast, ToastLength.Long).Show();
        }
    }
}