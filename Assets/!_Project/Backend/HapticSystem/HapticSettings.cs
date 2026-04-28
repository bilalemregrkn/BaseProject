using UnityEngine;

namespace Backend.Systems.Haptic
{
    [CreateAssetMenu(menuName = "Game/Haptic Settings", fileName = "HapticSettings")]
    public class HapticSettings : ScriptableObject
    {
        [SerializeField] private bool _defaultEnabled = true;

        public bool DefaultEnabled => _defaultEnabled;
    }
}
