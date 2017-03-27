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
        private Keyboard _keyBoardDigital;
        private KeyboardView _keyboardDigitalView;

        private Keyboard _keyBoardValues;
        private KeyboardView _keyboardValuesView;

        private KeyboardListener _keyboardListener;
        public TabHost tabHost;

        protected override void OnCreate(Bundle bundle)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

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
            tabSpec.SetContent(Resource.Id.linearLayout3);
            tabSpec.SetIndicator("Digitals");
            tabHost.AddTab(tabSpec);

            _keyBoardValues = new Keyboard(this, Resource.Xml.keyboard_values);
            _keyboardValuesView = FindViewById<KeyboardView>(Resource.Id.keyboard_values_view);
            _keyboardValuesView.Keyboard = _keyBoardValues;
            _keyboardValuesView.OnKeyboardActionListener = _keyboardListener;
            _keyboardValuesView.Visibility = ViewStates.Visible;
            _keyboardValuesView.SetBackgroundColor(Android.Graphics.Color.Magenta);

            tabSpec = tabHost.NewTabSpec("tagValuesKeyboard");
            tabSpec.SetContent(Resource.Id.linearLayout2);
            tabSpec.SetIndicator("Values");
            tabHost.AddTab(tabSpec);

            tabHost.CurrentTab = 0;
        }
    }
}