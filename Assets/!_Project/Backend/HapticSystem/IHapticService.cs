namespace Backend.Systems.Haptic
{
    public interface IHapticService
    {
        bool Enabled { get; set; }

        void Light();
        void Medium();
        void Heavy();
        void Selection();
    }
}
