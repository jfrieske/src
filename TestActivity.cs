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
    [Activity(Label = "TestActivity")]
    public class TestActivity : Activity
    {
        private List<DictionaryDBHelper.Question> questions;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_test);

            Bundle bundle = Intent.GetBundleExtra("testParams");
            int oldQuestions = bundle.GetInt("oldQuestions", 0);
            int newQuestions = bundle.GetInt("newQuestions", 0);

            FindViewById<Button>(Resource.Id.btnClose).Click += BtnClose_Click;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Finish();
        }
    }
}