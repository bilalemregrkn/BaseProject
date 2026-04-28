using UnityEngine;
using UnityEngine.UI;

namespace Tools.SmartComponent
{
    public class BaseButton : SmartComponent
    {
        public Button MyButton => Get<Button>();

        private void Awake()
        {
            MyButton.onClick.AddListener(Click);
        }

        protected virtual void Click()
        {
            Debug.Log("BaseButton clicked");
        }
    }
}