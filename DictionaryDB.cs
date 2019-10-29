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
    class DictionaryDB
    {
        private SQLiteConnection db;
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

        public void Open()
        {
            db = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dictionary.db3"));
            db.CreateTable<Category>();
        }
    }
}