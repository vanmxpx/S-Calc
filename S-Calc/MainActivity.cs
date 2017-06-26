using Android.App;
using Android.OS;
using Android.Widget;
using Android.Views;
using Android.InputMethodServices;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using S_Calc.Controls;
using RPNClassLibraryCSharp;

namespace S_Calc
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        EditText Input, Output;
        private string input => Input.Text;

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

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);


            // Setup Toolbar
            _mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolbar);

            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            SetSupportActionBar(_mToolBar);

            _drawerToggle = new MainActionBarDrawerToggle(this, _drawerLayout,
                Resource.String.openDrawer, Resource.String.closeDrawer);

            _drawerLayout.AddDrawerListener(_drawerToggle);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            _drawerToggle.SyncState();
            //

            _navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            var listner = new MenuItemListener(this);
            _navigationView.SetNavigationItemSelectedListener(this);
            //make actions on menu item pressed

            Input = FindViewById<EditText>(Resource.Id.InputEditText);
            Input.ShowSoftInputOnFocus = false;
            Output = FindViewById<EditText>(Resource.Id.OutputEditText);

            CreateTabs();

            Input.TextChanged += _keyboardListener.OnInputTextChanged;
            Input.TextChanged += Input_TextChanged;
        }

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            //make actions on menu item pressed
            menuItem.SetChecked(true);
            switch (menuItem.ItemId)
            {
                case Resource.Id.nav_home:
                    Toast.MakeText(this, "home", ToastLength.Long).Show();
                    return true;
                case Resource.Id.nav_messages:
                    Toast.MakeText(this, "nav_messages", ToastLength.Long).Show();
                    return true;
            }
            return false;
        }

        private void Input_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            string output;
            bool error;
            Controller.Evaluate(input, out output, out error);
            Output.Text = $" = {output}";
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