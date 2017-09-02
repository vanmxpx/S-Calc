using System;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using S_Calc.Common.Controls.CustomKeyboard;
using System.Text;

namespace S_Calc.Common.fragments
{
    public class CalcFragment : Android.Support.V4.App.Fragment
    {
        private EditText Input, Output;
        private View _view;
        private string input => Input.Text;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.Calc, container, false);

            Input = _view.FindViewById<EditText>(Resource.Id.InputEditText);
            Input.ShowSoftInputOnFocus = false;

            Output = _view.FindViewById<EditText>(Resource.Id.OutputEditText);

            Kernel.Keyboard.OnCustomKeyboardCreate(_view.FindViewById<RippleKeyboardView>(Resource.Id.keyboard_view));
            Kernel.Keyboard.RegisterEditText(Input, Output);
            //Input.TextChanged += Input_TextChanged;
            Input.RequestFocus();

            _view.FindViewById<Button>(Resource.Id.but_equals).Click += (o, e) =>
            {
                if (Output.Text != string.Empty)
                {
                    AddCaclUnit(Output.Text);
                }
            };

            return _view;
        }

        internal void AddCaclUnit(string functionText, int numberOfArguments = 0)
        {
            StringBuilder sb = new StringBuilder();
            int cursPos = Input.SelectionStart + functionText.Length;
            sb.Append(functionText);

            if (numberOfArguments != 0)
            {
                sb.Append("(");
                for (int i = 1; i < numberOfArguments; i++)
                    sb.Append(",");
                sb.Append(")");
                cursPos ++;
            }

            Input.Text = Input.Text.Insert(Input.SelectionStart, sb.ToString());
            Input.SetSelection(cursPos);
        }

        //bool lastRes;

        //private void Input_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        //{
        //    if (input.Length == 0)
        //    {
        //        Output.Text = " =";
        //        return;
        //    }
        //    fab.Alpha = input.Length > 22 ? 0.5f : 1;
        //    string output;
        //    bool succsess;
        //    Controller.Evaluate(input, Input.SelectionStart, out output, out succsess);

        //    //TODO: Setup error handling
        //    if (!succsess)
        //    {
        //        //if (lastRes && output != "Stack empty.")
        //        //{
        //        //    Snackbar.Make(_view, output, Snackbar.LengthLong)
        //        //        .SetAction("Undo", v =>
        //        //        {
        //        //            Kernel.Keyboard.Undo();
        //        //        })
        //        //        .Show();
        //        //    lastRes = false;
        //        //}
        //        Output.Text = " =";

        //    }
        //    else
        //    {
        //        Output.Text = $" = {output}";
        //        lastRes = true;
        //    }

        //}
    }
}