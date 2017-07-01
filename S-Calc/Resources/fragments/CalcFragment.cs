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
        private MainActivity _mainActivity;

        private TabHost tabHost;
        private FloatingActionButton fab;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            _view = inflater.Inflate(Resource.Layout.Calc, container, false);

            this._mainActivity = MainActivity.Instance;

            Input = _view.FindViewById<EditText>(Resource.Id.InputEditText);
            Input.ShowSoftInputOnFocus = false;

            Output = _view.FindViewById<EditText>(Resource.Id.OutputEditText);
            Output.RequestFocus();
            CreateTabs();

            Input.TextChanged += _keyboardListener.OnInputTextChanged;
            Input.TextChanged += Input_TextChanged;
            Input.RequestFocus();

            fab = _view.FindViewById<FloatingActionButton>(Resource.Id.fab_equals);

            fab.Click += (o, e) =>
            {
                if (lastRes)
                {
                    Input.Text = Output.Text.Replace(" = ", string.Empty);
                    Input.SetSelection(Input.Text.Length);
                }
            };
            return _view;
        }
        bool lastRes;
        private void Input_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            fab.Alpha = input.Length > 22 ? 0.5f : 1;
            string output;
            bool succsess;
            Controller.Evaluate(input, out output, out succsess);

            if (!succsess)
            {
                if (lastRes)
                    Snackbar.Make(_view, output, Snackbar.LengthLong)
                        .SetAction("Undo", v =>
                        {
                            _keyboardListener.Undo();
                        })
                        .Show();
                Output.Text = " = ------";
            }
            else
                Output.Text = $" = {output}";
            lastRes = succsess;
        }

        private void CreateTabs()
        {
            tabHost = _view.FindViewById<TabHost>(Resource.Id.tabHost);

            tabHost.Setup();

            _keyBoardDigital = new Keyboard(_mainActivity, Resource.Xml.keyboard_digital);
            _keyboardDigitalView = _view.FindViewById<KeyboardView>(Resource.Id.keyboard_digital_view);
            _keyboardDigitalView.Keyboard = _keyBoardDigital;
            _keyboardDigitalView.OnKeyboardActionListener = _keyboardListener = new KeyboardListener(_mainActivity, Input, Output);
            _keyboardDigitalView.Visibility = ViewStates.Visible;
            _keyboardDigitalView.SetBackgroundColor(Android.Graphics.Color.Magenta);

            TabHost.TabSpec tabSpec = tabHost.NewTabSpec("tagDigitalKeyboard");
            tabSpec.SetContent(Resource.Id.linearLayoutTab1);
            tabSpec.SetIndicator("Digitals");
            tabHost.AddTab(tabSpec);

            _keyBoardValues = new Keyboard(_mainActivity, Resource.Xml.keyboard_values);
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