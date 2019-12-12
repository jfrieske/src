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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_test_summary);

            Generate_Table(JsonConvert.DeserializeObject<List<TestHelper.Question>>(Intent.GetStringExtra("questions")));

            FindViewById<Button>(Resource.Id.btnClose).Click += BtnClose_Click;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            SetResult(Result.Ok);
            Finish();
        }

        private void Generate_Table(List<TestHelper.Question> questions)
        {
            TableLayout table = FindViewById<TableLayout>(Resource.Id.tableAnswers);
            foreach (TestHelper.Question question in questions)
            {
                TextView text_value = new TextView(this);
                text_value.Text = question.value;
                TextView text_wrong_answers = new TextView(this);
                text_wrong_answers.Text = question.wrong_answers.ToString();
                text_wrong_answers.Gravity=GravityFlags.CenterHorizontal;
                if (question.wrong_answers > 0)
                {
                    text_value.SetTextColor(Android.Graphics.Color.Red);
                    text_wrong_answers.SetTextColor(Android.Graphics.Color.Red);
                }
                TableRow row = new TableRow(this);
                row.AddView(text_value);
                row.AddView(text_wrong_answers);
                table.AddView(row);
            }
        }
    }
}