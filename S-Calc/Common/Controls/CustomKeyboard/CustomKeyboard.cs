using System;
using System.Collections.Generic;
using Android.InputMethodServices;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Views.Animations;
using System.Threading.Tasks;

namespace S_Calc.Common.Controls.CustomKeyboard
{
    public class CustomKeyboard : Java.Lang.Object, View.IOnFocusChangeListener, View.IOnClickListener
    {
        private KeyboardListener _keyboardListener;
        private KeyboardView _keyboardView;
        private Keyboard _keyBoardDigital;
        private Keyboard _keyBoardValues;
        private Keyboard _currentKeyboard;

        private EditText input;
        private EditText output;

        public bool Visible => _keyboardView.Visibility == ViewStates.Visible;

        //public void OnCustomKeyboardCreate()
        //{
        //    _keyBoardDigital = new NumericKeyboard(MainActivity.Instance, Resource.Xml.keyboard_digital);
        //    _keyBoardValues = new NumericKeyboard(MainActivity.Instance, Resource.Xml.keyboard_values);
        //    _currentKeyboard = _keyBoardDigital;

        //    _keyboardView = MainActivity.Instance.FindViewById<KeyboardView>(Resource.Id.keyboard_view);
        //    _keyboardView.Keyboard = _currentKeyboard;
        //    _keyboardView.Visibility = ViewStates.Visible;
        //    _keyboardView.SetBackgroundColor(Android.Graphics.Color.Magenta);
        //    _keyboardView.PreviewEnabled = false;

        //    history = new List<Tuple<string, int>>() { Tuple.Create(string.Empty, 0) };

        //}
        public void OnCustomKeyboardCreate(CustomKeyboardView keyboardView)
        {
            _keyBoardDigital = new NumericKeyboard(MainActivity.Instance, Resource.Xml.keyboard_digital);
            _keyBoardValues = new NumericKeyboard(MainActivity.Instance, Resource.Xml.keyboard_values);
            _currentKeyboard = _keyBoardDigital;

            _keyboardView = keyboardView;// MainActivity.Instance.FindViewById<KeyboardView>(Resource.Id.keyboard_view);
            _keyboardView.Keyboard = _currentKeyboard;
            _keyboardView.Visibility = ViewStates.Visible;
            _keyboardView.SetBackgroundColor(Android.Graphics.Color.Magenta);
            _keyboardView.PreviewEnabled = false;

        }

        public void RegisterEditText(EditText target, EditText output)
        {
            input = target;
            // Make the custom keyboard appear
            input.OnFocusChangeListener = this;
            input.SetOnClickListener(this);

            // Disable spell check (hex strings look like words to Android)
            target.InputType = target.InputType | Android.Text.InputTypes.TextFlagNoSuggestions;

            //Handle text changing
            _keyboardView.OnKeyboardActionListener = _keyboardListener = new KeyboardListener(input, output);
            _keyboardListener.Swipe += OnSwipe;
        }

        #region Events
        public void OnFocusChange(View v, bool hasFocus)
        {
            if (hasFocus) ShowCustomKeyboard(v);
            else HideCustomKeyboard();
        }
        public void OnClick(View v)
        {
            ShowCustomKeyboard(v);
        }
        public void OnSwipe(object sender, EventArgs e)
        {
            _keyboardView.StartAnimation(AnimationUtils
                .LoadAnimation(MainActivity.Instance, Resource.Animation.abc_fade_out));
            _currentKeyboard = _currentKeyboard == _keyBoardDigital ? _keyBoardValues : _keyBoardDigital;
            _keyboardView.Keyboard = _currentKeyboard;
            _keyboardView.StartAnimation(AnimationUtils
                .LoadAnimation(MainActivity.Instance, Resource.Animation.abc_fade_in));
        }
        #endregion

        public void HideCustomKeyboard()
        {
            if (!Visible) return;
            _keyboardView.Enabled = false;

            var anim = AnimationUtils.LoadAnimation(MainActivity.Instance, Resource.Animation.abc_slide_out_bottom);
            anim.Duration = 220;
            _keyboardView.StartAnimation(anim);

            _keyboardView.Visibility = ViewStates.Gone;

        }

        public void ShowCustomKeyboard(View v)
        {
            if (Visible) return;
            _keyboardView.Visibility = ViewStates.Visible;

            var anim = AnimationUtils.LoadAnimation(MainActivity.Instance, Resource.Animation.abc_slide_in_bottom);
            anim.Duration = 220;
            _keyboardView.StartAnimation(anim);

            _keyboardView.Enabled = true;
        }
    }
}