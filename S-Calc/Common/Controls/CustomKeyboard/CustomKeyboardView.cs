using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.InputMethodServices;
using Android.Runtime;
using Android.Util;
using System;
using System.Collections.Generic;

namespace S_Calc.Common.Controls.CustomKeyboard
{
    [Register("com.controls.CustomKeyboardView")]
    public class CustomKeyboardView : KeyboardView
    {
        private const string ANDROID_SCHEMA = "http://schemas.android.com/apk/res/android";
        private Typeface _typeface;

        public CustomKeyboardView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            ApplyCustomFont(context, attrs);
        }

        private void ApplyCustomFont(Context context, IAttributeSet attrs)
        {
            TypedArray attributeArray = context.ObtainStyledAttributes(
                      attrs,
                      Resource.Styleable.CustomKeyboardView);

            string fontName = attributeArray.GetString(Resource.Styleable.CustomKeyboardView_font);
            int textStyle = attrs.GetAttributeIntValue(ANDROID_SCHEMA, "textStyle", (int)TypefaceStyle.Normal);

            _typeface = SelectTypeface(context, fontName, textStyle);

            attributeArray.Recycle();
        }

        public override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            //IList<Keyboard.Key> keys = Keyboard.Keys;
            //foreach (Keyboard.Key key in keys)
            //{
            //    if (key.Label == null) continue;
            //    Paint paint = new Paint();
            //    paint.TextAlign = Paint.Align.Center;
            //    paint.Color = Color.Gray;
            //    paint.SetTypeface(_typeface);
            //    paint.TextSize = 40;
            //    canvas.DrawText(key.Label.ToString(), key.X + key.Width / 2 ,
            //                    key.Y + key.Height / 2, paint);
            //}
        }

        private Typeface SelectTypeface(Context context, string fontName, int textStyle)
        {
            if (fontName == context.GetString(Resource.String.font_awake))
                return FontCache.GetTypeface("awakelight.ttf", context);
            //else if (fontName == context.GetString(Resource.String.font_name_source_sans_pro))
            //{
            //    /*
            //    information about the TextView textStyle:
            //    http://developer.android.com/reference/android/R.styleable.html#TextView_textStyle
            //    */
            //    switch ((TypefaceStyle)textStyle)
            //    {
            //        case TypefaceStyle.Bold: // bold
            //            return FontCache.GetTypeface("SourceSansPro-Bold.ttf", context);

            //        case TypefaceStyle.Italic: // italic
            //            return FontCache.GetTypeface("SourceSansPro-Italic.ttf", context);

            //        case TypefaceStyle.BoldItalic: // bold italic
            //            return FontCache.GetTypeface("SourceSansPro-BoldItalic.ttf", context);

            //        case TypefaceStyle.Normal: // regular
            //        default:
            //            return FontCache.GetTypeface("SourceSansPro-Regular.ttf", context);
            //    }
            //}
            else
            {
                // no matching font found
                // return null so Android just uses the standard font (Roboto)
                return null;
            }
        }
    }
}