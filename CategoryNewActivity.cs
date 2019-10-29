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
    [Activity(Label = "CategoryNewActivity")]
    public class CategoryNewActivity : Activity
    {
        private List<KeyValuePair<string, int>> parents;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_category_new);

            PopulateParents();
            FindViewById<Button>(Resource.Id.btnClose).Click += BtnClose_Click;
            FindViewById<Button>(Resource.Id.btnCategoryAdd).Click += BtnCategoryAdd_Click;
        }

        private void BtnCategoryAdd_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void PopulateParents()
        {
            Spinner spinner = FindViewById<Spinner>(Resource.Id.spinParent);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_ItemSelected);
            parents = new List<KeyValuePair<string, int>>
                {
                    new KeyValuePair<string, int>("brak", -1),
                    new KeyValuePair<string, int>("angielski", 1),
                    new KeyValuePair<string, int>("angielski.gramatyka", 2),
                    new KeyValuePair<string, int>("angielski.gramatyka.odmiana there is", 3)
                };

            List<string> parentNames = new List<string>();
            foreach (var item in parents)
                parentNames.Add(item.Key);
            var adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, parentNames);
            adapter.SetDropDownViewResource(Resource.Layout.spinner_item);
            spinner.Adapter = adapter;
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string toast = string.Format("The mean temperature for planet {0} is {1}",
                spinner.GetItemAtPosition(e.Position), parents[e.Position].Value);
            Toast.MakeText(this, toast, ToastLength.Long).Show();
        }
    }
}