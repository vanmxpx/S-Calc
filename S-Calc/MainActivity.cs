using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Android.Views;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportFragment = Android.Support.V4.App.Fragment;
using S_Calc.Controls;
using S_Calc.Resources.fragments;

namespace S_Calc
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme", WindowSoftInputMode = SoftInput.StateAlwaysHidden)]

public class MainActivity : AppCompatActivity
    {

        //Menu        
        private SupportToolbar _mToolBar;
        private DrawerLayout _drawerLayout;
        private MainActionBarDrawerToggle _drawerToggle;
        private NavigationView _navigationView;

        //Fragments
        private CalcFragment _calcFragment;
        private InfoFragment _infoFragment;
        private SupportFragment _currentFragment;
        private Stack<SupportFragment> _fragmentsStack;

        public static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            Instance = this;
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            //Setup Fragments
            _fragmentsStack = new Stack<SupportFragment>();
            var trans = SupportFragmentManager.BeginTransaction();
            if (SupportFragmentManager.FindFragmentByTag("Calc") != null)
            {
                _calcFragment = SupportFragmentManager.FindFragmentByTag("Calc") as CalcFragment;
                _infoFragment = SupportFragmentManager.FindFragmentByTag("About") as InfoFragment;
                trans.Hide(_infoFragment);
            }
            else
            {
                _infoFragment = new InfoFragment();
                trans.Add(Resource.Id.fragment_container, _infoFragment, "About");
                trans.Hide(_infoFragment);
                _calcFragment = new CalcFragment();
                trans.Add(Resource.Id.fragment_container, _calcFragment, "Calc");
            }
            trans.Commit();
            _currentFragment = _calcFragment;
            _fragmentsStack.Push(_currentFragment);

            // Setup Toolbar
            _mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(_mToolBar);
            //SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //Setup Drawer
            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _drawerToggle = new MainActionBarDrawerToggle(this, _drawerLayout,
                Resource.String.openDrawer, Resource.String.closeDrawer);
            _drawerLayout.AddDrawerListener(_drawerToggle);
            _drawerToggle.SyncState();

            _navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            _navigationView.NavigationItemSelected += OnNavigationItemSelected;

        }

        private void ShowFragment(SupportFragment fragment)
        {
            if (fragment.IsVisible) return;
            //TODO: Be added
            //trans.SetCustomAnimations();
            var trans = SupportFragmentManager.BeginTransaction();
            trans.Hide(_currentFragment);
            trans.Show(fragment);
            trans.AddToBackStack(null);
            trans.Commit();

            _fragmentsStack.Push(_currentFragment);
            _currentFragment = fragment;
        }

        public void ShowMessage(string message)
        {
            Toast.MakeText(this, message, ToastLength.Long).Show();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            _drawerToggle.OnOptionsItemSelected(item);
            int id = item.ItemId;
            if (id == Resource.Id.home)
            {
                if (SupportFragmentManager.BackStackEntryCount != 0)
                {
                    SupportFragmentManager.PopBackStack();
                    _currentFragment = _fragmentsStack.Pop();
                }
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public void OnNavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            //make actions on menu item pressed
            switch (e.MenuItem.ItemId)
            {
                case Resource.Id.nav_calc:
                    ShowFragment(_calcFragment);
                    break;
                case Resource.Id.nav_about:
                    ShowFragment(_infoFragment);
                    break;
                case Resource.Id.nav_exit:
                    Finish();
                    break;
            }
            _drawerLayout.CloseDrawers();
        }

        public override void OnBackPressed()
        {
            if (SupportFragmentManager.BackStackEntryCount != 0)
            {
                SupportFragmentManager.PopBackStack();
                _currentFragment = _fragmentsStack.Pop();
            }
            else
            {
                base.OnBackPressed();
            }
        }
    }
}