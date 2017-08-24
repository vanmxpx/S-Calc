using System;
using System.Collections.Generic;

using Android.App;
using Android.Views;
using Android.Widget;
using Android.Content;
using S_Calc.Core;
using Java.Lang;

namespace S_Calc.Common.Controls.PagerSlidingTabs
{
    public class CustomListAdapter<T> : BaseAdapter<T> where T : ICalculatorUnit
    {
        private List<T> items;
        private Context context;

        public override int Count => items.Count;
        public override T this[int position] => items[position];


        public CustomListAdapter(Context context, List<T> items)
       : base()
        {
            this.context = context;
            this.items = items;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = LayoutInflater.From(context).Inflate(Resource.Layout.list_item, null);
            view.FindViewById<TextView>(Resource.Id.text_view_name).Text = item.Name;
            view.FindViewById<TextView>(Resource.Id.text_view_descr).Text = item.Description;
            return view;
        }
    }
}