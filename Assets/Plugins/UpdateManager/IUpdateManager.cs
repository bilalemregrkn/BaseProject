namespace Plugins.UpdateManager
{
    public interface IUpdateManager
    {
        void Add(IUpdatable updatable);
        void Remove(IUpdatable updatable);

        void Add(ILateUpdatable updatable);
        void Remove(ILateUpdatable updatable);

        void Add(IFixedUpdatable updatable);
        void Remove(IFixedUpdatable updatable);
    }
}