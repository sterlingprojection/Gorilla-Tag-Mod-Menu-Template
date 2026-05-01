using UnityEngine;
using TMPro;
using System;

namespace Testplate.Menu.UI
{
    public class PhysicalButton : MonoBehaviour
    {
        public Action onClick;
        public TextMeshProUGUI associatedText;
        public MenuButton associatedMenuButton;
        public Color activeColor = Color.green;
        public Color inactiveColor = Color.white;
        public Color highlightColor = Color.yellow;
        public bool isToggled = false;
        
        public void OnHover()
        {
            if (associatedText != null)
            {
                associatedText.color = highlightColor;
            }
        }

        public void OnUnhover()
        {
            UpdateVisuals(isToggled);
        }

        public void Select()
        {
            onClick?.Invoke();
        }

        public void UpdateVisuals(bool toggled)
        {
            isToggled = toggled;
            if (associatedText != null)
            {
                associatedText.color = isToggled ? activeColor : inactiveColor;
            }
        }
    }
}
