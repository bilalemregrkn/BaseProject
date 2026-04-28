using Cysharp.Threading.Tasks;

namespace Backend.Systems.Panel
{
    public interface IPanelService
    {
        void Register(string id, IPanel panel);
        void Unregister(string id);
        UniTask ShowAsync(string id);
        UniTask HideAsync(string id);
        UniTask ShowExclusiveAsync(string id);
        bool IsVisible(string id);
    }
}
