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

namespace S_Calc.Common.Controls
{
    public class FrameWithDragPanelLayout : FrameLayout
    {
        private static DecelerateInterpolator sDecelerator = new DecelerateInterpolator();

        private int panelPeekHeight = 200;

        private bool opened;
        private bool touching;
        private bool isBeingDragged;
        private static bool willDrawShadow = true;

        private int touchSlop;
        private float parallaxFactor = 0.2f;
        private float touchX;

        private VelocityTracker velocityTracker = null;

        private static View bottomPanel;
        private static View slidingPanel;

        private Drawable shadowDrawable;

        private static bool animating = false;

        public FrameWithDragPanelLayout(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public FrameWithDragPanelLayout(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
            
        }

        private void Initialize()
        {
            touchSlop = ViewConfiguration.Get(Context).ScaledTouchSlop;
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);

            if (ChildCount != 2)
            {
                throw new InvalidProgramException("DraggedPanelLayout must have 2 children!");
            }

            bottomPanel = GetChildAt(0);
            bottomPanel.Layout(left, top, right, bottom);

            slidingPanel = GetChildAt(1);
            if (!opened)
            {
                int panelMeasuredWidth = slidingPanel.MeasuredWidth;
                slidingPanel.Layout(left - panelPeekHeight + panelMeasuredWidth,
                    (int) (top + (bottom / 2.5)),
                    right,
                    (int) (bottom - (bottom / 10)));

            }
        }

        //TODO: Draw shadow
        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);

            if (!IsInEditMode && willDrawShadow)
            {
                int left = (int)(slidingPanel.Left + slidingPanel.TranslationX);
                shadowDrawable.SetBounds(left - shadowDrawable.IntrinsicWidth, MeasuredHeight, left, MeasuredWidth);
                shadowDrawable.Draw(canvas);

            }
            if (animating)
            {
                ViewCompat.PostInvalidateOnAnimation(this);
            }
        }
        public override bool OnTouchEvent(MotionEvent e)
        {
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
                        .TranslationX = (float) (opened
                        ? -(MeasuredWidth - panelPeekHeight - translation)
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

            bottomPanel.Visibility = ViewStates.Visible;


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
                duration = (long) Math.Abs(Math.Round(distX / velocityX));
                AnimatePanel(opening, distX, duration);
            }
            else
            {
                // If user motion is slow or stopped we check if half distance is
                // traveled and based on that complete the motion
                bool halfway = Math.Abs(slidingPanel.TranslationX) >= (MeasuredWidth - panelPeekHeight) / 2;
                opening = opened ? !halfway : halfway;
                distX = CalculateDistance(opening);
                duration = (long) Math.Round(300 * Math.Abs(slidingPanel.TranslationX)
                                             / (double) (MeasuredWidth - panelPeekHeight));
            }

            AnimatePanel(opening, distX, duration);
        }

        public void AnimatePanel(bool opening, float distX, long duration)
        {
            ObjectAnimator slidingPanelAnimator = ObjectAnimator.OfFloat(slidingPanel, "translationX",
                slidingPanel.TranslationX, slidingPanel.TranslationX + distX);
            ObjectAnimator bottomPanelAnimator = ObjectAnimator.OfFloat(bottomPanel, "translationX",
                bottomPanel.TranslationX, bottomPanel.TranslationX + (float) (distX * parallaxFactor));

            AnimatorSet set = new AnimatorSet();
            set.PlayTogether(slidingPanelAnimator, bottomPanelAnimator);
            set.SetDuration(duration);
            set.SetInterpolator(sDecelerator);
            set.AddListener(new MyAnimListener(opening, this));
            set.Start();
        }

        #region Tools

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
                    : MeasuredWidth - panelPeekHeight
                      - slidingPanel.TranslationX;
            }
            else
            {
                distX = opening
                    ? -(MeasuredWidth - panelPeekHeight + slidingPanel.TranslationX)
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
                if (Math.Abs(translation) >= slidingPanel.MeasuredWidth - panelPeekHeight)
                {
                    translation = -slidingPanel.MeasuredWidth + panelPeekHeight;
                }
            }
            else
            {
                if (translation < 0)
                {
                    translation = 0;
                }
                if (translation >= slidingPanel.MeasuredWidth - panelPeekHeight)
                {
                    translation = slidingPanel.MeasuredWidth - panelPeekHeight;
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
                oldLayerTypeOne = bottomPanel.LayerType;

                slidingPanel.SetLayerType(LayerType.Hardware, null);
                bottomPanel.SetLayerType(LayerType.Hardware, null);

                bottomPanel.Visibility = ViewStates.Visible;

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

                bottomPanel.TranslationX = 0;
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
            bottomPanel.Visibility = opened ? ViewStates.Gone : ViewStates.Visible;
            HideShadowIfNotNeeded();
        }

        private void AllowShadow()
        {
            willDrawShadow = shadowDrawable != null;
            SetWillNotDraw(!willDrawShadow);
        }

        private void HideShadowIfNotNeeded()
        {
            willDrawShadow = shadowDrawable != null && !opened;
            SetWillNotDraw(!willDrawShadow);
        }

    }
}
