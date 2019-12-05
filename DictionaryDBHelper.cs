using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using Environment = System.Environment;

namespace vocab_tester
{
    class DictionaryDBHelper
    {
        private SQLiteConnection db;

        [Table("Config")]
        public class Config
        {
            [PrimaryKey, Unique, MaxLength(40)]
            public string Name { get; set; }

            [MaxLength(40)]
            public string Value { get; set; }
            
        }
         
        [Table("Category")]
        public class Category
        {
            [PrimaryKey, AutoIncrement]
            public long Id { get; set; }
            [MaxLength(20)]
            public string Name { get; set; }
            public long? Parent_id { get; set; }

            public bool Is_sealed { get; set; }
        }

        [Table("Question")]
        public class Question
        {
            [PrimaryKey, AutoIncrement]
            public long Id { get; set; }
            [MaxLength(20)]
            public string Name { get; set; }
            public long Category_id { get; set; }
            public long Total_answers { get; set; }
            public long Wrong_answers { get; set; }
            public DateTime? Last_answered { get; set; }
            public bool Is_sealed { get; set; }
        }

        public class QuestionExt1: Question
        {                        
            public bool Category_Is_sealed { get; set; }
        }

        [Table("Answer")]
        public class Answer
        {
            [PrimaryKey, AutoIncrement]
            public long Id { get; set; }
            [MaxLength(20)]
            public string Value { get; set; }
            public long Question_id { get; set; }
        }

        [Table("TestHeader")]
        public class TestHeader
        {
            [PrimaryKey, AutoIncrement]
            public long Id { get; set; }            
            public DateTime Date { get; set; }
            public long Duration { get; set; }            
        }

        [Table("TestQuestion")]
        public class TestQuestion
        {
            [PrimaryKey, AutoIncrement]
            public long Id { get; set; }
            public long Test_id { get; set; }
            public long Question_id { get; set; }
            public long Wrong_answers { get; set; }            
        }

        public DictionaryDBHelper()
        {
            db = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dictionary.db3"));
            //db = new SQLiteConnection(Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "dictionary.db3"), SQLiteOpenFlags.Create, true);
            CreateTables();
        }

        #region db_utils

        private Config GetVersionRow()
        {
            return (from s in db.Table<Config>()
                    where s.Name.Equals("Version")
                    select s).FirstOrDefault();
        }

        private long GetLastKeyValue()
        {
            var cmd = db.CreateCommand("SELECT last_insert_rowid()", new object[] { });
            return cmd.ExecuteScalar<long>();

            /*
            cmd.CommandText = "SELECT Id FROM Question WHERE rowid=" + i.ToString();
            int res = cmd.ExecuteScalar<int>();
            return res;
            */
        }

        public string GetVersion()
        {            
            if (db.Table<Config>().Count() == 0)
            {
                var newConfig = new Config();
                newConfig.Name = "Version";
                newConfig.Value = "";
                db.Insert(newConfig);               
            }
            return GetVersionRow().Value;
        }

        public void SetVersion(string version)
        {
            Config versionRow = GetVersionRow();
            versionRow.Value = version;
            db.Update(versionRow);
        }

        public void ReCreateTables()
        {
            var cmd = db.CreateCommand("drop table Config");
            cmd.ExecuteNonQuery();
            cmd.CommandText = ("drop table Category");
            cmd.ExecuteNonQuery();
            cmd.CommandText = ("drop table Question");
            cmd.ExecuteNonQuery();
            cmd.CommandText = ("drop table Answer");
            cmd.ExecuteNonQuery();
            cmd.CommandText = ("drop table TestHeader");
            cmd.ExecuteNonQuery();
            cmd.CommandText = ("drop table TestQuestion");
            cmd.ExecuteNonQuery();
            CreateTables();
        }

        public void CreateTables()
        {
            db.CreateTable<Config>();
            db.CreateTable<Category>();
            db.CreateTable<Question>();
            db.CreateTable<Answer>();
            db.CreateTable<TestHeader>();
            db.CreateTable<TestQuestion>();
        }

        #endregion

        #region db_structure

        public long AddCategory(long? parent_id, string name, bool is_sealed)
        {            
            var db_row = (from s in db.Table<Category>()
                       where (s.Name.Equals(name)) && (s.Parent_id == parent_id || (parent_id == null && s.Parent_id == null))
                          select s).FirstOrDefault();
            if (db_row == null)
            {
                var newCategory = new Category();
                newCategory.Name = name;
                newCategory.Parent_id = parent_id;
                newCategory.Is_sealed = is_sealed;
                db.Insert(newCategory);
                return GetLastKeyValue();
            }
            else
            {
                return db_row.Id;
            }
        }

        public long AddQuestion(long category_id, string name, bool is_sealed)
        {
            var db_row = (from s in db.Table<Question>()
                          where s.Name.Equals(name) && s.Category_id.Equals(category_id)
                          select s).FirstOrDefault();
            if (db_row == null)
            {
                var newQuestion = new Question();
                newQuestion.Name = name;
                newQuestion.Category_id = category_id;
                newQuestion.Is_sealed = is_sealed;
                newQuestion.Total_answers = 0;
                newQuestion.Wrong_answers = 0;
                newQuestion.Last_answered = null;
                db.Insert(newQuestion);
                return GetLastKeyValue();
            }
            else
            {
                return db_row.Id;
            }
        }

        public long AddAnswer(long question_id, string value)
        {
            var db_row = (from s in db.Table<Answer>()
                          where s.Value.Equals(value) && s.Question_id.Equals(question_id)
                          select s).FirstOrDefault();
            if (db_row == null)
            {
                var newAnswer = new Answer();
                newAnswer.Value = value;
                newAnswer.Question_id = question_id;
                db.Insert(newAnswer);
                return GetLastKeyValue();
            }
            else
            {
                return db_row.Id;
            }
        }

        #endregion

        public List<Category> GetCategories(long? parent_id)
        {
            List<Category> categories = new List<Category>();
            var db_rows = (from s in db.Table<Category>()
                           where (s.Parent_id == parent_id || (parent_id == null && s.Parent_id == null))
                           select s);
            if (db_rows != null)
            {                
                foreach (Category category in db_rows)
                {
                    categories.Add(category);
                }
            }
            return categories;
        }

        public List<QuestionExt1> GetNewQuestions(int count, List<long> categories)
        {
            string cmd_str = "select Question.*, Category.Is_sealed Category_Is_sealed from Question " +
                "inner join Category on Category.Id = Question.Category_id " +
                "where Total_answers = 0 and " +
                "Category_id in (" + string.Join(",", categories.ToArray()) + ") " +
                "limit " + count.ToString();            
            var cmd = db.CreateCommand(cmd_str);
            return cmd.ExecuteQuery<QuestionExt1>();            
        }

        public List<Answer> GetAnswersForQuestion(long questionId)
        {
            string cmd_str = "select * from Answer where Question_id=" + questionId.ToString() + " order by Id";
            var cmd = db.CreateCommand(cmd_str);
            return cmd.ExecuteQuery<Answer>();
        }

        public List<Answer> GetAnswersForCategory(long categoryId, long questionId)
        {
            string cmd_str = "select Answer.* from Answer" +
                " inner join Question on Question.Id = Answer.Question_id where Question.Category_id=" + categoryId.ToString() +
                " and Question.Id <> " + questionId.ToString();
            var cmd = db.CreateCommand(cmd_str);
            return cmd.ExecuteQuery<Answer>();
        }
    }
}
 