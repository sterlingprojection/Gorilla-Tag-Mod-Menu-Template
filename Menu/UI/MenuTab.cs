using System.Collections.Generic;

namespace Testplate.Menu.UI
{
    public class MenuTab
    {
        public string Name { get; set; }
        public List<MenuButton> Buttons { get; set; } = new List<MenuButton>();

        public MenuTab(string name)
        {
            Name = name;
        }

        public void AddButton(MenuButton button)
        {
            Buttons.Add(button);
        }
    }
}
