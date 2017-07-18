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

namespace S_Calc.Common.fragments
{
    public class WorkflowFragment : Android.Support.V4.App.Fragment
    {
        private View _view;
        private FrameLayout _calcContainer;
        private FrameLayout _dragPanelContainer;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.Workflow, container, false);

            _calcContainer = _view.FindViewById<FrameLayout>(Resource.Id.calcContainer);
            _dragPanelContainer = _view.FindViewById<FrameLayout>(Resource.Id.dragPanelContainer);

            var trans = Activity.SupportFragmentManager.BeginTransaction();
            trans.Add(_calcContainer.Id, new CalcFragment(), "Calc");
            trans.Add(_dragPanelContainer.Id, new DragPanelFragment(), "DragPanel");
            trans.Commit();

            return _view;
        }
    }
}