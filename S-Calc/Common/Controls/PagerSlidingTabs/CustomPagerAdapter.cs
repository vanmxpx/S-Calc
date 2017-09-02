using System.Collections.Generic;

using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using Java.Lang;
using Android.Runtime;
using System;
using System.Linq;
using S_Calc.Core;

namespace S_Calc.Common.Controls.PagerSlidingTabs
{
    public class CustomPagerAdapter : PagerAdapter
    {
        List<string> items = new List<string>();
        private PagerType state;

        public AdapterView.IOnItemClickListener ItemListener { get; set; }
        public CustomPagerAdapter(PagerType state, List<string> items) : base()
        {
            this.state = state;
            this.items = items;
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override bool IsViewFromObject(View view, Java.Lang.Object obj)
        {
            return view == obj;
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            View view = LayoutInflater.From(container.Context).Inflate(Resource.Layout.pager_item, container, false);
            container.AddView(view);


            ListView listView = view.FindViewById<ListView>(Resource.Id.listView);
            //int pos = position + 1;
            //txtTitle.Text = pos.ToString();

            if (state == PagerType.Functions)
            {
                if (position > 0)
                {
                    var list = Core.Controller.GetFunctionList((Core.FunctionType)(position - 1)).ToList();
                    var adapter = new CustomListAdapter<Core.Function>(container.Context, list);
                    listView.Adapter = adapter;
                }
            }
            else
            {
                var list = Core.Controller.GetConstantList((Core.ConstantType)position).ToList();
                var adapter = new CustomListAdapter<Core.Constant>(container.Context, list);
                listView.Adapter = adapter;
            }
            if (ItemListener != null)
                listView.OnItemClickListener = ItemListener;
            return view;
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(items[position]);
        }

        public new string GetPageTitle(int position)
        {
            return items[position];
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object obj)
        {
            container.RemoveView((View)obj);
        }
    }

    public enum PagerType { Functions, Constants }
}