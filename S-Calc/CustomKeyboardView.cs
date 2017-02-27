using System;
using Android.Content;
using Android.Views;
using Android.Views.Animations;
using Android.InputMethodServices;
using Android.Util;

namespace S_Calc
{
    public class CustomKeyboardView : KeyboardView
    {
        public CustomKeyboardView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }
    }
}