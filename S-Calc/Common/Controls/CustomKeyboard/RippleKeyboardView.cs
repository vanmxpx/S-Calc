using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.InputMethodServices;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using System.Collections.Generic;
using System.Linq;

namespace S_Calc.Common.Controls.CustomKeyboard
{
    [Register("com.controls.RippleKeyboardView")]
    public class RippleKeyboardView : KeyboardView
    {
        private Handler canvasHandler;
        private ScaleAnimation scaleAnimation;

        private List<RippleItem> ripples = new List<RippleItem>();

        /// <summary>
        /// Set Ripple color
        /// Default is #FFFFFF
        /// </summary>
        public Color RippleColor { get; set; }

        /// <summary>
        /// Set Ripple type
        /// Default is RippleType.Rectangle
        /// </summary>
        public RippleType RippleType { get; set; }

        /// <summary>
        /// Set if ripple animation has to be centered in its parent view or not
        /// Default is True
        /// </summary>
        public bool Centered { get; set; }

        /// <summary>
        /// Set Ripple padding if you want to avoid some graphic glitch
        /// Default is 0px
        /// </summary>
        public int RipplePadding { get; set; }

        /// <summary>
        /// At the end of Ripple effect, the child views has to zoom
        /// Default is False
        /// </summary>
        public bool Zooming { get; set; }

        /// <summary>
        /// Scale of the end animation
        /// Default is 1.03f
        /// </summary>
        public float ZoomScale { get; set; }

        /// <summary>
        /// Duration of the ending animation in ms
        /// Default is 200ms
        /// </summary>
        public int ZoomDuration { get; set; }

        /// <summary>
        /// Duration of the Ripple animation in ms
        /// Default is 400ms
        /// </summary>
        public int RippleDuration { get; set; }
        /// <summary>
        /// Set framerate for Ripple animation
        /// Default is 10
        /// </summary>
        public int FrameRate { get; set; }

        /// <summary>
        /// Set alpha for ripple effect color 
        /// Alpha value between 0 and 255, default is 90
        /// </summary>
        public int RippleAlpha { get; set; }

        public OnRippleCompleteListener OnCompleteListener { get; set; }

        public RippleKeyboardView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Initialize(context, attrs);
        }
        private void Initialize(Context context, IAttributeSet attrs)
        {
            if (IsInEditMode)
                return;

            TypedArray typedArray = context.ObtainStyledAttributes(attrs, Resource.Styleable.RippleKeyboardView);
            RippleColor = typedArray.GetColor(Resource.Styleable.RippleKeyboardView_rv_color, Resources.GetColor(Resource.Color.rippelColor));
            RippleType = (RippleType)typedArray.GetInt(Resource.Styleable.RippleKeyboardView_rv_type, 1);
            Zooming = typedArray.GetBoolean(Resource.Styleable.RippleKeyboardView_rv_zoom, false);
            Centered = typedArray.GetBoolean(Resource.Styleable.RippleKeyboardView_rv_centered, true);
            RippleDuration = typedArray.GetInteger(Resource.Styleable.RippleKeyboardView_rv_rippleDuration, 300);
            FrameRate = typedArray.GetInteger(Resource.Styleable.RippleKeyboardView_rv_framerate, 1000 / 100);
            RippleAlpha = typedArray.GetInteger(Resource.Styleable.RippleKeyboardView_rv_alpha, 90);
            RipplePadding = typedArray.GetDimensionPixelSize(Resource.Styleable.RippleKeyboardView_rv_ripplePadding, 0);
            canvasHandler = new Handler();
            ZoomScale = typedArray.GetFloat(Resource.Styleable.RippleKeyboardView_rv_zoomScale, 1.03f);
            ZoomDuration = typedArray.GetInt(Resource.Styleable.RippleKeyboardView_rv_zoomDuration, 200);
            typedArray.Recycle();

            this.SetWillNotDraw(false);
            this.DrawingCacheEnabled = true;
            this.Clickable = true;
        }

        public override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            RippleItem[] ripplesArray = ripples.ToArray();
            int currCount = ripplesArray.Length;
            if ( currCount != 0) PostInvalidateDelayed(FrameRate);
            //PostInvalidateDelayed(FrameRate,(int)(ripple.X - (ripple.RadiusMax / 2)) + 5, (int)(ripple.Y - (ripple.RadiusMaxY / 2)) + 5,
            //(int)(ripple.X + (ripple.RadiusMax / 2)) + 5, (int)(ripple.Y + (ripple.RadiusMaxY / 2)) + 5);
            for (int i = 0; i < currCount; i++)
            {
                RippleItem ripple = ripplesArray[i];

                if (RippleDuration <= ripple.ElapsedTime)
                {
                    ripples.Remove(ripple);
                    continue;
                }

                var radius  = (ripple.RadiusMax * (((float)ripple.ElapsedTime) / RippleDuration));
                var radiusY = (ripple.RadiusMaxY * (((float)ripple.ElapsedTime) / RippleDuration));
                if (RippleType == RippleType.Simple)
                    canvas.DrawCircle(ripple.X, ripple.Y, radius, ripple.Paint);
                else
                {
                    var rectRad = 100 < radiusY ? 0 : 100 - radiusY;
                    var maxX = ripple.RadiusMax / 8;
                    var maxY = ripple.RadiusMaxY / 8;
                    if (radiusY > maxY)
                    {
                        if (radiusY > maxX)
                            canvas.DrawRoundRect(ripple.X - maxX, ripple.Y - maxY, ripple.X + maxX, ripple.Y + maxY, rectRad, rectRad, ripple.Paint);
                        else
                            canvas.DrawRoundRect(ripple.X - radiusY, ripple.Y - maxY, ripple.X + radiusY, ripple.Y + maxY, rectRad, rectRad, ripple.Paint);
                    }
                    else
                        canvas.DrawRoundRect(ripple.X - radiusY, ripple.Y - radiusY, ripple.X + radiusY, ripple.Y + radiusY, 100, 100, ripple.Paint);
                }

                var ripp = (int)(RippleAlpha - (RippleAlpha * (((float)ripple.ElapsedTime) / RippleDuration)));
                ripple.Paint.Alpha = ripp < 0 ? 0 : ripp;
            }
        }

        public override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            scaleAnimation = new ScaleAnimation(1.0f, ZoomScale, 1.0f, ZoomScale, w / 2, h / 2);
            scaleAnimation.Duration = ZoomDuration;
            scaleAnimation.RepeatMode = RepeatMode.Reverse;
            scaleAnimation.RepeatCount = 1;
        }


        /// <summary>
        /// Launch Ripple animation for the current view with a MotionEvent
        /// </summary>
        /// <param name="e"> MotionEvent registered by the Ripple gesture listener </param>
        public void AnimateRipple(MotionEvent e) {
            CreateAnimation(e.GetX(), e.GetY());
        }

        /// <summary>
        /// Launch Ripple animation for the current view centered at x and y position
        /// </summary>
        /// <param name="x"> Horizontal position of the ripple center </param>
        /// <param name="y"> Vertical position of the ripple center </param>
        public void AnimateRipple(float x, float y)
        {
            CreateAnimation(x, y);
        }

        /// <summary>
        /// Create Ripple animation centered at x, y
        /// </summary>
        /// <param name="x"> Horizontal position of the ripple center </param>
        /// <param name="y"> Vertical position of the ripple center </param>
        private void CreateAnimation(float x, float y)
        {
            if (!this.Enabled) return;
            if (Zooming)
                this.StartAnimation(scaleAnimation);
 
            //find pressed key
            int primaryIndex = 0;
            int[] nearestKeyIndices = Keyboard.GetNearestKeys((int)x, (int)y);
            int keyCount = nearestKeyIndices.Length;
            List<Keyboard.Key> keys = Keyboard.Keys.ToList();
            for (int i = 0; i < keyCount; i++)
            {
                Keyboard.Key key = keys[nearestKeyIndices[i]];
                bool isInside = key.IsInside((int)x, (int)y);
                if (isInside)
                    primaryIndex = nearestKeyIndices[i];
            }
            var currKey = keys[primaryIndex];
            float posX, posY;
            var radiusMax = currKey.Width;
            var radiusMaxY = currKey.Height;
            if (RippleType == RippleType.Rectangle)
            {
                radiusMax *= 4;
                radiusMaxY *= 4;
            }
            else
                radiusMax /= 2;

            radiusMax -= RipplePadding;
            radiusMaxY -= RipplePadding;

            if (Centered)
            {
                posX = currKey.X + currKey.Width / 2;
                posY = currKey.Y + currKey.Height / 2;
            }
            else
            {
                posX = x;
                posY = y;
            }

            var p = new Paint();
            p.AntiAlias = true;
            p.SetStyle(Paint.Style.Fill);
            p.Color = RippleColor;
            p.Alpha = RippleAlpha;
            ripples.Add(new RippleItem(radiusMax, radiusMaxY, posX, posY + 3, p));

            Invalidate();
        }
        public override bool OnTouchEvent(MotionEvent e)
        {
            if(e.Action == MotionEventActions.Down)
                AnimateRipple(e);
            return base.OnTouchEvent(e);
        }

    }
        /// <summary>
        /// Defines a callback called at the end of the Ripple effect
        /// </summary>
    public interface OnRippleCompleteListener
    {
        void OnComplete(RippleKeyboardView rippleView);
    }

    public enum RippleType
    {
        Simple = 0,
        Rectangle = 1
    }
}

