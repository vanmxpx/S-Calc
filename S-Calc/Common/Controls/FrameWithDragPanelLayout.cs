using Android.Content;
using Android.Util;
using Android.Widget;
using System;
using Android.Animation;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.View;
using Android.Views;
using Android.Views.Animations;
using static Android.Animation.Animator;
using Android.Runtime;
using Android.Content.Res;

namespace S_Calc.Common.Controls
{
    [Register("com.controls.FrameWithDragPanelLayout")]
    public class FrameWithDragPanelLayout : FrameLayout
    {
        private static DecelerateInterpolator sDecelerator = new DecelerateInterpolator();

        private int panelPeekWidth;
        private int markerHeight;

        private bool opened;
        private bool touching;
        private bool isBeingDragged;
        private static bool willDrawShadow;

        private int touchSlop;
        private float parallaxFactor = 0f;
        private float touchX;

        private VelocityTracker velocityTracker = null;

        private static View bottomPanel;
        private static View slidingPanel;

        private Drawable shadowDrawable;

        private static bool animating = false;
        private int slidingPanelTop;
        private int slidingPanelBottom;
        private int panelPeekPadding;
        private int panelPeekHeight;

        public bool PanelState => opened;

        public FrameWithDragPanelLayout(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize(context, attrs);
        }

        public FrameWithDragPanelLayout(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize(context, attrs);
        }

        private void Initialize(Context context, IAttributeSet attrs)
        {
            touchSlop = ViewConfiguration.Get(Context).ScaledTouchSlop;
            
            TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.FrameWithDragPanelLayout);
            willDrawShadow = a.GetBoolean(Resource.Styleable.FrameWithDragPanelLayout_willDrawShadow, willDrawShadow);
            panelPeekWidth = a.GetDimensionPixelSize(Resource.Styleable.FrameWithDragPanelLayout_panelPeekWidth, panelPeekWidth);
            markerHeight = a.GetDimensionPixelSize(Resource.Styleable.FrameWithDragPanelLayout_markerHeight, markerHeight);
            parallaxFactor = a.GetFloat(Resource.Styleable.FrameWithDragPanelLayout_parallaxFactor, parallaxFactor);
            panelPeekPadding = markerHeight + (int)TypedValue.ApplyDimension(ComplexUnitType.Px, 15, Resources.DisplayMetrics);
            panelPeekHeight = markerHeight + a.GetDimensionPixelSize(Resource.Styleable.FrameWithDragPanelLayout_panelPeekHeight, 40);
            //TODO: Draw shadow
            int shadowDrawableId = a.GetResourceId(Resource.Styleable.FrameWithDragPanelLayout_shadowDrawable, -1);
            if (shadowDrawableId == -1)
            {
                willDrawShadow = false;
                return;
            }
            shadowDrawable = context.GetDrawable(shadowDrawableId);

        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);

            if (ChildCount != 2)
                throw new InvalidProgramException("DraggedPanelLayout must have 2 children!");

            bottomPanel = GetChildAt(0);
            bottomPanel.Layout(left, top, right, bottom);

            slidingPanel = GetChildAt(1);

            slidingPanelTop = (int)(top + (bottom / 6));
            slidingPanelBottom = (int)(bottom - 10);
            slidingPanel.LayoutParameters.Height = slidingPanelBottom - slidingPanelTop;
            int panelMeasuredWidth = slidingPanel.MeasuredWidth;

            slidingPanel.Layout(
                opened ? left : right - panelPeekWidth,
                slidingPanelTop,
                opened ? right : right - panelPeekWidth + panelMeasuredWidth,
                slidingPanelBottom);
        }

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);

            Paint myPaint = new Paint();
            myPaint.Color = Color.Rgb(200, 200, 200);
            myPaint.StrokeWidth = 10;
            myPaint.SetStyle(Paint.Style.Fill);

            int top1 = slidingPanel.Left + panelPeekWidth;
            int bottom = slidingPanel.Top;
            int left = slidingPanel.Top + markerHeight;
            int right = slidingPanel.Right;

            canvas.DrawRect(left, top1, right, bottom, myPaint);

            top1 = slidingPanel.Top + panelPeekPadding;
            bottom = slidingPanel.Top + panelPeekHeight + 40;
            left = slidingPanel.Left - 40;
            right = slidingPanel.Left + panelPeekWidth + panelPeekWidth + 40;

            canvas.DrawRect(left, top1, right, bottom, myPaint);

            if (willDrawShadow) //!IsInEditMode &&
            {
                int top = (int)(slidingPanel.Top + slidingPanel.TranslationY);
                shadowDrawable.SetBounds(slidingPanel.Left, top - shadowDrawable.IntrinsicWidth, MeasuredWidth, top);
                shadowDrawable.Draw(canvas);
            }
            if (animating)
            {
                ViewCompat.PostInvalidateOnAnimation(this);
            }
        }

        public void Open()
        {
            AnimatePanel(true, CalculateDistance(true), 500);
        }

        public void Close()
        {
            AnimatePanel(false, CalculateDistance(false), 500);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (!CheckPanelTouch(e) && !touching)
                return base.OnTouchEvent(e);
            ObtainVelocityTracker();
            if (e.Action == MotionEventActions.Down)
            {
                StartDragging(e);
            }
            else if (e.Action == MotionEventActions.Move)
            {
                if (touching)
                {
                    velocityTracker.AddMovement(e);

                    float translation = e.GetX() - touchX;
                    translation = BoundTranslation(translation);

                    slidingPanel.TranslationX = translation;
                    bottomPanel
                        .TranslationX = (float)(opened
                        ? -(MeasuredWidth - panelPeekWidth - translation)
                          * parallaxFactor
                        : translation * parallaxFactor);
                }
            }
            else if (e.Action == MotionEventActions.Up)
            {
                isBeingDragged = false;
                touching = false;

                velocityTracker.AddMovement(e);
                velocityTracker.ComputeCurrentVelocity(1);
                float velocityX = velocityTracker.XVelocity;
                velocityTracker.Recycle();
                velocityTracker = null;


                FinishAnimateToFinalPosition(velocityX);
            }
            return true;
        }

        public override bool OnInterceptTouchEvent(MotionEvent e)
        {
            if (!CheckPanelTouch(e))
                return base.OnInterceptTouchEvent(e);

            if (e.Action == MotionEventActions.Down)
            {
                touchX = e.GetX();
            }
            else if (e.Action == MotionEventActions.Move)
            {
                if (Math.Abs(touchX - e.GetX()) > touchSlop)
                {
                    isBeingDragged = true;
                    StartDragging(e);
                }
            }
            else if (e.Action == MotionEventActions.Up)
            {
                isBeingDragged = false;
            }

            return isBeingDragged;
        }

        public void StartDragging(MotionEvent e)
        {
            touchX = e.GetX();
            touching = true;

            ObtainVelocityTracker();
            velocityTracker.AddMovement(e);
            AllowShadow();
        }

        public void FinishAnimateToFinalPosition(float velocityX)
        {
            bool flinging = Math.Abs(velocityX) > 0.5;

            bool opening;
            float distX;
            long duration;

            if (flinging)
            {
                // If fling velocity is fast enough we continue the motion starting
                // with the current speed
                opening = velocityX < 0;
                distX = CalculateDistance(opening);
                duration = (long)Math.Abs(Math.Round(distX / velocityX));
                AnimatePanel(opening, distX, duration);
            }
            else
            {
                // If user motion is slow or stopped we check if half distance is
                // traveled and based on that complete the motion
                bool halfway = Math.Abs(slidingPanel.TranslationX) >= (MeasuredWidth - panelPeekWidth) / 2;
                opening = opened ? !halfway : halfway;
                distX = CalculateDistance(opening);
                duration = (long)Math.Round(300 * Math.Abs(slidingPanel.TranslationX)
                                             / (double)(MeasuredWidth - panelPeekWidth));
            }

            AnimatePanel(opening, distX, duration);
        }

        public void AnimatePanel(bool opening, float distX, long duration)
        {
            ObjectAnimator slidingPanelAnimator = ObjectAnimator.OfFloat(slidingPanel, "TranslationX",
                slidingPanel.TranslationX, slidingPanel.TranslationX + distX);
            ObjectAnimator bottomPanelAnimator = ObjectAnimator.OfFloat(bottomPanel, "TranslationX",
                bottomPanel.TranslationX, bottomPanel.TranslationX + (float)(distX * parallaxFactor));

            AnimatorSet set = new AnimatorSet();
            set.PlayTogether(slidingPanelAnimator, bottomPanelAnimator);
            set.SetDuration(duration);
            set.SetInterpolator(sDecelerator);
            set.AddListener(new MyAnimListener(opening, this));
            set.Start();
        }

        #region Tools

        public bool CheckPanelTouch(MotionEvent e)
        {
            int x = (int)e.GetX();
            int y = (int)e.GetY();
            bool a = x > slidingPanel.Left + panelPeekWidth  &&
                y > slidingPanel.Top &&
                y < slidingPanel.Top + markerHeight;
            bool b =
               (y > slidingPanel.Top + panelPeekPadding ) &&
                y < slidingPanel.Top + panelPeekPadding + panelPeekHeight &&
                x > slidingPanel.Left - 40 &&
                x < slidingPanel.Left + panelPeekWidth + panelPeekWidth;

            return a || b;
        }

        public void ObtainVelocityTracker()
        {
            if (velocityTracker == null)
            {
                velocityTracker = VelocityTracker.Obtain();
            }
        }

        public float CalculateDistance(bool opening)
        {
            float distX;
            if (opened)
            {
                distX = opening
                    ? -slidingPanel.TranslationX
                    : MeasuredWidth - panelPeekWidth
                      - slidingPanel.TranslationX;
            }
            else
            {
                distX = opening
                    ? -(MeasuredWidth - panelPeekWidth + slidingPanel.TranslationX)
                    : -slidingPanel.TranslationX;
            }
            return distX;
        }


        public float BoundTranslation(float translation)
        {
            if (!opened)
            {
                if (translation > 0)
                {
                    translation = 0;
                }
                if (Math.Abs(translation) >= slidingPanel.MeasuredWidth - panelPeekWidth)
                {
                    translation = -slidingPanel.MeasuredWidth + panelPeekWidth;
                }
            }
            else
            {
                if (translation < 0)
                {
                    translation = 0;
                }
                if (translation >= slidingPanel.MeasuredWidth - panelPeekWidth)
                {
                    translation = slidingPanel.MeasuredWidth - panelPeekWidth;
                }
            }
            return translation;
        }

        #endregion

        class MyAnimListener : Java.Lang.Object, IAnimatorListener
        {

            LayerType oldLayerTypeOne;
            LayerType oldLayerTypeTwo;

            private FrameWithDragPanelLayout instance;
            bool opening;


            public MyAnimListener(bool opening, FrameWithDragPanelLayout instance)
                : base()
            {
                this.instance = instance;
                this.opening = opening;
            }

            public void OnAnimationStart(Animator animation)
            {
                oldLayerTypeOne = slidingPanel.LayerType;
                oldLayerTypeTwo = bottomPanel.LayerType;

                slidingPanel.SetLayerType(LayerType.Hardware, null);
                bottomPanel.SetLayerType(LayerType.Hardware, null);
                
                if (willDrawShadow)
                {
                    animating = true;
                    ViewCompat.PostInvalidateOnAnimation(instance);
                }
            }

            public void OnAnimationRepeat(Animator animation)
            {
            }

            public void OnAnimationEnd(Animator animation)
            {
                instance.SetOpenedState(opening);

                slidingPanel.TranslationX = 0;

                slidingPanel.SetLayerType(oldLayerTypeOne, null);
                bottomPanel.SetLayerType(oldLayerTypeTwo, null);

                instance.RequestLayout();

                if (willDrawShadow)
                {
                    animating = false;
                    ViewCompat.PostInvalidateOnAnimation(instance);
                }
            }

            public void OnAnimationCancel(Animator animation)
            {
                if (willDrawShadow)
                {
                    animating = false;
                    ViewCompat.PostInvalidateOnAnimation(instance);
                }
            }

        }

        private void SetOpenedState(bool opened)
        {
            this.opened = opened;
            AllowShadow();
        }

        private void AllowShadow()
        {
            willDrawShadow = shadowDrawable != null;
            SetWillNotDraw(!willDrawShadow);
        }
    }
}
