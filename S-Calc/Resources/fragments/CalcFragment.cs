using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.InputMethodServices;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using RPNClassLibraryCSharp;

namespace S_Calc.Resources.fragments
{
    public class CalcFragment : Android.Support.V4.App.Fragment
    {
        EditText Input, Output;
        private View _view;
        private string input => Input.Text;

        private KeyboardView _keyboardDigitalView;
        private KeyboardView _keyboardValuesView;
        private Keyboard _keyBoardDigital;
        private Keyboard _keyBoardValues;
        private KeyboardListener _keyboardListener;
        private TabHost tabHost;
        private MainActivity mainActivity;

        public CalcFragment(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            _view = inflater.Inflate(Resource.Layout.Calc, container, false);

            Input = _view.FindViewById<EditText>(Resource.Id.InputEditText);
            Input.ShowSoftInputOnFocus = false;

            Output = _view.FindViewById<EditText>(Resource.Id.OutputEditText);
            CreateTabs();

            Input.TextChanged += _keyboardListener.OnInputTextChanged;
            Input.TextChanged += Input_TextChanged;
            Input.RequestFocus();
            return _view;
        }

        private void Input_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            string output;
            bool error;
            Controller.Evaluate(input, out output, out error);
            Output.Text = $" = {output}";
        }

        private void CreateTabs()
        {
            tabHost = _view.FindViewById<TabHost>(Resource.Id.tabHost);

            tabHost.Setup();

            _keyBoardDigital = new Keyboard(mainActivity, Resource.Xml.keyboard_digital);
            _keyboardDigitalView = _view.FindViewById<KeyboardView>(Resource.Id.keyboard_digital_view);
            _keyboardDigitalView.Keyboard = _keyBoardDigital;
            _keyboardDigitalView.OnKeyboardActionListener = _keyboardListener = new KeyboardListener(mainActivity, Input, Output);
            _keyboardDigitalView.Visibility = ViewStates.Visible;
            _keyboardDigitalView.SetBackgroundColor(Android.Graphics.Color.Magenta);

            TabHost.TabSpec tabSpec = tabHost.NewTabSpec("tagDigitalKeyboard");
            tabSpec.SetContent(Resource.Id.linearLayoutTab1);
            tabSpec.SetIndicator("Digitals");
            tabHost.AddTab(tabSpec);

            _keyBoardValues = new Keyboard(mainActivity, Resource.Xml.keyboard_values);
            _keyboardValuesView = _view.FindViewById<KeyboardView>(Resource.Id.keyboard_values_view);
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