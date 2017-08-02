using Android.Views;

namespace S_Calc.Common.Controls.PagerSlidingTabs
{
    public interface ICustomTabProvider
    {
        View GetCustomTabView(ViewGroup parent, int position);
    }
}

