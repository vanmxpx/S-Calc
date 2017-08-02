using System;
using Android.App;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;

namespace S_Calc.Common.Controls
{
    class MainActionBarDrawerToggle : ActionBarDrawerToggle
    {
        private Activity _hostActivity;
        DrawerLayout _drawerLayout;
        private int _openedResource;
        private int _closedResource;


        public MainActionBarDrawerToggle(Activity host, DrawerLayout drawerLayout, int openedResource, int closedResource) 
            : base(host, drawerLayout, openedResource, closedResource)
        {
            this._drawerLayout = drawerLayout;
            this._hostActivity = host;
            this._openedResource = openedResource;
            this._closedResource = closedResource;
        }

        public override void OnDrawerOpened(View drawerView)
        {
            base.OnDrawerOpened(drawerView);
        }

        public override void OnDrawerClosed(View drawerView)
        {
            _drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            base.OnDrawerClosed(drawerView);
        }

        public override void OnDrawerSlide(View drawerView, float slideOffset)
        {
            base.OnDrawerSlide(drawerView, slideOffset);
        }
    }
}