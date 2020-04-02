using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Hardcodet.Wpf.TaskbarNotification;
using MicMute.Logic;
using MicMute.Observables;
using MicMute.Resources;
using NAudio.CoreAudioApi;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
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
        private WindowState windowState;
        private ICommand closeCommand;

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
            this._core.OnVolumeNotification += (data) => UpdateMicProperties();

            foreach (MMDevice device in this._core.Devices)
            {
                this.AudioEndPoints.Add(new AudioObservable(device));
            }
        }

        public ObservableCollection<AudioObservable> AudioEndPoints { get; } = new ObservableCollection<AudioObservable>();

        public bool IsMuted
        {
            get => this._core.AreAllMicsMuted;
            set
            {
                this._core.SwitchMicState(value ? MicStates.Muted : MicStates.Unmuted);

                this.UpdateMicProperties();

                if (this.IsMuted)
                {
                    ShowBalloonAction(Properties.Resources.TooltipTitle, Properties.Resources.TooltipMuted, BalloonIcon.Info);
                }
                else
                {
                    ShowBalloonAction(Properties.Resources.TooltipTitle, Properties.Resources.TooltipUnmuted, BalloonIcon.Info);
                }
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

        public ICommand CloseCommand => this.closeCommand ?? (this.closeCommand = 
            new RelayCommand(() =>
            {
                this.CanClose = true;
                //this.CloseTrigger = true;
            }));

    }
}