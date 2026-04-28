using UnityEngine;

namespace Backend.Systems.Panel
{
    [CreateAssetMenu(menuName = "Systems/Panel/Data", fileName = "PanelData", order = 0)]
    public class PanelData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private PanelBase _prefab;

        public string Id => _id;
        public PanelBase Prefab => _prefab;
    }
}
