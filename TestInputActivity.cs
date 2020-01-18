using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace vocab_tester
{
    [Activity(Label = "TestInputActivity")]
    public class TestInputActivity : Activity
    {
        private int questionIndex;
        private int questionsAnswered;
        protected AdView mAdView;
        private ProgressBar progressAnswered;
        private Locale locale;
        private DateTime duration_start;
        List<TestHelper.Question> questions;
        private EditText editAnswer;
        private TextView textAnswer;
        private int ACTIVITY_SUMMARY = 300;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_test_input);

            mAdView = FindViewById<AdView>(Resource.Id.adView1);
            var adRequest = new AdRequest.Builder().Build();
            mAdView.LoadAd(adRequest);

            questions = JsonConvert.DeserializeObject<List<TestHelper.Question>>(Intent.GetStringExtra("questions"));
            foreach (TestHelper.Question question in questions)
                {
                    question.is_answered = false;
                }            

            editAnswer = FindViewById<EditText>(Resource.Id.editAnswer);
            textAnswer = FindViewById<TextView>(Resource.Id.textAnswer);

            FindViewById<Button>(Resource.Id.btnVerify).Click += btnVerify_Click;
            FindViewById<Button>(Resource.Id.btnNext).Click += btnNext_Click;
            FindViewById<Button>(Resource.Id.btnClose).Click += BtnClose_Click;

            questionIndex = 0;
            questionsAnswered = 0;
            progressAnswered = FindViewById<ProgressBar>(Resource.Id.progressAnswered);
            progressAnswered.Max = questions.Count();

            GenerateQuestion();

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            string locale_saved_name = prefs.GetString("localeName", "");
            var locales = await TextToSpeech.GetLocalesAsync();
            locale = locales.FirstOrDefault(l => l.Name == locale_saved_name);

            duration_start = DateTime.Now;
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            if (editAnswer.Text != "")
            {
                VerifyQuestion();
            }
            else
            {
                Toast.MakeText(this, "Musisz wpisać odpowiedź.", ToastLength.Short).Show();
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
                ShowSummary();
            }
        }

        private async void BtnClose_Click(object sender, EventArgs e)
        {
            var answer = await MessageHelper.MessageBoxQuestion.Show(this, "Potwierdź", "Czy chcesz przerwać utrwalanie i test?", "", "");
            if (answer == MessageHelper.MessageBoxQuestion.MessageBoxResult.Positive)
            {
                Close_Canceled();
            }
        }

        private void Close_Ok()
        {
            SetResult(Result.Ok);
            Finish();
        }

        private void Close_Canceled()
        {
            SetResult(Result.Canceled);
            Finish();
        }

        private void GenerateQuestion()
        {
            FindViewById<TextView>(Resource.Id.textQuestion).Text = questions[questionIndex].value;
            editAnswer.Text = "";
            textAnswer.Visibility = ViewStates.Invisible;
            textAnswer.Text = questions[questionIndex].answers.Single(s => s.is_correct == true).value;
            FindViewById<Button>(Resource.Id.btnVerify).Visibility = ViewStates.Visible;
            FindViewById<Button>(Resource.Id.btnNext).Visibility = ViewStates.Gone;
        }

        private async void VerifyQuestion()
        {
            FindViewById<Button>(Resource.Id.btnVerify).Visibility = ViewStates.Invisible;
            if (editAnswer.Text == textAnswer.Text)
            {
                textAnswer.SetTextColor(Android.Graphics.Color.Green);
                ++questionsAnswered;
                questions[questionIndex].is_answered = true;
                progressAnswered.Progress = questionsAnswered;
            }
            else
            {
                ++questions[questionIndex].wrong_inputs;
                ++questions[questionIndex].wrong_answers;
                textAnswer.SetTextColor(Android.Graphics.Color.Red);
            }
            
            textAnswer.Visibility = ViewStates.Visible;
            var settings = new SpeechOptions()
            {
                Locale = locale
            };
            await TextToSpeech.SpeakAsync(textAnswer.Text, settings);
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
            intent.PutExtra("parent_activity", "testinput");
            StartActivityForResult(intent, ACTIVITY_SUMMARY);
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

        }
    }
}