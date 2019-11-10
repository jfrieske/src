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
        private NumberPicker npAnswers;
        private ISharedPreferences prefs;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_test_params);

            prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            npQuestions = (NumberPicker) FindViewById(Resource.Id.npQuestions);
            npQuestions.MinValue = 1;
            npQuestions.MaxValue = 99;            
            npQuestions.Value = prefs.GetInt("npQuestions_Value", 10);
            npAnswers = (NumberPicker)FindViewById(Resource.Id.npAnswers);
            npAnswers.MinValue = 1;
            npAnswers.MaxValue = 5;
            npAnswers.Value = prefs.GetInt("npAnswers_Value", 3);

            FindViewById<Button>(Resource.Id.btnClose).Click += BtnClose_Click;
            FindViewById<Button>(Resource.Id.btnTest).Click += BtnTest_Click;
            FindViewById<CheckedTextView>(Resource.Id.checkedTextView1).Click += CheckCategory_Click;
            FindViewById<CheckedTextView>(Resource.Id.checkedTextView2).Click += CheckCategory_Click;
        }

        private void CheckCategory_Click(object sender, EventArgs e)
        {
            ((CheckedTextView)sender).Checked = !((CheckedTextView)sender).Checked;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutInt("npQuestions_Value", npQuestions.Value);
            editor.PutInt("npAnswers_Value", npAnswers.Value);
            editor.Commit();
        }
    }
}