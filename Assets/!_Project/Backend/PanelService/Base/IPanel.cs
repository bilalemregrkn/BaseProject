using Cysharp.Threading.Tasks;

namespace Plugins.PanelService
{
    public interface IPanel
    {
        bool IsVisible { get; }
        UniTask ShowAsync();
        UniTask HideAsync();
    }
}
