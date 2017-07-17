using Android.InputMethodServices;
using Android.Content.Res;
using System.Xml;
using Android.Content;
using Java.Lang;

using static Android.InputMethodServices.Keyboard;

namespace S_Calc.Common.Controls.CustomKeyboard
{
    public class NumericKeyboard : Keyboard
    {
        public NumericKeyboard(Context context, int xmlLayoutResId)
            : base (context, xmlLayoutResId)
        {
        }

        public NumericKeyboard(Context context, int layoutTemplateResId,
                ICharSequence characters, int columns, int horizontalPadding)
            : base(context, layoutTemplateResId, characters, columns, horizontalPadding)
        {
        }

        protected override Key CreateKeyFromXml(Resources res, Row parent, int x, int y,
                XmlReader parser)
        {
            Key key = new NumKey(res, parent, x, y, parser);
            return key;
        }
    }
    class NumKey : Keyboard.Key
    {
        public NumKey(Resources res, Row parent, int x, int y, XmlReader parser)
            : base(res, parent, x, y, parser)
        {
        }

        private static int[] KEY_STATE_NORMAL_ON = {
            Android.Resource.Attribute.StateCheckable,
            Android.Resource.Attribute.StateChecked
        };

        private static int[] KEY_STATE_PRESSED_ON = {
            Android.Resource.Attribute.StatePressed,
            Android.Resource.Attribute.StateCheckable,
            Android.Resource.Attribute.StateChecked
        };

        private static int[] KEY_STATE_NORMAL_OFF = {
            Android.Resource.Attribute.StateCheckable
        };

        private static int[] KEY_STATE_PRESSED_OFF = {
            Android.Resource.Attribute.StatePressed,
            Android.Resource.Attribute.StateCheckable
        };

        private static int[] KEY_STATE_FUNCTION = {
            Android.Resource.Attribute.StateSingle
        };

        private static int[] KEY_STATE_FUNCTION_PRESSED = {
            Android.Resource.Attribute.StatePressed,
            Android.Resource.Attribute.StateSingle
        };

        private static int[] KEY_STATE_LONG = {
            Android.Resource.Attribute.StateLongPressable
        };

        private static int[] KEY_STATE_LONG_PRESSED = {
            Android.Resource.Attribute.StateLongPressable,
            Android.Resource.Attribute.StatePressed
        };

        private static int[] KEY_STATE_NORMAL = {
        };

        private  static int[] KEY_STATE_PRESSED = {
            Android.Resource.Attribute.StatePressed
        };
        public override int[] GetCurrentDrawableState()
        {
            int[] states = KEY_STATE_NORMAL;

            if (On)
            { 
                if (Pressed)
                    states = KEY_STATE_PRESSED_ON;
                else
                    states = KEY_STATE_NORMAL_ON;
            }
            else
            {
                if (Sticky)
                {
                    if (Pressed)
                        states = KEY_STATE_PRESSED_OFF;
                    else
                        states = KEY_STATE_NORMAL_OFF;
                }
                else if (Modifier)
                {
                    if (Pressed)
                        states = KEY_STATE_FUNCTION_PRESSED;
                    else
                        states = KEY_STATE_FUNCTION;
                }
                else if (Repeatable)
                {
                    if (Pressed)
                        states = KEY_STATE_LONG_PRESSED;
                    else
                        states = KEY_STATE_LONG;
                }
                else
                {
                    if (Pressed)
                        states = KEY_STATE_PRESSED;
                }
            }
            return states;
        }
    }
}