using System;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Android.Content;

namespace S_Calc.Controls
{
    class MenuItemListener : NavigationView.IOnNavigationItemSelectedListener
    {
        public IntPtr Handle { get; set; }
        private Context _owner;

        public MenuItemListener(Context _owner)
        {
            this._owner = _owner;
        }

        public void Dispose()
        {
            Handle = IntPtr.Zero;
        }

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            //make actions on menu item pressed
            menuItem.SetChecked(true);
            switch (menuItem.ItemId)
            {
                case Resource.Id.nav_home:
                    Toast.MakeText(_owner, "home", ToastLength.Long).Show();
                    return true;
                case Resource.Id.nav_messages:
                    Toast.MakeText(_owner, "nav_messages", ToastLength.Long).Show();
                    return true;
            }
            return false;
        }
    }
}