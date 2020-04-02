using System.Windows;

namespace MicMute
{
    public class CloseWindowBehavior : System.Windows.Interactivity.Behavior<Window>
    {
        #region Static Fields

        public static readonly DependencyProperty CloseTriggerProperty =
            DependencyProperty.Register(
                nameof(CloseTrigger),
                typeof(bool),
                typeof(CloseWindowBehavior),
                new PropertyMetadata(false, OnCloseTriggerChanged));
		
        #endregion

        #region Public Properties

        public bool CloseTrigger
        {
            get => (bool)this.GetValue(CloseTriggerProperty);
            set => this.SetValue(CloseTriggerProperty, value);
        }

        #endregion

        #region Methods

        private static void OnCloseTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as CloseWindowBehavior;

            if (behavior != null)
            {
                behavior.OnCloseTriggerChanged();
            }
        }

        private void OnCloseTriggerChanged()
        {
            // when closetrigger is true, close the window
            if (this.CloseTrigger)
            {
                this.AssociatedObject?.Close();
            }
        }

        #endregion
    }
}