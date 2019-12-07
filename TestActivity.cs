using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace vocab_tester
{
    [Activity(Label = "TestActivity")]
    public class TestActivity : Activity
    {        
        private class Question
        {
            public class Answer
            {
                public string value { get; set; }
                public bool is_correct { get; set; }
            }

            public long id;
            public string value;
            public bool is_answered;
            public long wrong_answers;
            public List<Answer> answers;
            
            public Question(long id, string name)
            {
                this.id = id;
                this.value = name;
                wrong_answers = 0;
                is_answered = false;
                answers = new List<Answer>();                
            }

            public void AddAnswer(string name, bool is_correct)
            {
                answers.Add(new Answer { value = name, is_correct = is_correct });
            }
        }
        private List<Question> questions;
        private int questionIndex;
        private int questionsAnswered;
        private RadioGroup radioGroupAnswers;
        private Xamarin.Essentials.Locale locale;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_test);

            Bundle bundle = Intent.GetBundleExtra("testParams");
            int oldQuestionsCount = bundle.GetInt("oldQuestions", 0);
            int newQuestionsCount = bundle.GetInt("newQuestions", 0);
            List<long> categories = bundle.GetLongArray("categories").ToList();
            questions = new List<Question>();

            DictionaryDBHelper db_helper = new DictionaryDBHelper();
            List<DictionaryDBHelper.QuestionExt1> db_questions = db_helper.GetNewQuestions(newQuestionsCount, categories);
            foreach (DictionaryDBHelper.QuestionExt1 db_question in db_questions)
            {
                Question question = new Question(db_question.Id, db_question.Name);
                DictionaryDBHelper.Answer valid_answer = db_helper.GetAnswerForQuestion(db_question.Id);
                question.AddAnswer(valid_answer.Value, true);                
                if (db_question.Is_sealed)
                {
                    List<DictionaryDBHelper.Answer> answers = db_helper.GetAnswersForSealedQuestion(db_question.Id, valid_answer.Id);
                    for (int i = 0; i < answers.Count; i++)
                    {
                        question.AddAnswer(answers[i].Value, false);
                    }
                }
                else
                    if (db_question.Category_Is_sealed)
                {
                    List<DictionaryDBHelper.Answer> answers = db_helper.GetAnswersForSealedCategory(3, db_question.Category_id, valid_answer.Id);
                    for (int i = 0; i < answers.Count; i++)
                    {
                        question.AddAnswer(answers[i].Value, false);
                    }
                }
                else
                {
                    List<DictionaryDBHelper.Answer> answers = db_helper.GetAnswersForCategories(3, categories, valid_answer.Id);
                    for (int i = 0; i < answers.Count; i++)
                    {
                        question.AddAnswer(answers[i].Value, false);
                    }
                }
                questions.Add(question);
            }

            radioGroupAnswers = FindViewById<RadioGroup>(Resource.Id.rgAnswers);
            questionIndex = 0;
            questionsAnswered = 0;
            GenerateQuestion();

            var locales = await TextToSpeech.GetLocalesAsync();
            locale = locales.FirstOrDefault(l => l.Language == "eng");

            FindViewById<Button>(Resource.Id.btnVerify).Click += btnVerify_Click;
            FindViewById<Button>(Resource.Id.btnNext).Click += btnNext_Click;
            FindViewById<Button>(Resource.Id.btnClose).Click += BtnClose_Click;
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            VerifyQuestion();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (questionsAnswered < questions.Count)
            {
                while (true)
                {
                    ++questionIndex;
                    if (questionIndex == questions.Count)
                    {
                        questionIndex = 0;
                    }
                    if (!questions[questionIndex].is_answered)
                    {
                        GenerateQuestion();
                        break;
                    }
                }
            }
            else
            {
                var intent = new Intent(this, typeof(TestSummaryActivity));
                /*Bundle bundle = new Bundle();
                bundle.PutInt("oldQuestions", npOldQuestions.Value);
                bundle.PutInt("newQuestions", npNewQuestions.Value);
                bundle.PutLongArray("categories", checked_ids.ToArray());
                intent.PutExtra("testParams", bundle);*/
                StartActivity(intent);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void GenerateQuestion()
        {
            FindViewById<TextView>(Resource.Id.textQuestion).Text = questions[questionIndex].value;
            radioGroupAnswers.RemoveAllViews();
            List<Question.Answer> answers = new List<Question.Answer>();
            answers.AddRange(questions[questionIndex].answers);
            while (answers.Count > 1)
            {
                var random = new Random();
                int answer_index = random.Next(answers.Count);
                AddAnswer(answers[answer_index].value, answers[answer_index].is_correct);
                answers.RemoveAt(answer_index);
            }
            AddAnswer(answers[0].value, answers[0].is_correct);
            FindViewById<Button>(Resource.Id.btnVerify).Visibility = ViewStates.Visible;
            FindViewById<Button>(Resource.Id.btnNext).Visibility = ViewStates.Gone;
        }        

        private void AddAnswer(string value, bool is_correct)
        {
            RadioButton radio = new RadioButton(this);
            radio.Text = value;
            radio.Tag = is_correct;
            radio.Click += Radio_Click;
            radioGroupAnswers.AddView(radio);
        }

        private async void Radio_Click(object sender, EventArgs e)
        {            
            var settings = new SpeechOptions()
            {
                Locale = locale
            };

            await TextToSpeech.SpeakAsync(((RadioButton)sender).Text, settings);
        }

        private void VerifyQuestion()
        {
            int childCount = radioGroupAnswers.ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                RadioButton child = (RadioButton)radioGroupAnswers.GetChildAt(i);
                child.Clickable = false;
                if ((bool)child.Tag)
                {
                    child.SetTextColor(Android.Graphics.Color.Green);
                }
                if (child.Checked)
                {
                    if ((bool)child.Tag)
                    {
                        ++questionsAnswered;
                        questions[questionIndex].is_answered = true;
                    }
                    else
                    {
                        child.SetTextColor(Android.Graphics.Color.Red);
                    }
                }
                
            }
            FindViewById<Button>(Resource.Id.btnVerify).Visibility = ViewStates.Gone;
            FindViewById<Button>(Resource.Id.btnNext).Visibility = ViewStates.Visible;
        }
    }
}