using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace vocab_tester
{
    [Activity(Label = "TestParamsActivity")]
    public class TestParamsActivity : Activity
    {
        private NumberPicker npQuestions;
        private ISharedPreferences prefs;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_test_params);

            npQuestions = (NumberPicker) FindViewById(Resource.Id.npQuestion);
            npQuestions.MinValue = 1;
            npQuestions.MaxValue = 99;

            prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            npQuestions.Value = prefs.GetInt("npQuestions_Value", 10);

            FindViewById<Button>(Resource.Id.btnClose).Click += BtnClose_Click;
            FindViewById<Button>(Resource.Id.btnTest).Click += BtnTest_Click;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutInt("npQuestions_Value", npQuestions.Value);
            editor.Commit();
        }
    }
}