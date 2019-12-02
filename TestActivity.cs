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
        
        private class Question
        {
            public class Answer
            {
                public string name { get; set; }
                public bool is_correct { get; set; }
            }

            public long id;
            public string name;
            public bool is_answered;
            public long wrong_answers;
            public List<Answer> answers;
            
            public Question(long id, string name)
            {
                this.id = id;
                this.name = name;
                wrong_answers = 0;
                is_answered = false;
                answers = new List<Answer>();                
            }

            public void AddAnswer(string name, bool is_correct)
            {
                answers.Add(new Answer { name = name, is_correct = is_correct });
            }
        }
        private List<Question> questions;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_test);

            Bundle bundle = Intent.GetBundleExtra("testParams");
            int oldQuestions = bundle.GetInt("oldQuestions", 0);
            int newQuestions = bundle.GetInt("newQuestions", 0);
            questions = new List<Question>();

            FindViewById<Button>(Resource.Id.btnClose).Click += BtnClose_Click;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Finish();
        }
    }
}