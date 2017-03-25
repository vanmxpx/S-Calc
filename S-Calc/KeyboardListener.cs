using System;
using Android.App;
using Android.Views;
using Java.Lang;
using Android.Widget;
using System.Collections.Generic;

namespace S_Calc
{
    public class KeyboardListener : Java.Lang.Object, Android.InputMethodServices.KeyboardView.IOnKeyboardActionListener
    {
        private readonly Activity _activity;
        private readonly EditText input;
        private readonly EditText output;
        string ExchangeBuffer = string.Empty, tmps;
        int tmpi;
        List<Tuple<string, int>> history;
        const int _history_list_max_limit = 100;
        bool IsUndoKeyEnabled = false;

        public KeyboardListener(Activity activity, EditText input, EditText output)
        {
            _activity = activity;
            this.input = input;
            this.output = output;
            history = new List<Tuple<string, int>>() { Tuple.Create(string.Empty, 0) };
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
                //case Keycode.Paste:
                //    PasteToInput();
                //    return;
                case Keycode.NavigatePrevious:
                    if (IsUndoKeyEnabled) { Undo(); }
                    return;
                default:
                    var keyEvent = new KeyEvent(eventTime, eventTime, KeyEventActions.Down, primaryCode, 0, MetaKeyStates.NumLockOn);
                    _activity.DispatchKeyEvent(keyEvent);
                    return;
            }
            
        }
        public void OnPress(Keycode primaryCode)
        {

        }
        public void OnRelease(Keycode primaryCode) { }
        public void OnText(ICharSequence text) { }
        public void SwipeDown() { }
        public void SwipeLeft() { }
        public void SwipeRight() { }
        public void SwipeUp() { }
        private void ClearInput()
        {
            input.Text = string.Empty;
            input.SetSelection(0);
            OnRelease(Keycode.Unknown);
        }
        private void CopyOutput()
        {
            tmps = output.Text.Replace(" = ", string.Empty);
            if (tmps != string.Empty)
            {
                ExchangeBuffer = tmps;
                ((MainActivity)_activity).ShowMessage("Результат вычислений был скопирован в буфер обмена.");
            }
            else
            {
                ((MainActivity)_activity).ShowMessage("Результат вычислений отсутствует.");
            }
            OnRelease(Keycode.Unknown);
        }
        private void PasteToInput()
        {
            if (ExchangeBuffer != string.Empty)
            {
                input.Text += ExchangeBuffer;
                input.SetSelection(input.Text.Length);
            }
            else
            {
                ((MainActivity)_activity).ShowMessage("Буфер обмена пуст.");
            }
            OnRelease(Keycode.Unknown);
        }
        private void Undo()
        {
            try
            {
                history.RemoveAt(history.Count - 1);
                tmps = history[history.Count - 1].Item1;
                tmpi = history[history.Count - 1].Item2;
                history.RemoveAt(history.Count - 1);
                input.Text = tmps;
                input.SetSelection(tmpi);
            }
            catch
            {
                input.SetSelection(input.Text.Length);
                return;
            }
            OnRelease(Keycode.Unknown);
        }
        private void HistoryListAdd(string s)
        {
            history.Add(Tuple.Create(s,input.SelectionStart));
            if (history.Count > _history_list_max_limit) { history.RemoveAt(0); }
            IsUndoKeyEnabled = history.Count <= 1 ? false : true;
        }
        public void OnInputTextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            HistoryListAdd(input.Text);
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