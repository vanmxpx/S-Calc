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
using Android.Support.V4.Content;
using Android.Graphics;

namespace S_Calc.Common.fragments
{
    public class DragPanelFragment : Android.Support.V4.App.Fragment
    {
        private View _view;
        private ViewPager _viewPager;

        private PagerSlidingTabStrip _slidingTabScrollView;
        private CustomPagerAdapter _functionAdapter;
        private CustomPagerAdapter _constAdapter;

        public PagerType PagerState { get; private set; }
        public AdapterView.IOnItemClickListener Listener { get; set; }

        private Button _functionsButton;
        private Button _constantsButton;

        private Color _activeButColor;
        private Color _unactiveButColor;

        public event EventHandler DragButtonClick;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _functionAdapter    = new CustomPagerAdapter(PagerType.Functions, new List<string>(Resources.GetStringArray(Resource.Array.func_types)));
            _constAdapter       = new CustomPagerAdapter(PagerType.Constants, new List<string>(Resources.GetStringArray(Resource.Array.const_types)));

            _activeButColor     = new Color(ContextCompat.GetColor(Context, Resource.Color.but_panel_state_active));
            _unactiveButColor   = new Color(ContextCompat.GetColor(Context, Resource.Color.but_panel_state_unactive));

            PagerState          = PagerType.Functions;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.DragPanel, container, false);
            
            //Pages
            _slidingTabScrollView = _view.FindViewById<PagerSlidingTabStrip>(Resource.Id.sliding_tabs);
            _viewPager = _view.FindViewById<ViewPager>(Resource.Id.viewpager);
            _viewPager.Adapter = PagerState == PagerType.Functions ? _functionAdapter : _constAdapter;
            _slidingTabScrollView.SetViewPager(_viewPager);

            _functionsButton = _view.FindViewById<Button>(Resource.Id.functionsButton);
            _constantsButton = _view.FindViewById<Button>(Resource.Id.constantsButton);

            _functionAdapter.ItemListener = Listener;
            _constAdapter.ItemListener = Listener;

            _view.FindViewById<Button>(Resource.Id.dragButton).Click += InvokeDragButtonClick;
            _functionsButton.Click += OnFuncButtonClick;
            _constantsButton.Click += OnConstButtonClick;

            return _view;
        }

        private void OnConstButtonClick(object sender, EventArgs e)
        {
            if (PagerState != PagerType.Constants)
            {
                PagerState = PagerType.Constants;
                _viewPager.Adapter = _constAdapter;
                _functionsButton.SetTextColor(_unactiveButColor);
                _constantsButton.SetTextColor(_activeButColor);
                _slidingTabScrollView.NotifyDataSetChanged();
            }
        }

        private void OnFuncButtonClick(object sender, EventArgs e)
        {
            if (PagerState != PagerType.Functions)
            {
                PagerState = PagerType.Functions;
                _viewPager.Adapter = _functionAdapter;
                _functionsButton.SetTextColor(_activeButColor);
                _constantsButton.SetTextColor(_unactiveButColor);
                _slidingTabScrollView.NotifyDataSetChanged();
            }
        }

        private void InvokeDragButtonClick(object sender, EventArgs e)
        {
            DragButtonClick?.Invoke(sender, e);
        }
    }
}