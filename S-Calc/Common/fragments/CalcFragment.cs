using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using RPNClassLibraryCSharp;

namespace S_Calc.Common.fragments
{
    public class CalcFragment : Android.Support.V4.App.Fragment
    {
        private EditText Input, Output;
        private View _view;
        private string input => Input.Text;

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

            Input = _view.FindViewById<EditText>(Resource.Id.InputEditText);
            Input.ShowSoftInputOnFocus = false;

            Output = _view.FindViewById<EditText>(Resource.Id.OutputEditText);
            //Output.RequestFocus();

            Kernel.Keyboard.RegisterEditText(Input);
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
                            Kernel.Keyboard.Undo();
                        })
                        .Show();
                Output.Text = " =";
            }
            else
                Output.Text = $" = {output}";
            lastRes = succsess;
        }
    }
}