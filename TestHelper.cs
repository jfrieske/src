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
    class TestHelper
    {
        public class Question
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
            public bool is_old;
            public List<Answer> answers;

            public Question(long id, string name, bool is_old)
            {
                this.id = id;
                value = name;
                this.is_old = is_old;
                wrong_answers = 0;
                is_answered = false;
                answers = new List<Answer>();
            }

            public void AddAnswer(string name, bool is_correct)
            {
                answers.Add(new Answer { value = name, is_correct = is_correct });
            }
        }
    }
}