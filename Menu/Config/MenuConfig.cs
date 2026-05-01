using UnityEngine;

namespace Testplate.Menu.Config
{
    public static class MenuConfig
    {
        public static Vector3 menuSize = new Vector3(0.1f, 0.3f, 0.3825f);
        public static Vector3 menubackgroundSize = new Vector3(0.05f, 1.1f, 1.1f);
        
        public static Vector3 buttonScale = new Vector3(0.09f, 0.9f, 0.08f);
        
        public static Color MenuBackgroundColor = new Color(0.05f, 0.05f, 0.05f, 0.9f);
        public static Color ButtonColor = Color.black;
        public static Color ToggledButtonColor = Color.green;
        public static Color TextColor = Color.white;

        public static Vector3 MenuPosition = new Vector3(0f, 0.15f, 0f);
        public static Vector3 MenuRotation = new Vector3(0f, -90f, 0f);
    }
}
