using Android.OS;
using Android.Views;

namespace S_Calc.Common.fragments
{
    public class InfoFragment : Android.Support.V4.App.Fragment
    {
        private View _view;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View _view = inflater.Inflate(Resource.Layout.Info, container, false);
            return _view;
        }
    }
}