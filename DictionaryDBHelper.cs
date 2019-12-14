using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            public bool Is_checked { get; set; }
        }

        [Table("Question")]
        public class Question
        {
            [PrimaryKey, AutoIncrement]
            public long Id { get; set; }
            [MaxLength(20)]
            public string Name { get; set; }
            public long Category_id { get; set; }
            public long Total_queries { get; set; }
            public long Wrong_answers { get; set; }
            public DateTime? Last_answered { get; set; }
            public bool Is_sealed { get; set; }
        }

        public class QuestionExt1 : Question
        {
            public bool Category_Is_sealed { get; set; }
            public bool Is_old { get; set; }
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

        [Table("Stats")]
        public class Stats
        {
            [PrimaryKey, AutoIncrement]
            public long Id { get; set; }
            public long Old_questions_total { get; set; }
            public long Old_questions_answer_ratio { get; set; }
            public long New_questions_total { get; set; }
            public long New_questions_answer_ratio { get; set; }
            public DateTime Date { get; set; }
            public long Duration { get; set; }
        }

        [Table("Stats_question")]
        public class Stats_question
        {
            [PrimaryKey, AutoIncrement]
            public long Id { get; set; }            
            public long Stats_id { get; set; }
            public long Question_id { get; set; }
            public long Wrong_answers { get; set; }
            public bool Is_old_question { get; set; }
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
                var newConfig = new Config
                {
                    Name = "Version",
                    Value = ""
                };
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
            cmd.CommandText = ("drop table Stats");
            cmd.ExecuteNonQuery();
            cmd.CommandText = ("drop table Stats_question");
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
            db.CreateTable<Stats>();
            db.CreateTable<Stats_question>();
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
                var newCategory = new Category
                {
                    Name = name,
                    Parent_id = parent_id,
                    Is_sealed = is_sealed,
                    Is_checked = false
                };
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
                var newQuestion = new Question
                {
                    Name = name,
                    Category_id = category_id,
                    Is_sealed = is_sealed,
                    Total_queries = 0,
                    Wrong_answers = 0,
                    Last_answered = null
                };
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
                var newAnswer = new Answer
                {
                    Value = value,
                    Question_id = question_id
                };
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

        public void UncheckCategories()
        {
            var cmd = db.CreateCommand("update Category set Is_checked = false");
            cmd.ExecuteNonQuery();
        }

        public void CheckCategories(List<long> categories)
        {
            var cmd = db.CreateCommand("update Category set Is_checked = true" +
                " where Id in (" + string.Join(",", categories.ToArray()) + ")");
            cmd.ExecuteNonQuery();
        }

        public List<QuestionExt1> GetNewQuestions(int count, List<long> categories)
        {
            string cmd_str = "select Question.*, Category.Is_sealed Category_Is_sealed, 0 Is_old from Question" +
                " inner join Category on Category.Id = Question.Category_id" +
                " where Total_queries = 0 and" +
                " Category_id in (" + string.Join(",", categories.ToArray()) + ")" +
                " ORDER BY RANDOM() limit " + count.ToString();
            var cmd = db.CreateCommand(cmd_str);
            return cmd.ExecuteQuery<QuestionExt1>();
        }

        public List<QuestionExt1> GetOldQuestions(int count, List<long> categories)
        {
            string cmd_str = "select Question.*, Category.Is_sealed Category_Is_sealed, 1 Is_old from Question" +
                " inner join Category on Category.Id = Question.Category_id" +
                " where Total_queries > 0 and" +
                " Category_id in (" + string.Join(",", categories.ToArray()) + ")" +
                " ORDER BY Last_answered desc limit " + count.ToString();
            var cmd = db.CreateCommand(cmd_str);
            return cmd.ExecuteQuery<QuestionExt1>();
        }

        public Answer GetAnswerForQuestion(long questionId)
        {
            string cmd_str = "select * from Answer where Question_id=" + questionId.ToString() + " order by Id LIMIT 1";
            var cmd = db.CreateCommand(cmd_str);
            return cmd.ExecuteQuery<Answer>().FirstOrDefault();
        }

        public List<Answer> GetAnswersForSealedQuestion(long questionId, long answerToSkip)
        {
            string cmd_str = "select * from Answer where Id <> " + answerToSkip + " and Question_id=" + questionId.ToString();
            var cmd = db.CreateCommand(cmd_str);
            return cmd.ExecuteQuery<Answer>();
        }

        public List<Answer> GetAnswersForSealedCategory(int count, long categoryId, long answerToSkip)
        {
            string cmd_str = "select Answer.* from Answer" +
                " inner join Question on Question.Id = Answer.Question_id where Question.Category_id=" + categoryId.ToString() +
                " and Answer.Id <> " + answerToSkip.ToString() +
                " ORDER BY RANDOM() LIMIT " + count.ToString();
            var cmd = db.CreateCommand(cmd_str);
            return cmd.ExecuteQuery<Answer>();
        }

        public List<Answer> GetAnswersForAllCategories(int count, long answerToSkip)
        {
            string cmd_str = "select Answer.* from Answer" +
                " inner join Question on Question.Id = Answer.Question_id" +
                " inner join Category on Category.Id = Question.Category_id" +
                " where not Question.Is_sealed and not Category.Is_sealed" +
                " and Answer.Id <> " + answerToSkip.ToString() +
                " ORDER BY RANDOM() LIMIT " + count.ToString();
            var cmd = db.CreateCommand(cmd_str);
            return cmd.ExecuteQuery<Answer>();
        }

        public List<Answer> GetAnswersForCategories(int count, List<long> categories, long answerToSkip)
        {
            string cmd_str = "select Answer.* from Answer" +
                " inner join Question on Question.Id = Answer.Question_id" +
                " inner join Category on Category.Id = Question.Category_id" +
                " where Category_id in (" + string.Join(",", categories.ToArray()) + ")" +
                " and Answer.Id <> " + answerToSkip.ToString() +
                " ORDER BY RANDOM() LIMIT " + count.ToString();
            var cmd = db.CreateCommand(cmd_str);
            return cmd.ExecuteQuery<Answer>();
        }

        public void UpdateQuestionStats(long id, long wrong_answers)
        {
            var cmd = db.CreateCommand(String.Format("update Question set " +
                "Total_queries = Total_queries + 1, " +
                "Wrong_answers = Wrong_answers + {1}, " +
                "Last_answered = datetime('now') " +
                "where Id = {0}", id, wrong_answers));
            cmd.ExecuteNonQuery();
        }

        #region Stats

        public long AddStats(long old_questions_total, long old_questions_answer_ratio, long new_questions_total,
                             long new_questions_answer_ratio, long duration)
        {
            var newStats = new Stats
            {
                Old_questions_total = old_questions_total,
                Old_questions_answer_ratio = old_questions_answer_ratio,
                New_questions_total = new_questions_total,
                New_questions_answer_ratio = new_questions_answer_ratio,
                Date = DateTime.Now,
                Duration = duration
            };

            db.Insert(newStats);
            return GetLastKeyValue();
        }

        public void AddStatsQuestion(long stats_id, long question_id, long wrong_answers, bool is_old_question)
        {
            var newStatsQuestion = new Stats_question
            {
                Stats_id = stats_id,
                Question_id = question_id,
                Wrong_answers = wrong_answers,
                Is_old_question = is_old_question
            };
            db.Insert(newStatsQuestion);
        }

        #endregion
    }
}
 