using UnityEngine.UI;

namespace Tools.SmartComponent
{
    public class BaseImage : SmartComponent
    {
        private Image myImage => Get<Image>();
    }
}