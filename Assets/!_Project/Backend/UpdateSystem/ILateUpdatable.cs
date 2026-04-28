namespace Backend.Systems.Update
{
    public interface ILateUpdatable
    {
        void LateTick(float dt);
    }
}