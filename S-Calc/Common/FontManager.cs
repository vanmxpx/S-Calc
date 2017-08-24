using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Java.Lang;
using Java.Lang.Reflect;

namespace S_Calc.Common
{
    public static class FontManager
    {
        public static void SetDefaultFont(Context context,
                string staticTypefaceFieldName, string fontAssetName)
        {
            Typeface regular = Typeface.CreateFromAsset(context.Assets,
                    fontAssetName);
            ReplaceFont(staticTypefaceFieldName, regular);
        }

        private static void ReplaceFont(string staticTypefaceFieldName,
                Typeface newTypeface)
        {
            try
            {
                Field staticField = Java.Lang.Class.FromType(typeof(Typeface)).GetDeclaredField(staticTypefaceFieldName);
                staticField.Accessible = true;
                staticField.Set(null, newTypeface);
            }
            catch (NoSuchFieldException e)
            {
                e.PrintStackTrace();
            }
            catch (IllegalAccessException e)
            {
                e.PrintStackTrace();
            }
        }
    }
    public static class FontCache
    {
        private static Dictionary<string, Typeface> fontCache = new Dictionary<string, Typeface>();

        public static Typeface GetTypeface(string fontname, Context context)
        {
            Typeface typeface;
            fontCache.TryGetValue(fontname, out typeface);

            if (typeface == null)
            {
                try
                {
                    typeface = Typeface.CreateFromAsset(context.Assets, fontname);
                }
                catch (Exception e)
                {
                    return null;
                }

                fontCache.Add(fontname, typeface);
            }

            return typeface;
        }
    }
}