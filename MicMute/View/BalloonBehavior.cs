using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Drawing;
using System.Windows;

namespace MicMute
{
    public class BalloonBehavior : System.Windows.Interactivity.Behavior<TaskbarIcon>
    {
        #region Static Fields

        public static readonly DependencyProperty ShowBalloonCustomIconActionProperty =
            DependencyProperty.Register(nameof(ShowBalloonCustomIconAction), typeof(Action<string, string, Icon>), typeof(BalloonBehavior),
                                        new PropertyMetadata(null));

        public static readonly DependencyProperty ShowBalloonActionProperty =
            DependencyProperty.Register(nameof(ShowBalloonAction), typeof(Action<string, string, BalloonIcon>), typeof(BalloonBehavior),
                                        new PropertyMetadata(null));

        #endregion

        #region Public Properties

        public Action<string, string, BalloonIcon> ShowBalloonAction
        {
            get => (Action<string, string, BalloonIcon>)this.GetValue(ShowBalloonActionProperty);
            set => this.SetValue(ShowBalloonActionProperty, value);
        }

        public Action<string, string, Icon> ShowBalloonCustomIconAction
        {
            get => (Action<string, string, Icon>)this.GetValue(ShowBalloonCustomIconActionProperty);
            set => this.SetValue(ShowBalloonCustomIconActionProperty, value);
        }

        #endregion

        #region Methods

        protected override void OnAttached()
        {
            this.SetCurrentValue(ShowBalloonActionProperty, (Action<string, string, BalloonIcon>)this.ShowBalloon);
            this.SetCurrentValue(ShowBalloonCustomIconActionProperty, (Action<string, string, Icon>)this.ShowBalloonCustomIcon);

            base.OnAttached();
        }

        private void ShowBalloon(string title, string message, BalloonIcon icon)
        {
            this.AssociatedObject.ShowBalloonTip(title, message, icon);
        }

        private void ShowBalloonCustomIcon(string title, string message, Icon icon)
        {
            this.AssociatedObject.ShowBalloonTip(title, message, icon);
        }

        #endregion
    }
}