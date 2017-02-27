using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views.InputMethods;
using Android.Graphics;
using Android.Content;
using Android.Views;
using RPNlib;
using System;
using Android.Runtime;
using Android.Views.Animations;
using Android.InputMethodServices;
using Android.Webkit;
using Java.Lang;
using Android.Util;

namespace S_Calc
{
    [Activity(Label = "S_Calc", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        RPN_Real r;
        EditText Input, Output;
        string input => Input.Text;
        string error;
        private Keyboard _keyBoard;
        private KeyboardView _keyboardView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            _keyBoard = new Keyboard(this, Resource.Xml.keyboard);
            _keyboardView = FindViewById<KeyboardView>(Resource.Id.keyboard_view);
            _keyboardView.Keyboard = _keyBoard;
            _keyboardView.OnKeyboardActionListener = new KeyboardListener(this);
            _keyboardView.Visibility = ViewStates.Visible;
            r = new RPN_Real();
            Input = FindViewById<EditText>(Resource.Id.InputEditText);
            Output = FindViewById<EditText>(Resource.Id.OutputEditText);
            Input.TextChanged += Input_TextChanged;          
        }
        private void Input_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            Output.Text = $" = {r.ToString(input,ref error)}";
        }
    }
}

