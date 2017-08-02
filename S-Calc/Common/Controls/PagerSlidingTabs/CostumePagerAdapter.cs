using System.Collections.Generic;

using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using Java.Lang;

namespace S_Calc.Common.Controls.PagerSlidingTabs
{
    public class CostumePagerAdapter : PagerAdapter
    {
        List<string> items = new List<string>();

        public CostumePagerAdapter(List<string> items) : base()
        {
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

            TextView txtTitle = view.FindViewById<TextView>(Resource.Id.item_title);
            int pos = position + 1;
            txtTitle.Text = pos.ToString();

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
}