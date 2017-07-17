using System;
using Android.InputMethodServices;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Keycode = Android.Views.Keycode;

namespace S_Calc.Common.Controls.CustomKeyboard
{
    public class KeyboardListener : Java.Lang.Object, KeyboardView.IOnKeyboardActionListener
    {
        private readonly EditText input;
        string ExchangeBuffer = string.Empty;

        public string tmps { get; set; }
        public int tmpi { get; set; }
        public EventHandler Swipe;

        public KeyboardListener(EditText input)
        {
            this.input = input;
            //TODO:             Android.Views.InputMethods.IInputConnection ic = CurrentInputConnection;
        }
        public void OnKey(Keycode primaryCode, Keycode[] keyCodes)
        {
            var eventTime = DateTime.Now.Ticks;
            switch (primaryCode)
            {
                case Keycode.P:
                    PutString("^", 0);
                    return;
                case Keycode.S:
                    PutString("√()", 1);
                    return;
                case Keycode.Clear:
                    ClearInput();
                    return;
                //case Keycode.Copy:
                //    CopyOutput();
                //    return;
                case Keycode.Paste:
                    PasteToInput();
                    return;
                case Keycode.NavigatePrevious:
                    Kernel.Keyboard.Undo();
                    return;
                case (Keycode)34://golden ratio
                    PutString("φ", 0);
                    return;
                case (Keycode)35://Euler-Maskeroni
                    PutString("γ", 0);
                    return;
                case (Keycode)46://pi
                    PutString("π", 0);
                    return;
                case (Keycode)252: //ln ~ 252
                    PutString("ln()", 1);
                    return;
                case (Keycode)253: //log ~ 253
                    PutString("log()", 1);
                    return;
                case (Keycode)254: //lg ~ 254
                    PutString("lg()", 1);
                    return;
                case (Keycode)255: //exp ~ 255
                    PutString("exp()", 1);
                    return;
                case (Keycode)256: //sin ~ 256
                    PutString("sin()", 1);
                    return;
                case (Keycode)257: //cos ~ 257
                    PutString("cos()", 1);
                    return;
                case (Keycode)258: //tg ~ 258
                    PutString("tg()", 1);
                    return;
                case (Keycode)259: //ctg ~ 259
                    PutString("ctg()", 1);
                    return;
                case (Keycode)261: //sec ~ 261
                    PutString("sec()", 1);
                    return;
                case (Keycode)262: //cosec ~ 262
                    PutString("cosec()", 1);
                    return;
                case (Keycode)263: //arcsin ~ 263
                    PutString("arcsin()", 1);
                    return;
                case (Keycode)264: //arccos ~ 264
                    PutString("arccos()", 1);
                    return;
                case (Keycode)265: //arctg ~ 265
                    PutString("arctg()", 1);
                    return;
                case (Keycode)266: //arcctg ~ 266
                    PutString("arcctg()", 1);
                    return; 
                case (Keycode)267: //abs ~ 267
                    PutString("abs()", 1);
                    return;
                case (Keycode)268: //! ~ 268
                    PutString("!", 0);
                    return;
                case (Keycode)269: //!! ~ 269
                    PutString("!!", 0);
                    return;

                default:
                    var keyEvent = new KeyEvent(eventTime, eventTime, KeyEventActions.Down, primaryCode, 0, MetaKeyStates.NumLockOn);
                    MainActivity.Instance.DispatchKeyEvent(keyEvent);
                    return;
            }

        }
        public void OnPress(Keycode primaryCode)
        {
        }
        public void OnRelease(Keycode primaryCode) { }
        public void OnText(ICharSequence text) { }
        public void SwipeDown() { }
        public void SwipeLeft()
        {
            Swipe?.Invoke(this, EventArgs.Empty);
        }
        public void SwipeRight()
        {
            Swipe?.Invoke(this, EventArgs.Empty);
        }
        public void SwipeUp() { }

        private void ClearInput()
        {
            input.Text = string.Empty;
            input.SetSelection(0);
            OnRelease(Keycode.Unknown);
        }

        //TODO: Remove to touch output control
        //private void CopyOutput()
        //{
        //    tmps = output.Text.Replace(" = ", string.Empty);
        //    if (tmps != string.Empty)
        //    {
        //        ExchangeBuffer = tmps;
        //        MainActivity.Instance.ShowMessage("Результат вычислений был скопирован в буфер обмена.");
        //    }
        //    else
        //    {
        //        MainActivity.Instance.ShowMessage("Результат вычислений отсутствует.");
        //    }
        //    OnRelease(Keycode.Unknown);
        //}

        private void PasteToInput()
        {
            if (ExchangeBuffer != string.Empty)
            {
                input.Text += ExchangeBuffer;
                input.SetSelection(input.Text.Length);
            }
            else
            {
                MainActivity.Instance.ShowMessage("Buffer is empty.");
            }
            OnRelease(Keycode.Unknown);
        }

        public void PutString(string s, int cursor_back)
        {
            tmpi = input.SelectionStart;
            input.Text = input.Text.Insert(input.SelectionStart, s);
            tmpi = tmpi + s.Length - cursor_back;
            input.SetSelection(tmpi);
            OnRelease(Keycode.Unknown);
        }
    }
}