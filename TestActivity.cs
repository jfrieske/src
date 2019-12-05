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
            List<long> categories = bundle.GetLongArray("categories").ToList();
            questions = new List<Question>();

            DictionaryDBHelper db_helper = new DictionaryDBHelper();
            List<DictionaryDBHelper.QuestionExt1> db_questions = db_helper.GetNewQuestions(newQuestions, categories);
            foreach (DictionaryDBHelper.QuestionExt1 db_question in db_questions)
            {
                Question question = new Question(db_question.Id, db_question.Name);
                List<DictionaryDBHelper.Answer> answers = db_helper.GetAnswersForQuestion(db_question.Id);
                for (int i = 0; i < answers.Count - 1; i++)
                {
                    question.AddAnswer(answers[i].Value, i == 0);
                }
                if (db_question.Is_sealed)
                {
                    
                }
                else
                    if (db_question.Category_Is_sealed)
                {
                    answers = db_helper.GetAnswersForCategory(db_question.Category_id, db_question.Id));
                    for (int i = 0; i < answers.Count - 1; i++)
                    {
                        question.AddAnswer(answers[i].Value, false);
                    }
                }
                else
                {

                }
            }

            FindViewById<Button>(Resource.Id.btnClose).Click += BtnClose_Click;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Finish();
        }
    }
}