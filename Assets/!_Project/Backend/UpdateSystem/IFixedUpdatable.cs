namespace Backend.Systems.Update
{
    public interface IFixedUpdatable
    {
        void FixedTick(float fdt);
    }
}