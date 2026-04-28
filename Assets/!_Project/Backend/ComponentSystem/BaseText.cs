namespace Backend.Systems.Component
{
    public class BaseText : SmartComponent
    {
        private TMPro.TextMeshProUGUI myText => Get<TMPro.TextMeshProUGUI>();

        public void SetText(string text)
        {
            myText.text = text;
        }
    }
}