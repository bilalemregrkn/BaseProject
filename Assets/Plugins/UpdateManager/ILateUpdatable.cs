namespace Plugins.UpdateManager
{
    public interface ILateUpdatable
    {
        void LateTick(float dt);
    }
}