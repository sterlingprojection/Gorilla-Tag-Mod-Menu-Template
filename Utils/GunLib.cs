using UnityEngine;
using GorillaLocomotion;
using Photon.Pun;

namespace Testplate.Utils
{
    public static class GunLib
    {
        private static GameObject pointer;
        private static LineRenderer lineRenderer;
        private static float animationTime;
        private static VRRig currentLockedRig;
        private static float lastClickTime = 0f;
        private const float clickCooldown = 0.3f;
        private static float lastRightControllerIndexFloat;

        public static bool IsDrawing() => ControllerInputPoller.instance != null && ControllerInputPoller.instance.rightGrab;
        public static bool IsShooting() => ControllerInputPoller.instance != null && ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f;
        public static bool WasShotThisFrame()
        {
            bool shot = ControllerInputPoller.instance != null && ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f && lastRightControllerIndexFloat <= 0.5f;
            if (ControllerInputPoller.instance != null) lastRightControllerIndexFloat = ControllerInputPoller.instance.rightControllerIndexFloat;
            return shot;
        }

        public static void UpdateGun(out RaycastHit hitInfo)
        {
            if (pointer == null)
            {
                pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Object.Destroy(pointer.GetComponent<Rigidbody>());
                Object.Destroy(pointer.GetComponent<SphereCollider>());
                pointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                pointer.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader") ?? Shader.Find("Standard");
            }

            if (lineRenderer == null)
            {
                GameObject lineObj = new GameObject("GunLine");
                lineRenderer = lineObj.AddComponent<LineRenderer>();
                lineRenderer.startWidth = 0.01f;
                lineRenderer.endWidth = 0.01f;
                lineRenderer.numCapVertices = 5;
                lineRenderer.numCornerVertices = 5;
                lineRenderer.positionCount = 30;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            }

            if (IsDrawing())
            {
                pointer.SetActive(true);
                lineRenderer.enabled = true;
                animationTime += Time.deltaTime * 2.5f;

                Transform hand = GorillaTagger.Instance.rightHandTransform;
                Ray ray = new Ray(hand.position, hand.forward);
                
                if (Physics.Raycast(ray, out hitInfo, 100f))
                {
                    VRRig hitRig = hitInfo.collider.GetComponentInParent<VRRig>();

                    if (WasShotThisFrame() && Time.time > lastClickTime + clickCooldown)
                    {
                        lastClickTime = Time.time;
                        if (hitRig != null && hitRig != GorillaTagger.Instance.offlineVRRig)
                        {
                            if (currentLockedRig == hitRig)
                            {
                                currentLockedRig = null;
                            }
                            else
                            {
                                currentLockedRig = hitRig;
                            }
                        }
                        else
                        {
                            currentLockedRig = null;
                        }
                    }

                    if (currentLockedRig != null)
                    {
                        pointer.GetComponent<Renderer>().material.color = Color.red;
                        SetGradient(Color.white, Color.red);
                        pointer.transform.position = currentLockedRig.mainSkin.transform.position;
                        DrawBezier(hand.position, currentLockedRig.mainSkin.transform.position);
                    }
                    else if (hitRig != null && hitRig != GorillaTagger.Instance.offlineVRRig)
                    {
                        pointer.GetComponent<Renderer>().material.color = Color.yellow;
                        SetGradient(Color.white, Color.yellow);
                        pointer.transform.position = hitRig.mainSkin.transform.position;
                        DrawBezier(hand.position, hitRig.mainSkin.transform.position);
                    }
                    else
                    {
                        pointer.GetComponent<Renderer>().material.color = Color.green;
                        SetGradient(Color.white, Color.green);
                        pointer.transform.position = hitInfo.point;
                        DrawBezier(hand.position, hitInfo.point);
                    }
                }
                else
                {
                    if (WasShotThisFrame() && Time.time > lastClickTime + clickCooldown)
                    {
                        lastClickTime = Time.time;
                        currentLockedRig = null;
                    }

                    if (currentLockedRig != null)
                    {
                        pointer.GetComponent<Renderer>().material.color = Color.red;
                        SetGradient(Color.white, Color.red);
                        pointer.transform.position = currentLockedRig.mainSkin.transform.position;
                        DrawBezier(hand.position, currentLockedRig.mainSkin.transform.position);
                    }
                    else
                    {
                        pointer.SetActive(false);
                        lineRenderer.enabled = false;
                    }
                }
            }
            else
            {
                currentLockedRig = null;
                pointer.SetActive(false);
                lineRenderer.enabled = false;
                hitInfo = new RaycastHit();
            }
        }

        public static VRRig GetLockedRig() => currentLockedRig;

        private static void DrawBezier(Vector3 start, Vector3 end)
        {
            float sway = Mathf.Sin(animationTime) * 0.4f;
            float pulse = (Mathf.Sin(animationTime * 5f) + 1f) * 0.05f;
            Vector3 control = (start + end) / 2f + Vector3.up * sway + Vector3.right * Mathf.Cos(animationTime * 0.7f) * 0.2f;
            lineRenderer.startWidth = 0.01f + pulse;
            lineRenderer.endWidth = 0.01f + pulse;
            
            for (int i = 0; i < 30; i++)
            {
                float t = i / 29f;
                lineRenderer.SetPosition(i, CalculateBezierPoint(t, start, control, end));
            }
        }

        private static Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            return u * u * p0 + 2 * u * t * p1 + t * t * p2;
        }

        private static void SetGradient(Color start, Color end)
        {
            Gradient gradient = new Gradient();
            float offset = Mathf.Repeat(animationTime * 0.5f, 1f);
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(start, offset), new GradientColorKey(end, Mathf.Repeat(offset + 0.1f, 1f)) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
            );
            lineRenderer.colorGradient = gradient;
        }

        public static void Cleanup()
        {
            if (pointer != null) Object.Destroy(pointer);
            if (lineRenderer != null) Object.Destroy(lineRenderer.gameObject);
        }
    }
}
