using Backend.Systems.Component;
using Cysharp.Threading.Tasks;

namespace Backend.Systems.Panel
{
    public class PanelGameSetting : PanelBase
    {
        public BaseButton buttonClose;

        protected override void Awake()
        {
            base.Awake();
            buttonClose.MyButton.onClick.AddListener(OnClickClose);
        }

        private void OnClickClose()
        {
            HideAsync().Forget();
        }
    }
}