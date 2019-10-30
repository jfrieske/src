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