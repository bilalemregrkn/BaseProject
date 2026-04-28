namespace Backend.Systems.Reward
{
    public interface IRewardService
    {
        void Grant(string type, int amount);
        bool TryClaim(string type, out int amount);
        bool HasPending(string type);
    }
}
