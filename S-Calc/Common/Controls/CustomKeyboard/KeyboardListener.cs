﻿using System;
using Android.InputMethodServices;
using Android.Views;
using Android.Widget;
using Java.Lang;
using S_Calc.Core;
using Keycode = Android.Views.Keycode;

namespace S_Calc.Common.Controls.CustomKeyboard
{
    public class OutputTouchListener: Java.Lang.Object, View.IOnTouchListener
    {
        readonly KeyboardListener listener;

        public OutputTouchListener(KeyboardListener listener)
        {
            this.listener = listener;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            listener.CopyOutput();
            return true;
        }
    }

    public class KeyboardListener : Java.Lang.Object, KeyboardView.IOnKeyboardActionListener
    {
        private readonly EditText input;
        private readonly EditText output;

        public string tmps { get; set; }
        public int tmpi { get; set; }
        public EventHandler Swipe;

        public KeyboardListener(EditText input, EditText output)
        {
            this.input = input;
            this.output = output;
            this.output.SetOnTouchListener(new OutputTouchListener(this));
            this.input.TextChanged += Input_TextChanged;
            //Android.Views.InputMethods.IInputConnection ic = CurrentInputConnection;
        }

        private void Input_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            Controller.Evaluate(input.Text, input.SelectionStart, out string result, out bool succsess);
            if (!succsess)
            {
                output.Text = " =";
            }
            else
            {
                output.Text = $" = {result}";
            }

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
                case (Keycode)252: //mod
                    PutString("mod", 0);
                    return;
                case Keycode.Clear:
                    ClearInput();
                    return;
                case (Keycode)253: // random from 0.0 to 1.0
                    PutString("random()", 0);
                    return;
                case (Keycode)279 : // Keycode.Paste ://
                    PasteToInput();
                    return;
                case (Keycode)260: //undo
                    Undo();
                    return;
                case (Keycode)261: //redo
                    Redo();
                    return;
                case (Keycode)67: //backspace
                    Backspace();
                    return;
                case (Keycode)1000: //left
                    CursorLeft();
                    return;
                case (Keycode)1001: //right
                    CursorRight();
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
        public void SwipeDown()
        {
            Kernel.Keyboard.HideCustomKeyboard();
        }
        public void SwipeLeft()
        {
            Swipe?.Invoke(this, EventArgs.Empty);
        }
        public void SwipeRight()
        {
            Swipe?.Invoke(this, EventArgs.Empty);
        }
        public void SwipeUp() { }

        private void Backspace()
        {
            input.TextChanged -= Input_TextChanged;

            tmpi = input.SelectionStart;
            if (tmpi != 0)
            {
                input.Text = input.Text.Remove(tmpi - 1,1);
                input.SetSelection(tmpi - 1);
                Input_TextChanged(null, null);
            }
            input.TextChanged += Input_TextChanged;
            OnRelease(Keycode.Unknown);
        }

        private void Undo()
        {
            bool UndoFailure = Controller.Undo(out string Expression, out int CursorPosition, out string Result, out bool success);
            if (!UndoFailure)
            {
                input.TextChanged -= Input_TextChanged;
                input.Text = Expression;
                input.SetSelection(CursorPosition);
                if (success)
                {
                    output.Text = " = " + string.Copy(Result);
                }
                else
                {
                    output.Text = " =";
                }
                input.TextChanged += Input_TextChanged;
            }
            else
            {
                Toast.MakeText(MainActivity.Instance, "Undo option does not available.", ToastLength.Long).Show();
            }
            OnRelease(Keycode.Unknown);
        }

        private void Redo()
        {
            bool RedoFailure = Controller.Redo(out string Expression, out int CursorPosition, out string Result, out bool success);
            if (!RedoFailure)
            {
                input.TextChanged -= Input_TextChanged;
                input.Text = Expression;
                input.SetSelection(CursorPosition);
                if (success)
                {
                    output.Text = " = " + string.Copy(Result);
                }
                else
                {
                    output.Text = " = ";
                }
                input.TextChanged += Input_TextChanged;
            }
            else
            {
                Toast.MakeText(MainActivity.Instance, "Redo option does not available.", ToastLength.Long).Show();
            }
            OnRelease(Keycode.Unknown);
        }

        private void ClearInput()
        {
            input.SetSelection(0);
            input.Text = string.Empty;
            OnRelease(Keycode.Unknown);
        }

        private void PasteToInput()
        {
            bool IsPasteAvailable = Controller.Paste(out string Expression);
            if (IsPasteAvailable)
            {
                input.TextChanged -= Input_TextChanged;

                tmpi = input.SelectionStart;
                input.Text = input.Text.Insert(input.SelectionStart, Expression);
                input.SetSelection(tmpi + Expression.Length);

                Input_TextChanged(null, null);

                input.TextChanged += Input_TextChanged;
            }
            else
            {
                Toast.MakeText(MainActivity.Instance, "Paste option does not available.", ToastLength.Long).Show();
            }
            OnRelease(Keycode.Unknown);
        }

        public void CopyOutput()
        {
            bool IsCopyAvailable = Controller.Copy(output.Text.Replace("=", string.Empty).Replace(" ", string.Empty));
            if (!IsCopyAvailable)
            {
                Toast.MakeText(MainActivity.Instance, "Copy option does not available.", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(MainActivity.Instance, "Result has been copied to clipboard.", ToastLength.Long).Show();
            }
            //OnRelease(Keycode.Unknown);
        }

        private void CursorLeft()
        {
            tmpi = input.SelectionStart;
            if (tmpi != 0)
            {
                input.SetSelection(--tmpi);
            }
            OnRelease(Keycode.Unknown);
        }

        private void CursorRight()
        {
            tmpi = input.SelectionStart;
            if (tmpi != input.Text.Length)
            {
                input.SetSelection(++tmpi);
            }
            OnRelease(Keycode.Unknown);
        }

        public void PutString(string s, int cursor_back)
        {
            input.TextChanged -= Input_TextChanged;

            tmpi = input.SelectionStart; 
            input.Text = input.Text.Insert(input.SelectionStart, s);
            tmpi = tmpi + s.Length - cursor_back;
            input.SetSelection(tmpi);
            Input_TextChanged(null, null);

            input.TextChanged += Input_TextChanged;

            OnRelease(Keycode.Unknown);
        }
    }
}