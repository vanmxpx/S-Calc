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
        string error;
        private Keyboard _keyBoard;
        private KeyboardView _keyboardView;
        private KeyboardListener _keyboardListener;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
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
            Output.Text = $" = {r.ToString(input, ref error)}";
        }
        public void ShowMessage(string message)
        {
            Toast.MakeText(this, message, ToastLength.Long).Show();
        }
    }
}