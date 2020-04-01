using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicMute.Logic
{

    public class Core
    {
        public delegate void VolumeNotificationEvent(AudioVolumeNotificationData data);
        public event VolumeNotificationEvent OnVolumeNotification;

        private readonly List<MMDevice> _devices;
        private MicStates _oldState;

        public Core()
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            _devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToList();
            
            foreach (MMDevice device in _devices)
            {
                device.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
            }

            _oldState = this.AreAllMicsMuted ? MicStates.Muted : MicStates.Unmuted;
        }

        public IReadOnlyList<MMDevice> Devices => _devices;

        void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            OnVolumeNotification?.Invoke(data);
        }

        public void SwitchMicState(MicStates state)
        {
            foreach (MMDevice device in _devices)
            {
                try
                {
                    device.AudioEndpointVolume.Mute = state == MicStates.Muted;
                }
                catch
                {
                    //We don't care about it beeing set or not.
                    //Sometimes, it doesn't work.
                }
            }
        }

        public bool AreAllMicsMuted => _devices.All(d => d.AudioEndpointVolume.Mute);
    }
}
