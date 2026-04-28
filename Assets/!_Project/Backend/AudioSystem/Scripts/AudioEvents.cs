using Backend.Systems.EventBus;

namespace Backend.Systems.Audio
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
