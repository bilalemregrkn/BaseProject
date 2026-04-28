using UnityEngine;

namespace Backend.Systems.Component
{
    public class BaseCanvas : SmartComponent
    {
        private Canvas myCanvas => Get<Canvas>();
    }
}