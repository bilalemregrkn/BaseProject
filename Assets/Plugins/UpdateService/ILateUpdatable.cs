namespace Plugins.UpdateService
{
    public interface ILateUpdatable
    {
        void LateTick(float dt);
    }
}