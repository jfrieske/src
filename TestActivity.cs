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
using Newtonsoft.Json;
using Xamarin.Essentials;
using Android.Gms.Ads;
using Android.Preferences;

namespace vocab_tester
{
    [Activity(Label = "TestActivity")]
    public class TestActivity : Activity
    {        
        
        private List<TestHelper.Question> questions;
        private int questionIndex;
        private int questionsAnswered;
        private RadioGroup radioGroupAnswers;
        private Locale locale;
        private bool answer_is_checked;
        private ProgressBar progressAnswered;
        private int ACTIVITY_SUMMARY = 300;
        private int ACTIVITY_TESTINPUT = 400;
        protected AdView mAdView;
        private DateTime duration_start;
        private bool show_testinput;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_test);

            mAdView = FindViewById<AdView>(Resource.Id.adView1);
            var adRequest = new AdRequest.Builder().Build();
            mAdView.LoadAd(adRequest);

            Bundle bundle = Intent.GetBundleExtra("testParams");
            int oldQuestionsCount = bundle.GetInt("oldQuestions", 0);
            int newQuestionsCount = bundle.GetInt("newQuestions", 0);
            List<long> categories = bundle.GetLongArray("categories").ToList();
            show_testinput = bundle.GetBoolean("show_testinput", false);

            questions = new List<TestHelper.Question>();
            DictionaryDBHelper db_helper = new DictionaryDBHelper();
            List<DictionaryDBHelper.QuestionExt1> db_questions = db_helper.GetNewQuestions(newQuestionsCount, categories);
            db_questions.AddRange(db_helper.GetOldQuestions(oldQuestionsCount, categories));
            foreach (DictionaryDBHelper.QuestionExt1 db_question in db_questions)
            {
                TestHelper.Question question = new TestHelper.Question(db_question.Id, db_question.Name, db_question.Is_old);
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
                    //List<DictionaryDBHelper.Answer> answers = db_helper.GetAnswersForCategories(3, categories, valid_answer.Id);
                    List<DictionaryDBHelper.Answer> answers = db_helper.GetAnswersForSealedCategory(3, db_question.Category_id, valid_answer.Id);
                    for (int i = 0; i < answers.Count; i++)
                    {
                        question.AddAnswer(answers[i].Value, false);
                    }
                }
                questions.Add(question);
            }

            if (questions.Count == 0)
            {
                Toast.MakeText(this, "Brak pytań dla wybranych kategorii", ToastLength.Short).Show();
                SetResult(Result.FirstUser);
                Finish();
                return;
            }

            radioGroupAnswers = FindViewById<RadioGroup>(Resource.Id.rgAnswers);
            questionIndex = 0;
            questionsAnswered = 0;
            progressAnswered = FindViewById<ProgressBar>(Resource.Id.progressAnswered);
            progressAnswered.Max = questions.Count();

            GenerateQuestion();

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            string locale_saved_name = prefs.GetString("localeName", "");
            var locales = await TextToSpeech.GetLocalesAsync();
            locale = locales.FirstOrDefault(l => l.Name == locale_saved_name);

            FindViewById<Button>(Resource.Id.btnVerify).Click += btnVerify_Click;
            FindViewById<Button>(Resource.Id.btnNext).Click += btnNext_Click;
            FindViewById<Button>(Resource.Id.btnClose).Click += BtnClose_Click;

            duration_start = DateTime.Now;
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            if (answer_is_checked)
            {
                VerifyQuestion();
            }
            else
            {
                Toast.MakeText(this, "Musisz zaznaczyć odpowiedź.", ToastLength.Short).Show();
            }            
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
                if (show_testinput)
                {
                    ShowTestInput();
                }
                else
                {
                    ShowSummary();
                }
            }
        }

        private async void BtnClose_Click(object sender, EventArgs e)
        {
            var answer = await MessageHelper.MessageBoxQuestion.Show(this, "Potwierdź", "Czy chcesz przerwać test ?", "", "");
            if (answer == MessageHelper.MessageBoxQuestion.MessageBoxResult.Positive)
            {
                Close_Ok();
            }
        }

        private void Close_Ok()
        {
            SetResult(Result.Ok);
            Finish();
        }

        private void GenerateQuestion()
        {
            answer_is_checked = false;
            FindViewById<TextView>(Resource.Id.textQuestion).Text = questions[questionIndex].value;
            radioGroupAnswers.RemoveAllViews();
            List<TestHelper.Question.Answer> answers = new List<TestHelper.Question.Answer>();
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
            RadioButton radio = new RadioButton(this)
            {
                Text = value,
                Tag = is_correct
            };
            radio.Click += Radio_Click;
            radioGroupAnswers.AddView(radio);
        }

        private async void Radio_Click(object sender, EventArgs e)
        {
            FindViewById<Button>(Resource.Id.btnVerify).Visibility = ViewStates.Invisible;
            answer_is_checked = true;
            var settings = new SpeechOptions()
            {
                Locale = locale
            };

            await TextToSpeech.SpeakAsync(((RadioButton)sender).Text, settings);
            FindViewById<Button>(Resource.Id.btnVerify).Visibility = ViewStates.Visible;
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
                        progressAnswered.Progress = questionsAnswered;
                    }
                    else
                    {
                        ++questions[questionIndex].wrong_answers;
                        child.SetTextColor(Android.Graphics.Color.Red);
                    }
                }
                
            }
            FindViewById<Button>(Resource.Id.btnVerify).Visibility = ViewStates.Gone;
            FindViewById<Button>(Resource.Id.btnNext).Visibility = ViewStates.Visible;            
        }

        private void ShowSummary()
        {            
            DictionaryDBHelper db_helper = new DictionaryDBHelper();
            db_helper.UpdateQuestionsStats(duration_start, questions);
            
            var intent = new Intent(this, typeof(TestSummaryActivity));
            
            //Bundle bundle = new Bundle();
            //bundle.PutInt("oldQuestions", npOldQuestions.Value);
            //bundle.PutInt("newQuestions", npNewQuestions.Value);
            //bundle.put .PutLongArray("categories", checked_ids.ToArray());
            intent.PutExtra("questions", JsonConvert.SerializeObject(questions));
            intent.PutExtra("parent_activity", "test");
            StartActivityForResult(intent, ACTIVITY_SUMMARY);
        }

        private void ShowTestInput()
        {            
            var intent = new Intent(this, typeof(TestInputActivity));

            //Bundle bundle = new Bundle();
            //bundle.PutInt("oldQuestions", npOldQuestions.Value);
            //bundle.PutInt("newQuestions", npNewQuestions.Value);
            //bundle.put .PutLongArray("categories", checked_ids.ToArray());
            intent.PutExtra("questions", JsonConvert.SerializeObject(questions));
            StartActivityForResult(intent, ACTIVITY_TESTINPUT);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == ACTIVITY_SUMMARY)
            {
                if (resultCode == Result.Ok)
                {
                    Close_Ok();
                }                
            }
            if (requestCode == ACTIVITY_TESTINPUT)
            {
                if (resultCode == Result.Ok)
                {
                    Close_Ok();
                }
                if (resultCode == Result.Canceled)
                {
                    Close_Ok();
                }
            }

        }
    }
}