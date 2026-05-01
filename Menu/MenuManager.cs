using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Testplate.Menu.Config;
using Testplate.Menu.UI;
using GorillaLocomotion;
using System.Collections.Generic;
using System.Linq;

namespace Testplate.Menu
{
    public class MenuManager : MonoBehaviour
    {
        public static GameObject menuInstance;
        public static GameObject canvasInstance;
        public static GameObject selectionPointer;
        public static LineRenderer selectionLine;
        public static int currentTab = 0;
        public static int currentPage = 0;
        public static int buttonsPerPage = 6;
        
        private static float lastToggleTime = 0f;
        private static float lastClickTime = 0f;
        private const float toggleCooldown = 0.2f;
        private const float clickCooldown = 0.1f;
        private static PhysicalButton lastHoveredButton;
        private static PhysicalButton activeSlider;

        private const int MENU_LAYER = 18; 

        public static void Initialize()
        {
            FeatureRegistry.Initialize();
        }

        public static void OnUpdate()
        {
            bool controllerToggle = ControllerInputPoller.instance != null && ControllerInputPoller.instance.rightControllerSecondaryButton;
            bool keyboardToggle = Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame;

            if ((keyboardToggle || controllerToggle) && Time.time > lastToggleTime + toggleCooldown)
            {
                lastToggleTime = Time.time;
                ToggleMenu();
            }

            if (menuInstance != null && menuInstance.activeSelf)
            {
                HandleSelection();
            }
            else
            {
                CleanupSelection();
            }

            FeatureRegistry.UpdateFeatures();
        }

        private static void HandleSelection()
        {
            if (selectionPointer == null)
            {
                selectionPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                selectionPointer.name = "SelectionPointer";
                selectionPointer.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                Destroy(selectionPointer.GetComponent<Collider>());
                var renderer = selectionPointer.GetComponent<Renderer>();
                renderer.material.shader = Shader.Find("GorillaTag/UberShader") ?? Shader.Find("Standard");
                renderer.material.color = Color.white;

                GameObject lineObj = new GameObject("SelectionLine");
                selectionLine = lineObj.AddComponent<LineRenderer>();
                selectionLine.startWidth = 0.005f;
                selectionLine.endWidth = 0.005f;
                selectionLine.material = new Material(Shader.Find("Sprites/Default"));
                selectionLine.startColor = Color.white;
                selectionLine.endColor = Color.white;
            }

            selectionPointer.SetActive(true);
            selectionLine.enabled = true;

            Transform hand = GorillaTagger.Instance.rightHandTransform;
            Ray ray = new Ray(hand.position, hand.forward);
            RaycastHit hit;

            selectionLine.SetPosition(0, hand.position);

            int layerMask = 1 << MENU_LAYER;

            bool isSelecting = ControllerInputPoller.instance != null && ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f;

            if (activeSlider != null)
            {
                if (!isSelecting)
                {
                    activeSlider = null;
                }
                else if (Physics.Raycast(ray, out hit, 100f, layerMask))
                {
                    UpdateSliderValue(activeSlider, hit.point);
                }
            }

            if (Physics.Raycast(ray, out hit, 100f, layerMask))
            {
                selectionPointer.transform.position = hit.point;
                selectionLine.SetPosition(1, hit.point);

                PhysicalButton btn = hit.collider.GetComponent<PhysicalButton>();
                if (btn != null)
                {
                    if (lastHoveredButton != null && lastHoveredButton != btn)
                        lastHoveredButton.OnUnhover();

                    btn.OnHover();
                    lastHoveredButton = btn;

                    if (isSelecting && Time.time > lastClickTime + clickCooldown)
                    {
                        if (btn.associatedMenuButton != null && btn.associatedMenuButton.IsSlider)
                        {
                            activeSlider = btn;
                            UpdateSliderValue(btn, hit.point);
                        }
                        else
                        {
                            lastClickTime = Time.time;
                            btn.Select();
                            DrawUI();
                        }
                    }
                }
                else if (lastHoveredButton != null)
                {
                    lastHoveredButton.OnUnhover();
                    lastHoveredButton = null;
                }
            }
            else
            {
                selectionPointer.transform.position = hand.position + hand.forward * 2f;
                selectionLine.SetPosition(1, hand.position + hand.forward * 2f);
                if (lastHoveredButton != null)
                {
                    lastHoveredButton.OnUnhover();
                    lastHoveredButton = null;
                }
            }
        }

        private static void UpdateSliderValue(PhysicalButton btn, Vector3 hitPoint)
        {
            if (btn.associatedMenuButton == null || !btn.associatedMenuButton.IsSlider) return;

            Vector3 localHit = btn.transform.InverseTransformPoint(hitPoint);
            float normalized = Mathf.Clamp01(localHit.z + 0.5f);
            btn.associatedMenuButton.SliderValue = Mathf.Lerp(btn.associatedMenuButton.MinValue, btn.associatedMenuButton.MaxValue, normalized);
            btn.associatedMenuButton.OnSliderChange?.Invoke(btn.associatedMenuButton.SliderValue);
            
            if (btn.associatedText != null)
            {
                btn.associatedText.text = $"{btn.associatedMenuButton.Text}: {btn.associatedMenuButton.SliderValue:F1}";
            }
        }

        private static void CleanupSelection()
        {
            if (selectionPointer != null) selectionPointer.SetActive(false);
            if (selectionLine != null) selectionLine.enabled = false;
            activeSlider = null;
            if (lastHoveredButton != null)
            {
                lastHoveredButton.OnUnhover();
                lastHoveredButton = null;
            }
        }

        public static void ToggleMenu()
        {
            if (menuInstance == null) CreateMenu();
            else 
            {
                menuInstance.SetActive(!menuInstance.activeSelf);
                if (menuInstance.activeSelf)
                {
                    currentPage = 0;
                    DrawUI();
                }
            }
        }

        public static void CreateMenu()
        {
            menuInstance = new GameObject("TestplateRoot");
            menuInstance.transform.SetParent(GorillaTagger.Instance.leftHandTransform, false);
            menuInstance.transform.localPosition = new Vector3(-0.02f, 0.12f, 0f);
            menuInstance.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            menuInstance.transform.localScale = new Vector3(0.01f, 0.25f, 0.35f);

            GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bg.name = "MenuBackground";
            bg.layer = MENU_LAYER;
            Destroy(bg.GetComponent<Rigidbody>());
            
            var coll = bg.GetComponent<BoxCollider>();
            coll.isTrigger = false; 

            bg.transform.SetParent(menuInstance.transform, false);
            bg.transform.localScale = Vector3.one;
            bg.transform.localPosition = Vector3.zero;
            
            var bgRenderer = bg.GetComponent<Renderer>();
            bgRenderer.material.shader = Shader.Find("GorillaTag/UberShader") ?? Shader.Find("Standard");
            bgRenderer.material.color = new Color(0, 0, 0, 0.85f);

            canvasInstance = new GameObject("Canvas");
            canvasInstance.transform.SetParent(menuInstance.transform, false);
            var canvas = canvasInstance.AddComponent<Canvas>();
            canvas.renderMode = UnityEngine.RenderMode.WorldSpace;
            
            var rect = canvasInstance.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(1000f, 1000f); 
            rect.localPosition = new Vector3(-0.51f, 0f, 0f); 
            rect.localRotation = Quaternion.Euler(0f, 90f, 0f);
            rect.localScale = new Vector3(0.001f, 0.001f, 0.001f); 

            DrawUI();
        }

        public static void DrawUI()
        {
            if (canvasInstance == null) return;
            
            foreach (Transform child in canvasInstance.transform) Destroy(child.gameObject);
            foreach (Transform child in menuInstance.transform) 
                if (child.name.EndsWith("_Button")) Destroy(child.gameObject);

            CreateHeaderUI();

            float startY = 320f;
            float spacing = 110f;

            for (int i = 0; i < FeatureRegistry.Tabs.Count; i++)
            {
                int index = i;
                CreateRaycastButton(null, FeatureRegistry.Tabs[i].Name, new Vector2(-380f, startY - (i * spacing)), new Vector2(200f, 80f), () => {
                    currentTab = index;
                    currentPage = 0;
                    DrawUI();
                }, currentTab == index, true);
            }

            var activeTab = FeatureRegistry.Tabs[currentTab];
            int totalPages = Mathf.Max(1, Mathf.CeilToInt((float)activeTab.Buttons.Count / buttonsPerPage));
            var pageButtons = activeTab.Buttons.Skip(currentPage * buttonsPerPage).Take(buttonsPerPage).ToList();

            for (int i = 0; i < pageButtons.Count; i++)
            {
                var btn = pageButtons[i];
                string label = btn.IsSlider ? $"{btn.Text}: {btn.SliderValue:F1}" : btn.Text;
                CreateRaycastButton(btn, label, new Vector2(100f, startY - (i * spacing)), new Vector2(600f, 90f), () => {
                    btn.HandleClick();
                }, btn.IsEnabled(), false);
            }

            if (totalPages > 1)
            {
                CreateRaycastButton(null, "<", new Vector2(-150f, -420f), new Vector2(150f, 80f), () => {
                    currentPage = (currentPage > 0) ? currentPage - 1 : totalPages - 1;
                    DrawUI();
                }, false, true);
                
                CreateRaycastButton(null, ">", new Vector2(150f, -420f), new Vector2(150f, 80f), () => {
                    currentPage = (currentPage + 1) % totalPages;
                    DrawUI();
                }, false, true);
            }
        }

        private static void CreateHeaderUI()
        {
            GameObject headerObj = new GameObject("Header");
            headerObj.transform.SetParent(canvasInstance.transform, false);
            var rect = headerObj.AddComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, 450f);
            rect.sizeDelta = new Vector2(1000f, 100f);

            var txt = headerObj.AddComponent<TextMeshProUGUI>();
            txt.text = $"<color=yellow>TESTPLATE</color> | {FeatureRegistry.Tabs[currentTab].Name}";
            txt.alignment = TextAlignmentOptions.Center;
            txt.fontSize = 65f;
            txt.fontStyle = FontStyles.Bold;
        }

        private static void CreateRaycastButton(MenuButton associated, string text, Vector2 pos, Vector2 size, System.Action onClick, bool isActive, bool isSmall)
        {
            GameObject txtObj = new GameObject(text + "_Text");
            txtObj.transform.SetParent(canvasInstance.transform, false);
            var txt = txtObj.AddComponent<TextMeshProUGUI>();
            txt.text = text;
            txt.alignment = TextAlignmentOptions.Center;
            txt.fontSize = isSmall ? 40f : 55f;
            txt.color = isActive ? Color.green : Color.white;
            txt.raycastTarget = false;
            
            var txtRect = txtObj.GetComponent<RectTransform>();
            txtRect.anchoredPosition = pos;
            txtRect.sizeDelta = size;

            GameObject btnTrigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
            btnTrigger.name = text + "_Button";
            btnTrigger.layer = MENU_LAYER;
            Destroy(btnTrigger.GetComponent<Rigidbody>());
            Destroy(btnTrigger.GetComponent<MeshRenderer>());
            
            var collider = btnTrigger.GetComponent<BoxCollider>();
            collider.isTrigger = false; 
            
            btnTrigger.transform.SetParent(menuInstance.transform, false);
            
            float xPos = -0.55f; 
            float yPos = pos.y / 1000f; 
            float zPos = -pos.x / 1000f; 
            
            btnTrigger.transform.localPosition = new Vector3(xPos, yPos, zPos);
            btnTrigger.transform.localScale = new Vector3(0.1f, size.y / 1000f, size.x / 1000f);
            btnTrigger.transform.localRotation = Quaternion.identity;

            var script = btnTrigger.AddComponent<PhysicalButton>();
            script.associatedText = txt;
            script.associatedMenuButton = associated;
            script.onClick = onClick;
            script.activeColor = Color.green;
            script.inactiveColor = Color.white;
            script.UpdateVisuals(isActive);
        }
    }
}
