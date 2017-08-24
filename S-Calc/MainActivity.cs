using Android.App;
using Android.OS;
using Android.Widget;
using Android.Views;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using S_Calc.Common.Controls;
using S_Calc.Common.fragments;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportFragment = Android.Support.V4.App.Fragment;
using Android.Support.V4.View;
using S_Calc.Common;

namespace S_Calc
{
    [Activity(MainLauncher = true, Icon  = "@drawable/icon",   ScreenOrientation   = Android.Content.PM.ScreenOrientation.SensorPortrait,
                                   Theme = "@style/AppTheme",  WindowSoftInputMode = SoftInput.StateAlwaysHidden)]
    public class MainActivity : AppCompatActivity
    {
        //Menu        
        private SupportToolbar              _mToolBar;
        private DrawerLayout                _drawerLayout;
        private MainActionBarDrawerToggle   _drawerToggle;
        private NavigationView              _navigationView;

        //Fragments
        private WorkflowFragment            _workflowFragment;
        private InfoFragment                _infoFragment;
        private SupportFragment             _currentFragment;

        protected override void OnCreate(Bundle bundle)
        {
            Core.Controller.DoNothing();//<== ето пiзда 
            FontManager.SetDefaultFont(this, "DEFAULT", "awakelight.ttf");
            FontManager.SetDefaultFont(this, "MONOSPACE", "awakelight.ttf");
            Kernel.Activity = this;
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            //Setup Fragments
            var trans = SupportFragmentManager.BeginTransaction();
            if (SupportFragmentManager.FindFragmentByTag("Workflow") != null)
            {
                _workflowFragment = SupportFragmentManager.FindFragmentByTag("Workflow") as WorkflowFragment;
                _infoFragment = SupportFragmentManager.FindFragmentByTag("About") as InfoFragment;
                trans.Hide(_infoFragment);
            }
            else
            {
                _infoFragment = new InfoFragment();
                trans.Add(Resource.Id.fragment_container, _infoFragment, "About");
                trans.Hide(_infoFragment);
                _workflowFragment = new WorkflowFragment();
                trans.Add(Resource.Id.fragment_container, _workflowFragment, "Workflow");
            }
            trans.Commit();
            _currentFragment = _workflowFragment;

            // Setup Toolbar
            _mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(_mToolBar);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            //Setup Drawer
            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _drawerToggle = new MainActionBarDrawerToggle(this, _drawerLayout,
                Resource.String.openDrawer, Resource.String.closeDrawer);
            _drawerLayout.AddDrawerListener(_drawerToggle);
            _drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
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
            trans.Commit();

            _currentFragment = fragment;
        }

        public void ShowMessage(string message)
        {
            Toast.MakeText(this, message, ToastLength.Long).Show();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            
            _workflowFragment.HideWorkPanel();
            Kernel.Keyboard.HideCustomKeyboard();

            //TODO: Add custome buttons
            if (item.ItemId == Android.Resource.Id.Home)
            {
                _drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
            }
            _drawerToggle.OnOptionsItemSelected(item);
            return base.OnOptionsItemSelected(item);
        }

        public void OnNavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            //make actions on menu item pressed
            switch (e.MenuItem.ItemId)
            {
                case Resource.Id.nav_calc:
                    ShowFragment(_workflowFragment);
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
            if (_drawerLayout.IsDrawerOpen(GravityCompat.Start))
                _drawerLayout.CloseDrawer(GravityCompat.Start);
            else if (_workflowFragment.HideWorkPanel())
                return;
            else if (Kernel.Keyboard.Visible)
                Kernel.Keyboard.HideCustomKeyboard();
            else if (_currentFragment != _workflowFragment)
                ShowFragment(_workflowFragment);
            else
            {
                base.OnBackPressed();
            }
        }
    }
}