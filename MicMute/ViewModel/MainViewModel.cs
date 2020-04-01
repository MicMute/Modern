using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MicMute.Logic;
using MicMute.Observables;
using MicMute.Resources;
using NAudio.CoreAudioApi;
using System.Collections.ObjectModel;
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
            _core.OnVolumeNotification += (data) =>
            {
                RaisePropertyChanged(() => this.IsMuted);
                RaisePropertyChanged(() => this.MicImage);
            };

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
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(() => this.MicImage);
            }
        }

        public ImageSource MicImage => this.IsMuted ? ImageResources.Muted.Source : ImageResources.Unmuted.Source;

        public ICommand ToggleMuteCommand => this._toggleMuteCommand ?? (this._toggleMuteCommand = new RelayCommand(this.ToggleMuteState));

        private void ToggleMuteState()
        {
            this.IsMuted = !this.IsMuted;
        }
    }
}