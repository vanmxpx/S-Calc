using Android.App;
using Android.OS;
using Android.Widget;
using Android.Views;
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
            Input.ShowSoftInputOnFocus = false;

            Output = FindViewById<EditText>(Resource.Id.OutputEditText);
            Input.TextChanged += Input_TextChanged;
        }
        private void Input_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            Output.Text = $" = {r.ToString(input, ref error)}";
        }
    }
}

////Hiding keyboard
//public override bool DispatchTouchEvent(MotionEvent ev)
//{
//    if (ev.Action == MotionEventActions.Down)
//    {
//        SetSoftInputMode(SoftInput.StateAlwaysHidden);
//
//          InputKeyboardType.None;
//        InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
//        imm.HideSoftInputFromWindow(Window.DecorView.WindowToken, HideSoftInputFlags.NotAlways);
//    }
//    return base.DispatchTouchEvent(ev);
//}