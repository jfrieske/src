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

namespace vocab_tester
{
    [Activity(Label = "DictionaryActivity")]
    public class DictionaryActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_dictionary);

            FindViewById<Button>(Resource.Id.btnClose).Click += BtnClose_Click;
            FindViewById<Button>(Resource.Id.btnCategoryNew).Click += BtnCategoryNew_Click;
            FindViewById<Button>(Resource.Id.btnPobierz).Click += async delegate
            {
                await KoloAsync();
            };
        }

        private async System.Threading.Tasks.Task KoloAsync()
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                // send a GET request  
                var uri = "http://blog.xamarin.com/feed";
                var result = await client.GetStreamAsync(uri);

                //handling the answer  
                //var serializer = new XmlSerializer(typeof(Rss));
                //var feed = (Rss)serializer.Deserialize(result);

                // generate the output  
                //var item = feed.Channel.Items.First();
                //textView.Text = "First Item:\n\n" + item;
                Toast.MakeText(this, "zrobione", ToastLength.Long).Show();
            }
        }

        private void BtnPobierz_Click(object sender, EventArgs e)
        {
            DictionaryHttp dictionary = new DictionaryHttp();
            dictionary.Download();
        }

        private void BtnCategoryNew_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(CategoryNewActivity));
            StartActivity(intent);
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Finish();
        }
    }
}