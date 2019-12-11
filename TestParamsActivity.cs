using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace vocab_tester
{
    [Activity(Label = "TestParamsActivity")]
    public class TestParamsActivity : Activity
    {
        private NumberPicker npOldQuestions;
        private NumberPicker npNewQuestions;
        private ISharedPreferences prefs;
        private LinearLayout linearCategories;
        private int ACTIVITY_TEST = 300;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_test_params);

            prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            npOldQuestions = (NumberPicker)FindViewById(Resource.Id.npOldQuestions);
            npOldQuestions.MinValue = 0;
            npOldQuestions.MaxValue = 99;
            npOldQuestions.Value = prefs.GetInt("npOldQuestions_Value", 10);
            npNewQuestions = (NumberPicker)FindViewById(Resource.Id.npNewQuestions);
            npNewQuestions.MinValue = 0;
            npNewQuestions.MaxValue = 99;
            npNewQuestions.Value = prefs.GetInt("npNewQuestions_Value", 10);

            FindViewById<Button>(Resource.Id.btnClose).Click += BtnClose_Click;
            FindViewById<Button>(Resource.Id.btnTest).Click += BtnTest_Click;
            FindViewById<CheckBox>(Resource.Id.checkCategory_1).Click += CheckCategory_Click;
            FindViewById<CheckBox>(Resource.Id.checkCategory_2).Click += CheckCategory_Click;
            FindViewById<ImageButton>(Resource.Id.btnCategory_1).Click += BtnCategory_Click;
            FindViewById<ImageButton>(Resource.Id.btnCategory_1).Tag = false;
            FindViewById<ImageButton>(Resource.Id.btnCategory_2).Click += BtnCategory_Click;
            FindViewById<ImageButton>(Resource.Id.btnCategory_2).Tag = false;
            linearCategories = FindViewById<LinearLayout>(Resource.Id.linearCategories);
            GenerateCategoryTree();
        }

        private void GenerateCategoryTree()
        {
            DictionaryDBHelper dbHelper = new DictionaryDBHelper();
            foreach (DictionaryDBHelper.Category category in dbHelper.GetCategories(null))
            {
                BuildCategoryRow(category.Id, null, category.Name, 0, category.Is_checked);
                if (!GenerateSubcategories(dbHelper, category.Id, 0))
                {
                    HideExpandButton(category.Id);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="parent_id"></param>
        /// <returns>true jeśli są podkategorie</returns>
        private bool GenerateSubcategories(DictionaryDBHelper dbHelper, long parent_id, int parents_count)
        {
            ++parents_count;
            bool hasChildren = false;
            foreach (DictionaryDBHelper.Category category in dbHelper.GetCategories(parent_id))
            {
                BuildCategoryRow(category.Id, category.Parent_id, category.Name, parents_count, category.Is_checked);
                if (!GenerateSubcategories(dbHelper, category.Id, parents_count))
                {
                    HideExpandButton(category.Id);
                }
                hasChildren = true;
            }
            return hasChildren;
        }

        private void BuildCategoryRow(long id, long? parent_id, string name, int parents_count, bool is_checked)
        {
            float d = Resources.DisplayMetrics.Density;

            LinearLayout linearOutside = new LinearLayout(this); //linearCategory_1
            linearOutside.Id = (int)id;
            linearOutside.Orientation = Orientation.Horizontal;
            linearOutside.SetMinimumWidth(25);
            linearOutside.SetMinimumHeight(20);
            int dpValue = parents_count * 9; // margin in dips            
            int left_margin = (int)(dpValue * d); // margin in pixels
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            layoutParams.SetMargins(left_margin, 0, 0, 0);
            linearOutside.LayoutParameters = layoutParams;
            linearOutside.Tag = parent_id;
            linearOutside.SetGravity(GravityFlags.CenterVertical);

            ImageButton button = new ImageButton(this);
            button.Id = 1;
            button.SetImageResource(Resource.Drawable.minus_16);
            button.SetBackgroundColor(Android.Graphics.Color.Transparent);
            button.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            button.Tag = true; //expanded
            button.Click += BtnCategory_Click;            
            linearOutside.AddView(button);

            LinearLayout linearInsideGroup = new LinearLayout(this);
            linearInsideGroup.Orientation = Orientation.Horizontal;
            LinearLayout.LayoutParams layoutParams2 = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            layoutParams2.SetMargins((int)(5 * Resources.DisplayMetrics.Density), 0, 0, 0);
            linearInsideGroup.LayoutParameters = layoutParams2;
            LinearLayout linearTextView = new LinearLayout(this);
            linearTextView.Orientation = Orientation.Horizontal;
            LinearLayout.LayoutParams layoutTextGroupParams = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.MatchParent);
            layoutTextGroupParams.SetMargins((int)(6 * Resources.DisplayMetrics.Density), 0, 0, 0);
            layoutTextGroupParams.Weight = 1;
            linearTextView.LayoutParameters = layoutTextGroupParams;
            linearTextView.SetGravity(GravityFlags.CenterVertical);
            TextView textView = new TextView(this);
            textView.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            textView.Text = name;
            linearTextView.AddView(textView);
            linearInsideGroup.AddView(linearTextView);

            LinearLayout linearCheckBox = new LinearLayout(this);
            linearCheckBox.Orientation = Orientation.Horizontal;
            linearCheckBox.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.MatchParent);
            CheckBox checkBox = new CheckBox(this);
            checkBox.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            checkBox.Checked = is_checked;
            checkBox.Click += CheckCategory_Click;
            linearCheckBox.AddView(checkBox);

            linearInsideGroup.AddView(linearCheckBox);
            linearOutside.AddView(linearInsideGroup);
            linearCategories.AddView(linearOutside);

        }
        
        private void HideExpandButton(long category_id)
        {
            LinearLayout linearOutside = linearCategories.FindViewById<LinearLayout>((int)category_id);
            linearOutside.FindViewById<ImageButton>(1).Visibility = ViewStates.Invisible;
        }

        private void BtnCategory_Click(object sender, EventArgs e)
        {
            ((ImageButton)sender).Tag = !Convert.ToBoolean(((ImageButton)sender).Tag);
            if (Convert.ToBoolean(((ImageButton)sender).Tag))
            {
                ((ImageButton)sender).SetImageResource(Resource.Drawable.minus_16);
                Category_Expand(((LinearLayout)(((ImageButton)sender).Parent)).Id);
            }
            else
            {
                ((ImageButton)sender).SetImageResource(Resource.Drawable.plus_16);
                Category_Collapse(((LinearLayout)(((ImageButton)sender).Parent)).Id);
            }
        }

        private void Category_Collapse(long category_id)
        {
            List<LinearLayout> children = ActivityHelper.GetLinearChildren(linearCategories, category_id);
            foreach(LinearLayout child in children)
            {
                child.Visibility = ViewStates.Gone;
                Category_Collapse((long)child.Id);                
            }
        }

        private void Category_Expand(long category_id)
        {
            List<LinearLayout> children = ActivityHelper.GetLinearChildren(linearCategories, category_id);
            foreach (LinearLayout child in children)
            {
                child.Visibility = ViewStates.Visible;
                if (Convert.ToBoolean(child.FindViewById<ImageButton>(1).Tag))
                {
                    Category_Expand((long)child.Id);
                }
            }
        }

        private void CheckCategory_Click(object sender, EventArgs e)
        {
            LinearLayout linearOutside = (LinearLayout)((CheckBox)sender).Parent.Parent.Parent;
            Category_Check(linearOutside.Id, ((CheckBox)sender).Checked);
        }

        private void Category_Check(long parent_id, bool is_checked)
        {
            List<LinearLayout> children = ActivityHelper.GetLinearChildren(linearCategories, parent_id);
            foreach (LinearLayout child in children)
            {
                CheckBox checkBox = (CheckBox)((LinearLayout)((LinearLayout)child.GetChildAt(1)).GetChildAt(1)).GetChildAt(0);
                checkBox.Checked = is_checked;
                Category_Check((long)child.Id, is_checked);
            }
        }

        private List<long> Category_Get_Checked()
        {
            List<long> checked_ids = new List<long>();
            int childCount = linearCategories.ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                LinearLayout child = (LinearLayout)linearCategories.GetChildAt(i);
                CheckBox checkBox = (CheckBox)((LinearLayout)((LinearLayout)child.GetChildAt(1)).GetChildAt(1)).GetChildAt(0);
                if (checkBox.Checked)
                {
                    checked_ids.Add(child.Id);
                }
            }
            return checked_ids;            
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutInt("npOldQuestions_Value", npOldQuestions.Value);
            editor.PutInt("npNewQuestions_Value", npNewQuestions.Value);
            editor.Commit();

            List<long> checked_ids = Category_Get_Checked();

            DictionaryDBHelper dbHelper = new DictionaryDBHelper();
            dbHelper.UncheckCategories();
            dbHelper.CheckCategories(checked_ids);

            var intent = new Intent(this, typeof(TestActivity));
            Bundle bundle = new Bundle();
            bundle.PutInt("oldQuestions", npOldQuestions.Value);
            bundle.PutInt("newQuestions", npNewQuestions.Value);
            bundle.PutLongArray("categories", checked_ids.ToArray());
            intent.PutExtra("testParams", bundle);
            StartActivityForResult(intent, ACTIVITY_TEST);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == ACTIVITY_TEST)
            {
                if (resultCode == Result.Ok)
                {

                }
                if (resultCode == Result.FirstUser)
                {
                    
                }
            }
            
        }
    }
}


 