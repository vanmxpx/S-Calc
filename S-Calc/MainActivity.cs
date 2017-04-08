using Android.App;
using Android.OS;
using Android.Widget;
using Android.Views;
using RPNlib;
using Android.InputMethodServices;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using S_Calc.Controls;

namespace S_Calc
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {

        RPN_Real r;
        EditText Input, Output;
        string input => Input.Text;
        string _error;

        SupportToolbar _mToolBar;
        private KeyboardView _keyboardDigitalView;
        private KeyboardView _keyboardValuesView;
        private Keyboard _keyBoardDigital; 
        private Keyboard _keyBoardValues;
        private KeyboardListener _keyboardListener;

        private TabHost tabHost;
        //Menu
        private DrawerLayout _drawerLayout;
        private NavigationView _navigationView;
        private MainActionBarDrawerToggle _drawerToggle;

        string TAG = "Menu";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);


            // Setup Toolbar
            _mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolbar);

            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            _navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            _navigationView.NavigationItemSelected += (sender, e) =>
            {//make actions on menu item pressed
                e.MenuItem.SetChecked(true);
                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_home:
                        Log.Error(TAG, "nav_home");
                        break;
                    case Resource.Id.nav_messages:
                        Log.Error(TAG, "nav_messages");
                        break;
                }
            };

            SetSupportActionBar(_mToolBar);

            _drawerToggle = new MainActionBarDrawerToggle(this, _drawerLayout,
                Resource.String.openDrawer, Resource.String.closeDrawer);

            _drawerLayout.AddDrawerListener(_drawerToggle);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            _drawerToggle.SyncState();
            //

            Input = FindViewById<EditText>(Resource.Id.InputEditText);
            Input.ShowSoftInputOnFocus = false;
            Output = FindViewById<EditText>(Resource.Id.OutputEditText);

            CreateTabs();

            r = new RPN_Real();

            Input.TextChanged += _keyboardListener.OnInputTextChanged;
            Input.TextChanged += Input_TextChanged;
        }

        private void Input_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            Output.Text = $" = {r.ToString(input, ref _error)}";
        }

        public void ShowMessage(string message)
        {
            Toast.MakeText(this, message, ToastLength.Long).Show();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            _drawerToggle.OnOptionsItemSelected(item);
            return base.OnOptionsItemSelected(item);
        }

        private void CreateTabs()
        {
            tabHost = FindViewById<TabHost>(Resource.Id.tabHost);

            tabHost.Setup();

            _keyBoardDigital = new Keyboard(this, Resource.Xml.keyboard_digital);
            _keyboardDigitalView = FindViewById<KeyboardView>(Resource.Id.keyboard_digital_view);
            _keyboardDigitalView.Keyboard = _keyBoardDigital;
            _keyboardDigitalView.OnKeyboardActionListener = _keyboardListener = new KeyboardListener(this, Input, Output);
            _keyboardDigitalView.Visibility = ViewStates.Visible;
            _keyboardDigitalView.SetBackgroundColor(Android.Graphics.Color.Magenta);

            TabHost.TabSpec tabSpec = tabHost.NewTabSpec("tagDigitalKeyboard");
            tabSpec.SetContent(Resource.Id.linearLayoutTab1);
            tabSpec.SetIndicator("Digitals");
            tabHost.AddTab(tabSpec);

            _keyBoardValues = new Keyboard(this, Resource.Xml.keyboard_values);
            _keyboardValuesView = FindViewById<KeyboardView>(Resource.Id.keyboard_values_view);
            _keyboardValuesView.Keyboard = _keyBoardValues;
            _keyboardValuesView.OnKeyboardActionListener = _keyboardListener;
            _keyboardValuesView.Visibility = ViewStates.Visible;
            _keyboardValuesView.SetBackgroundColor(Android.Graphics.Color.Magenta);

            tabSpec = tabHost.NewTabSpec("tagValuesKeyboard");
            tabSpec.SetContent(Resource.Id.linearLayoutTab2);
            tabSpec.SetIndicator("Values");
            tabHost.AddTab(tabSpec);

            tabHost.CurrentTab = 0;
        }
    }
}