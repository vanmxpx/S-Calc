using Android.App;
using Android.OS;
using Android.Widget;
using Android.Views;
using System.Collections.Generic;
using RPNlib;
using Android.InputMethodServices;

namespace S_Calc
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        RPN_Real r;
        EditText Input, Output;
        string input => Input.Text;
        string _error;
        private Keyboard _keyBoard;
        private KeyboardView _keyboardView;
        private KeyboardListener _keyboardListener;

        protected override void OnCreate(Bundle bundle)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            CreateTabs();

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