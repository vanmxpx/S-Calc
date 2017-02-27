using System;
using Android.App;
using Android.Views;
using Java.Lang;

namespace S_Calc
{
    public class KeyboardListener : Java.Lang.Object, Android.InputMethodServices.KeyboardView.IOnKeyboardActionListener
    {
        private readonly Activity _activity;

        public KeyboardListener(Activity activity)
        {
            _activity = activity;
        }
        public void OnKey(Keycode primaryCode, Keycode[] keyCodes)
        {
            var eventTime = DateTime.Now.Ticks;
            var keyEvent = new KeyEvent(eventTime, eventTime, KeyEventActions.Down, primaryCode, 0, MetaKeyStates.NumLockOn);
            _activity.DispatchKeyEvent(keyEvent);
            
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
    }
}