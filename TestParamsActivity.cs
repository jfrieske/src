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
using Android.Widget;

namespace vocab_tester
{
    [Activity(Label = "TestParamsActivity")]
    public class TestParamsActivity : Activity
    {
        private NumberPicker npQuestions;
        private NumberPicker npAnswers;
        private ISharedPreferences prefs;
        private LinearLayout linearCategories;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_test_params);

            prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            npQuestions = (NumberPicker)FindViewById(Resource.Id.npQuestions);
            npQuestions.MinValue = 1;
            npQuestions.MaxValue = 99;
            npQuestions.Value = prefs.GetInt("npQuestions_Value", 10);
            npAnswers = (NumberPicker)FindViewById(Resource.Id.npAnswers);
            npAnswers.MinValue = 1;
            npAnswers.MaxValue = 5;
            npAnswers.Value = prefs.GetInt("npAnswers_Value", 3);

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
                BuildCategoryRow(category.Id, null, category.Name, 0, false);
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
                BuildCategoryRow(category.Id, category.Parent_id, category.Name, parents_count, false);
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
            linearOutside.Tag = string.Format("{0}_{1}", id, parent_id);
            linearOutside.SetGravity(GravityFlags.CenterVertical);

            ImageButton button = new ImageButton(this);
            button.Id = 1;
            button.SetImageResource(Resource.Drawable.minus_16);
            button.SetBackgroundColor(Android.Graphics.Color.Transparent);
            button.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            button.Tag = true;
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
            }
            else
            {
                ((ImageButton)sender).SetImageResource(Resource.Drawable.plus_16);
            }
        }

        private void CheckCategory_Click(object sender, EventArgs e)
        {
            //((CheckBox)sender).Checked = !((CheckBox)sender).Checked;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutInt("npQuestions_Value", npQuestions.Value);
            editor.PutInt("npAnswers_Value", npAnswers.Value);
            editor.Commit();
        }
    }
}


 