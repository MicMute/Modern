using GalaSoft.MvvmLight;
using NAudio.CoreAudioApi;

namespace MicMute.Observables
{
    public class AudioObservable : ObservableObject
    {
        private bool _selected;

        public AudioObservable(MMDevice device)
        {
            this.Device = device;

            device.AudioEndpointVolume.OnVolumeNotification += this.HandleVolumeNotification;
        }

        private void HandleVolumeNotification(AudioVolumeNotificationData data)
        {
            this.IsMuted = data.Muted;
        }

        public string FriendlyName => this.Device.FriendlyName;

        public bool Selected
        {
            get => this._selected;
            set => this.Set(ref this._selected, value);
        }

        public bool IsMuted
        {
            get => this.Device.AudioEndpointVolume.Mute;
            set
            {
                this.Device.AudioEndpointVolume.Mute = value;
                this.RaisePropertyChanged();
            }
        }

        public MMDevice Device { get; }
    }
}
