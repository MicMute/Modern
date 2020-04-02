using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Hardcodet.Wpf.TaskbarNotification;
using MicMute.Logic;
using MicMute.Observables;
using MicMute.Resources;
using NAudio.CoreAudioApi;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MicMute.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly Core _core;
        private ICommand _toggleMuteCommand;
        private ICommand toggleWindowStateCommand;
        private ICommand exitApplicationCommand;
        private WindowState windowState;
        private MicStates? forcedState;
        private bool _closeTrigger;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(Core core)
        {
            if (this.IsInDesignMode)
            {
                // code runs in Blend
                return;
            }

            this._core = core;
            this._core.OnVolumeNotification += (data) =>
            {
                Application.Current.Dispatcher.Invoke(() => this.CheckForcedState(data));
                this.UpdateMicProperties();
            };

            foreach (MMDevice device in this._core.Devices)
            {
                this.AudioEndPoints.Add(new AudioObservable(device));
            }
        }

        private void CheckForcedState(AudioVolumeNotificationData data)
        {
            if (!this.IsForced)
            {
                return;
            }

            var current = data.Muted ? MicStates.Muted : MicStates.Unmuted;
            if (this.forcedState.Value != current)
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    Thread.Sleep(100);
                    this._core.SwitchMicState(this.forcedState.Value);
                    this.ShowBalloonAction(Properties.Resources.TooltipTitle, Properties.Resources.TooltipStateForced, BalloonIcon.Info);
                }));
            }
        }

        public ObservableCollection<AudioObservable> AudioEndPoints { get; } = new ObservableCollection<AudioObservable>();

        public bool IsMuted
        {
            get => this._core.AreAllMicsMuted;
            set
            {
                if (this.IsForced)
                {
                    this.forcedState = value ? MicStates.Muted : MicStates.Unmuted;
                }

                this._core.SwitchMicState(value ? MicStates.Muted : MicStates.Unmuted);

                this.UpdateMicProperties();

                this.ShowBalloonAction(
                    Properties.Resources.TooltipTitle,
                    this.IsMuted ? Properties.Resources.TooltipMuted : Properties.Resources.TooltipUnmuted,
                    BalloonIcon.Info);
            }
        }

        public bool IsForced
        {
            get => this.forcedState.HasValue;
            set
            {
                this.forcedState = value ? (this.IsMuted ? MicStates.Muted : MicStates.Unmuted) : (MicStates?)null;
                this.RaisePropertyChanged();
            }
        }

        private void UpdateMicProperties()
        {
            this.RaisePropertyChanged(() => this.IsMuted);
            this.RaisePropertyChanged(() => this.MicImage);
            this.RaisePropertyChanged(() => this.TaskbarIcon);
        }

        public ImageSource MicImage => this.IsMuted ? ImageResources.Muted.Source : ImageResources.Unmuted.Source;

        public ICommand ToggleMuteCommand => this._toggleMuteCommand ?? (this._toggleMuteCommand = new RelayCommand(() => this.IsMuted = !this.IsMuted));

        public ICommand ToggleWindowStateCommand
            => this.toggleWindowStateCommand ?? (this.toggleWindowStateCommand = new RelayCommand(this.ToggleWindowState));

        private void ToggleWindowState()
        {
            this.WindowState = this.WindowState != WindowState.Minimized ? WindowState.Minimized : WindowState.Normal;
        }

        public WindowState WindowState
        {
            get => this.windowState;
            set => this.Set(ref this.windowState, value);
        }

        public ImageSource TaskbarIcon => this.IsMuted ? ImageResources.IconMutedImage.Source : ImageResources.IconUnmutedImage.Source;

        public Action<string, string, BalloonIcon> ShowBalloonAction { get; set; }

        /// <summary>
        /// States whether the program can be closed.
        /// </summary>
        public bool CanClose { get; private set; }

        /// <summary>
        /// Setting this to true closes the window through CloseWindowBehavior.
        /// </summary>
        public bool CloseTrigger
        {
            get => this._closeTrigger;
            private set => this.Set(ref this._closeTrigger, value);
        }

        public ICommand ExitApplicationCommand => this.exitApplicationCommand ?? (this.exitApplicationCommand =
            new RelayCommand(() =>
            {
                this.CanClose = true;
                this.CloseTrigger = true;
            }));

    }
}