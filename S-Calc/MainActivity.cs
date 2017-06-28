using Android.App;
using Android.Content;
using Android.Hardware.Input;
using Android.OS;
using Android.Widget;
using Android.Views;
using Android.InputMethodServices;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Views.InputMethods;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using S_Calc.Controls;
using RPNClassLibraryCSharp;
using S_Calc.Resources.fragments;

namespace S_Calc
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        //EditText Input, Output;
        //private string input => Input.Text;

        SupportToolbar _mToolBar;
        //private KeyboardView _keyboardDigitalView;
        //private KeyboardView _keyboardValuesView;
        //private Keyboard _keyBoardDigital; 
        //private Keyboard _keyBoardValues;
        //private KeyboardListener _keyboardListener;
        //private TabHost tabHost;
        //Menu
        private DrawerLayout _drawerLayout;
        private NavigationView _navigationView;
        private MainActionBarDrawerToggle _drawerToggle;
        private CalcFragment _calcFragment;
        private InfoFragment _infoFragment;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);


            // Setup Toolbar
            _mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(_mToolBar);

            _calcFragment = new CalcFragment(this);
            _infoFragment = new InfoFragment();
            var trans = SupportFragmentManager.BeginTransaction();
            trans.Add(Resource.Id.fragment_container, _calcFragment, "About");
            trans.Hide(_infoFragment);
            trans.Add(Resource.Id.fragment_container, _infoFragment, "Calc");
            trans.Commit();

            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _drawerToggle = new MainActionBarDrawerToggle(this, _drawerLayout,
                Resource.String.openDrawer, Resource.String.closeDrawer);

            _drawerLayout.AddDrawerListener(_drawerToggle);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            _drawerToggle.SyncState();

            _navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            _navigationView.NavigationItemSelected += OnNavigationItemSelected;
            //make actions on menu item pressed

            //Input = FindViewById<EditText>(Resource.Id.InputEditText);
            //Input.ShowSoftInputOnFocus = false;

            //InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
            //inputManager.HideSoftInputFromWindow(FindViewById<NavigationView>(Resource.Id.main_layout).WindowToken, HideSoftInputFlags.None );//this.CurrentFocus

            //Output = FindViewById<EditText>(Resource.Id.OutputEditText);
            //CreateTabs();

            //Input.TextChanged += _keyboardListener.OnInputTextChanged;
            //Input.TextChanged += Input_TextChanged;
            //Input.RequestFocus();
        }

        //private void Input_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        //{
        //    string output;
        //    bool error;
        //    Controller.Evaluate(input, out output, out error);
        //    Output.Text = $" = {output}";
        //}

        public void ShowMessage(string message)
        {
            Toast.MakeText(this, message, ToastLength.Long).Show();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            _drawerToggle.OnOptionsItemSelected(item);
            return base.OnOptionsItemSelected(item);
        }

        public void OnNavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            //make actions on menu item pressed
            e.MenuItem.SetChecked(true);
            switch (e.MenuItem.ItemId)
            {
                case Resource.Id.nav_home:
                    Toast.MakeText(this, "home", ToastLength.Long).Show();
                    break;
                case Resource.Id.nav_messages:
                    Toast.MakeText(this, "nav_messages", ToastLength.Long).Show();
                    break;
                case Resource.Id.nav_about:
                    var trans = SupportFragmentManager.BeginTransaction();
                    trans.Show(_infoFragment);
                    trans.Commit();
                    break;
                case Resource.Id.nav_exit:
                    Finish();
                    break;
            }
            e.MenuItem.SetChecked(false);
            _drawerLayout.CloseDrawers();
        }

        //private void CreateTabs()
        //{
        //    tabHost = FindViewById<TabHost>(Resource.Id.tabHost);

        //    tabHost.Setup();

        //    _keyBoardDigital = new Keyboard(this, Resource.Xml.keyboard_digital);
        //    _keyboardDigitalView = FindViewById<KeyboardView>(Resource.Id.keyboard_digital_view);
        //    _keyboardDigitalView.Keyboard = _keyBoardDigital;
        //    _keyboardDigitalView.OnKeyboardActionListener = _keyboardListener = new KeyboardListener(this, Input, Output);
        //    _keyboardDigitalView.Visibility = ViewStates.Visible;
        //    _keyboardDigitalView.SetBackgroundColor(Android.Graphics.Color.Magenta);

        //    TabHost.TabSpec tabSpec = tabHost.NewTabSpec("tagDigitalKeyboard");
        //    tabSpec.SetContent(Resource.Id.linearLayoutTab1);
        //    tabSpec.SetIndicator("Digitals");
        //    tabHost.AddTab(tabSpec);

        //    _keyBoardValues = new Keyboard(this, Resource.Xml.keyboard_values);
        //    _keyboardValuesView = FindViewById<KeyboardView>(Resource.Id.keyboard_values_view);
        //    _keyboardValuesView.Keyboard = _keyBoardValues;
        //    _keyboardValuesView.OnKeyboardActionListener = _keyboardListener;
        //    _keyboardValuesView.Visibility = ViewStates.Visible;
        //    _keyboardValuesView.SetBackgroundColor(Android.Graphics.Color.Magenta);

        //    tabSpec = tabHost.NewTabSpec("tagValuesKeyboard");
        //    tabSpec.SetContent(Resource.Id.linearLayoutTab2);
        //    tabSpec.SetIndicator("Values");
        //    tabHost.AddTab(tabSpec);

        //    tabHost.CurrentTab = 0;
        //}
    }
}