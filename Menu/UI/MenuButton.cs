using System;

namespace Testplate.Menu.UI
{
    public class MenuButton
    {
        public string Text { get; set; }
        public Action OnClick { get; set; }
        public Action OnEnable { get; set; }
        public Action OnDisable { get; set; }
        public Func<bool> IsEnabledCheck { get; set; }
        public bool IsToggle { get; set; }
        
        public bool IsSlider { get; set; }
        public float SliderValue { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public Action<float> OnSliderChange { get; set; }

        public MenuButton(string text, Action onClick)
        {
            Text = text;
            OnClick = onClick;
            IsToggle = false;
        }

        public MenuButton(string text, Action onEnable, Action onDisable, Func<bool> checkEnabled)
        {
            Text = text;
            OnEnable = onEnable;
            OnDisable = onDisable;
            IsEnabledCheck = checkEnabled;
            IsToggle = true;
        }

        public MenuButton(string text, float min, float max, float start, Action<float> onChange)
        {
            Text = text;
            MinValue = min;
            MaxValue = max;
            SliderValue = start;
            OnSliderChange = onChange;
            IsSlider = true;
            IsToggle = false;
        }

        public bool IsEnabled()
        {
            if (IsEnabledCheck != null) return IsEnabledCheck();
            return false;
        }

        public void HandleClick()
        {
            if (IsSlider) return;

            if (IsToggle)
            {
                if (IsEnabled()) OnDisable?.Invoke();
                else OnEnable?.Invoke();
            }
            else
            {
                OnClick?.Invoke();
            }
        }
    }
}
