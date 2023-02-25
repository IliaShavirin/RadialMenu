using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BaseProj.SharpVectors.SharpVectorRuntime
{
    /// <summary>
    ///     This provides a wrapper for the Scoreboard, which is used for opacity animation.
    /// </summary>
    public sealed class SvgAnimator : FrameworkElement
    {
        #region Constructors and Destructor

        public SvgAnimator()
        {
            NameScope.SetNameScope(this, new NameScope());

            _opacityAnimation = new DoubleAnimation();
            _opacityAnimation.From = 0.9;
            _opacityAnimation.To = 0.0;
            _opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            _opacityAnimation.AutoReverse = false;
            _opacityAnimation.RepeatBehavior = RepeatBehavior.Forever;

            _storyboard = new Storyboard();

            _storyboard.Children.Add(_opacityAnimation);

            Storyboard.SetTargetProperty(_opacityAnimation, new PropertyPath(
                Brush.OpacityProperty));
        }

        #endregion

        #region Private Fields

        private readonly Storyboard _storyboard;
        private readonly DoubleAnimation _opacityAnimation;

        #endregion

        #region Public Properties

        public bool IsAnimating { get; private set; }

        public string TargetName { get; private set; }

        #endregion

        #region Public Methods

        public void Start(string targetName, object scopedElement)
        {
            if (string.IsNullOrEmpty(targetName)) return;

            if (IsAnimating) Stop();
            RegisterName(targetName, scopedElement);

            Storyboard.SetTargetName(_opacityAnimation, targetName);

            _storyboard.Begin(this, true);

            TargetName = targetName;
            IsAnimating = true;
        }

        public void Stop()
        {
            _storyboard.Stop(this);

            if (!string.IsNullOrEmpty(TargetName)) UnregisterName(TargetName);

            IsAnimating = false;
            TargetName = null;
        }

        #endregion
    }
}