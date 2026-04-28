using UnityEngine.UI;

namespace Backend.Systems.Component
{
    public class BaseImage : SmartComponent
    {
        private Image myImage => Get<Image>();
    }
}