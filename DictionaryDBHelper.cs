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
            [Unique, MaxLength(40)]
            public string Unique_name { get; set; }
            public int Parent_id { get; set; }
        }

        public DictionaryDBHelper()
        {
            db = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dictionary.db3"));
            //db = new SQLiteConnection(Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "dictionary.db3"), SQLiteOpenFlags.Create, true);
        }

        private Config GetVersionRow()
        {
            return (from s in db.Table<Config>()
                    where s.Name.Equals("Version")
                    select s).FirstOrDefault();
        }

        public string GetVersion()
        {
            db.CreateTable<Config>();
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
    }
}