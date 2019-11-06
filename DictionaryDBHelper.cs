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
            public int Id { get; set; }
            [MaxLength(20)]
            public string Name { get; set; }
            public long? Parent_id { get; set; }
        }

        [Table("Question")]
        public class Question
        {
            [PrimaryKey, AutoIncrement]
            public long Id { get; set; }
            [MaxLength(20)]
            public string Name { get; set; }
            public long Category_id { get; set; }
        }

        public DictionaryDBHelper()
        {
            db = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dictionary.db3"));
            //db = new SQLiteConnection(Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "dictionary.db3"), SQLiteOpenFlags.Create, true);
            db.CreateTable<Config>();
            db.CreateTable<Category>();
            db.CreateTable<Question>();

            //Truncate();
        }

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

        public void Truncate()
        {
            var cmd = db.CreateCommand("delete from Category");
            cmd.ExecuteNonQuery();
            cmd.CommandText = ("delete from Question");
            cmd.ExecuteNonQuery();
        }

        public long AddCategory(long? parent_id, string name)
        {            
            var db_row = (from s in db.Table<Category>()
                       where (s.Name.Equals(name)) && (s.Parent_id == parent_id || (parent_id == null && s.Parent_id == null))
                          select s).FirstOrDefault();
            if (db_row == null)
            {
                var newCategory = new Category();
                newCategory.Name = name;
                newCategory.Parent_id = parent_id;
                db.Insert(newCategory);
                return GetLastKeyValue();
            }
            else
            {
                return db_row.Id;
            }
        }

        public long AddQuestion(long category_id, string name)
        {
            var db_row = (from s in db.Table<Question>()
                          where s.Name.Equals(name) && s.Category_id.Equals(category_id)
                          select s).FirstOrDefault();
            if (db_row == null)
            {
                var newQuestion = new Question();
                newQuestion.Name = name;
                newQuestion.Category_id = category_id;
                db.Insert(newQuestion);
                return GetLastKeyValue();
            }
            else
            {
                return db_row.Id;
            }
        }

        public void GetCategories(long? parent_id, string name)
        {
            var db_rows = (from s in db.Table<Category>()                          
                          select s);            
        }

        
    }
}
 