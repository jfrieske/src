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
using Newtonsoft.Json;

namespace vocab_tester
{
    [Activity(Label = "TestSummaryActivity")]
    public class TestSummaryActivity : Activity
    {
        private List<TestHelper.Question> questions;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_test_summary);

            questions = JsonConvert.DeserializeObject<List<TestHelper.Question>>(Intent.GetStringExtra("questions"));

            FindViewById<Button>(Resource.Id.btnClose).Click += BtnClose_Click;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Finish();
        }
    }
}