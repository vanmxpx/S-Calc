using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using S_Calc.Common.Controls.PagerSlidingTabs;

namespace S_Calc.Common.fragments
{
    public class DragPanelFragment : Android.Support.V4.App.Fragment
    {
        private View _view;
        private ViewPager _viewPager;

        private PagerSlidingTabStrip _slidingTabScrollView;

        public event EventHandler DragButtonClick;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.DragPanel, container, false);
            
            _slidingTabScrollView = _view.FindViewById<PagerSlidingTabStrip>(Resource.Id.sliding_tabs);
            _viewPager = _view.FindViewById<ViewPager>(Resource.Id.viewpager);
            _viewPager.Adapter = new CostumePagerAdapter(new List<string> {"Favorites","My", "Tryhonometric", "Hyperbolic", "Common"});
            _view.FindViewById<Button>(Resource.Id.dragButton).Click += InvokeDragButtonClick;
            _slidingTabScrollView.SetViewPager(_viewPager);

            return _view;
        }

        private void InvokeDragButtonClick(object sender, EventArgs e)
        {
            DragButtonClick?.Invoke(sender, e);
        }
    }
}