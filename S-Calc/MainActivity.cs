using Android.App;
using Android.Content.Res;
using Android.OS;
using Android.Widget;
using Android.Views;
using RPNlib;
using Android.InputMethodServices;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Support.V7.App;
using Android.Support.V4.Widget;

namespace S_Calc
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {

        RPN_Real r;
        EditText Input, Output;
        string input => Input.Text;
        string _error;
        private Keyboard _keyBoard;
        private KeyboardView _keyboardView;
        private KeyboardListener _keyboardListener;
        private DrawerLayout _drawerLayout;
        private NavigationView _navigationView;
        string TAG = "Menu";

        protected override void OnCreate(Bundle bundle)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            CreateTabs();


            SupportActionBar.SetDefaultDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            _navigationView.NavigationItemSelected += (sender, e) =>
            {
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

            Input = FindViewById<EditText>(Resource.Id.InputEditText);
            Input.ShowSoftInputOnFocus = false;
            Output = FindViewById<EditText>(Resource.Id.OutputEditText);
            _keyBoard = new Keyboard(this, Resource.Xml.keyboard);
            _keyboardView = FindViewById<KeyboardView>(Resource.Id.keyboard_view);
            _keyboardView.Keyboard = _keyBoard;
            _keyboardView.OnKeyboardActionListener = _keyboardListener = new KeyboardListener(this, Input, Output);
            _keyboardView.Visibility = ViewStates.Visible;
            _keyboardView.SetBackgroundColor(Android.Graphics.Color.Magenta);
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
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    _drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void CreateTabs()
        {
            TabHost tabHost = FindViewById<TabHost>(Resource.Id.tabHost);

            tabHost.Setup();

            TabHost.TabSpec tabSpec = tabHost.NewTabSpec("tagKeyboard");

            tabSpec.SetContent(Resource.Id.linearLayout3);
            tabSpec.SetIndicator("Keyboard");
            tabHost.AddTab(tabSpec);

            tabSpec = tabHost.NewTabSpec("tagButton");
            tabSpec.SetContent(Resource.Id.linearLayout2);
            tabSpec.SetIndicator("Button");
            tabHost.AddTab(tabSpec);

            tabSpec = tabHost.NewTabSpec("tagEmpty");
            tabSpec.SetContent(Resource.Id.linearLayout4);
            tabSpec.SetIndicator("Empty");
            tabHost.AddTab(tabSpec);

            tabHost.CurrentTab = 0;
        }
    }
}