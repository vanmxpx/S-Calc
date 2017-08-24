using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace S_Calc.Common.Controls.CustomKeyboard
{
    public class RippleItem
    {
        private float radiusMax, radiusMaxY;
        private long startTime;
        private float x, y;
        private bool animationRunning;
        private Paint paint;


        public float RadiusMax => radiusMax;
        public float RadiusMaxY => radiusMaxY;
        public float X => x;
        public float Y => y;
        public long StartTime { get{ return startTime; } set{ startTime = value; } }
        public long ElapsedTime { get { return Java.Lang.JavaSystem.CurrentTimeMillis() - startTime; } }
        public bool AnimationRunning { get { return animationRunning; } set { animationRunning = value; } }
        public Paint Paint => paint;

        public RippleItem(float radiusMax, float radiusMaxY, float x, float y, Paint paint)
        {
            startTime = Java.Lang.JavaSystem.CurrentTimeMillis();
            this.radiusMax = radiusMax;
            this.radiusMaxY = radiusMaxY;
            this.x = x;
            this.y = y;
            this.animationRunning = true;
            this.paint = paint;
        }
    }
}