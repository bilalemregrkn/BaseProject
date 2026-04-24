using Plugins.EventBus;

namespace Plugins.AudioService
{
    public struct VolumeChanged : IEvent
    {
        public float OldVolume;
        public float NewVolume;
    }

    public struct MuteChanged : IEvent
    {
        public bool Muted;
    }
}
