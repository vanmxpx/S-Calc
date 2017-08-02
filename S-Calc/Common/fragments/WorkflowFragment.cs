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
using S_Calc.Common.Controls;

namespace S_Calc.Common.fragments
{
    public class WorkflowFragment : Android.Support.V4.App.Fragment
    { 
        private View _view;

        //Layouts
        private FrameLayout _calcContainer;
        private FrameLayout _dragPanelContainer;

        private FrameWithDragPanelLayout _frameWithDragPanel;
        //Fragments
        private CalcFragment _calcFragment;
        private DragPanelFragment _panelFragment;

        public override void OnCreate(Bundle savedInstanceState)
        {
            _calcFragment  = new CalcFragment();
            _panelFragment = new DragPanelFragment();
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.Workflow, container, false);
            _frameWithDragPanel = _view.FindViewById<FrameWithDragPanelLayout>(Resource.Id.frameWithDragPanel);
            _calcContainer      = _view.FindViewById<FrameLayout>(Resource.Id.calcContainer);
            _dragPanelContainer = _view.FindViewById<FrameLayout>(Resource.Id.dragPanelContainer);

            _panelFragment.DragButtonClick += OnDragButtonClick;

            var trans = Activity.SupportFragmentManager.BeginTransaction();
            trans.Add(_calcContainer.Id,      _calcFragment,  "Calc");
            trans.Add(_dragPanelContainer.Id, _panelFragment, "DragPanel");
            trans.Commit();

            return _view;
        }

        public bool ShowWorkPanel()
        {
            if (_frameWithDragPanel.PanelState)
                return false;

            _frameWithDragPanel.Open();
            return true;
        }
        public bool HideWorkPanel()
        {
            if (!_frameWithDragPanel.PanelState)
                return false;

            _frameWithDragPanel.Close();
            return true;
        }

        private void OnDragButtonClick(object sender, EventArgs e)
        {
            if (_frameWithDragPanel.PanelState) _frameWithDragPanel.Close();
            else _frameWithDragPanel.Open();
        }

    }
}