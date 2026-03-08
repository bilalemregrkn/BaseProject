using UnityEngine;
using UnityEngine.UI;

namespace Tools.SmartComponent
{
    public class BaseButton : SmartComponent
    {
        private Button myButton => Get<Button>();

        private void Awake()
        {
            myButton.onClick.AddListener(Click);
        }

        protected virtual void Click()
        {
            Debug.Log("BaseButton clicked");
        }
    }
}