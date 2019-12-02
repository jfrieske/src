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
    public static class ActivityHelper
    {
        public static List<LinearLayout> GetLinearChildren(LinearLayout root, long parent_id)
        {
            List<LinearLayout> views = new List<LinearLayout>();
            int childCount = root.ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                View child = root.GetChildAt(i);                
                Object tagObj = child.Tag;
                if (tagObj != null)
                {
                    if (Convert.ToInt64(tagObj) == parent_id)
                    {
                        views.Add((LinearLayout)child);
                    }
                }
            }
            return views;
        }
    }
}
