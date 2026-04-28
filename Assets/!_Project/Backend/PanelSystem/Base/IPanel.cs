using Cysharp.Threading.Tasks;

namespace Backend.Systems.Panel
{
    public interface IPanel
    {
        bool IsVisible { get; }
        UniTask ShowAsync();
        UniTask HideAsync();
    }
}
