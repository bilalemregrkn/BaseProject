using Backend.Systems.Component;
using UnityEngine;

namespace Backend.Systems.Haptic
{
    [CreateAssetMenu(menuName = "Systems/Haptic/Settings", fileName = "HapticSettings")]
    public class HapticSettings : BaseServiceSetting
    {
        [SerializeField] private bool _defaultEnabled = true;

        public bool DefaultEnabled => _defaultEnabled;
    }
}
