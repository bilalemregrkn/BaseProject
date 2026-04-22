using UnityEngine;

namespace Tools.SmartComponent
{
    public class BaseCanvas : SmartComponent
    {
        private Canvas myCanvas => Get<Canvas>();
    }
}