using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views.InputMethods;
using Android.Graphics;
using Android.Content;
using Android.Views;
using RPNlib;
using System;

namespace S_Calc
{
    [Activity(Label = "S_Calc", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        RPN_Real r;
        EditText Input, Output;
        string input => Input.Text;

        private InputMethodManager imm;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            r = new RPN_Real();
            Input = FindViewById<EditText>(Resource.Id.InputEditText);
            Output = FindViewById<EditText>(Resource.Id.OutputEditText);
            Output.Text = "Hello world";
            imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            Input.Click += EnterTextClick;
            Output.Click += EnterTextClick;
            Input.TextChanged += Input_TextChanged;
        }

        private void Input_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            Output.Text = $"Input SelectionStart: {Input.SelectionStart}";
        }

        private void EnterTextClick(object sender, EventArgs e)
        {
            //Это прячет клавиатуру, работает через мат со второго нажатия и неизвестно как

            //imm.HideSoftInputFromWindow(Input.WindowToken, 0);
            //imm.HideSoftInputFromWindow(Output.WindowToken, 0);
        }
    }
}

