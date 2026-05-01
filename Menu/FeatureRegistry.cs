using System;
using System.Collections.Generic;
using Testplate.Menu.UI;
using Testplate.Features;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using GorillaNetworking;
using Photon.Realtime;
using System.Linq;
using Testplate.Utils;

namespace Testplate.Menu
{
    public static class FeatureRegistry
    {
        public static List<MenuTab> Tabs = new List<MenuTab>();
        public static List<FeatureBase> AllFeatures = new List<FeatureBase>();

        private static bool superJump = false;
        private static bool speedBoost = false;
        private static bool fly = false;
        private static float flySpeed = 15f;

        public static void Initialize()
        {
            Tabs.Clear();
            AllFeatures.Clear();

            var movementTab = new MenuTab("Movement");
            movementTab.AddButton(new MenuButton("Super Jump", (Action)(() => superJump = true), (Action)(() => superJump = false), (Func<bool>)(() => superJump)));
            movementTab.AddButton(new MenuButton("Speed Boost", (Action)(() => speedBoost = true), (Action)(() => speedBoost = false), (Func<bool>)(() => speedBoost)));
            movementTab.AddButton(new MenuButton("Fly", (Action)(() => fly = true), (Action)(() => fly = false), (Func<bool>)(() => fly)));
            movementTab.AddButton(new MenuButton("Fly Speed", 5f, 50f, flySpeed, (val) => flySpeed = val));
            Tabs.Add(movementTab);

            var networkTab = new MenuTab("Network");
            networkTab.AddButton(new MenuButton("Disconnect", (Action)(() => PhotonNetwork.Disconnect())));
            networkTab.AddButton(new MenuButton("Join Random", (Action)(() => PhotonNetwork.JoinRandomRoom())));
            Tabs.Add(networkTab);

            var visualTab = new MenuTab("Visual");
            visualTab.AddButton(new MenuButton("Blue Fog", (Action)(() => {
                RenderSettings.fogColor = Color.blue;
                RenderSettings.fog = true;
                RenderSettings.fogMode = FogMode.Linear;
                RenderSettings.fogStartDistance = 0f;
                RenderSettings.fogEndDistance = 100f;
            }), (Action)(() => RenderSettings.fog = false), (Func<bool>)(() => RenderSettings.fog)));
            Tabs.Add(visualTab);
        }

        private static void RegisterFeature(MenuTab tab, FeatureBase feature)
        {
            AllFeatures.Add(feature);
            tab.AddButton(new MenuButton(
                feature.Name,
                (Action)(() => feature.OnEnable()),
                (Action)(() => feature.OnDisable()),
                (Func<bool>)(() => feature.IsEnabled)
            ));
        }

        public static void UpdateFeatures()
        {
            foreach (var feature in AllFeatures)
            {
                if (feature.IsEnabled) feature.OnUpdate();
            }

            if (superJump && GTPlayer.Instance != null)
                GTPlayer.Instance.jumpMultiplier = 2.5f;
            else if (GTPlayer.Instance != null)
                GTPlayer.Instance.jumpMultiplier = 1.1f;

            if (speedBoost && GTPlayer.Instance != null)
                GTPlayer.Instance.maxJumpSpeed = 15f;
            else if (GTPlayer.Instance != null)
                GTPlayer.Instance.maxJumpSpeed = 6.5f;

            if (fly && GTPlayer.Instance != null)
            {
                if (ControllerInputPoller.instance.rightControllerPrimaryButton)
                {
                    GTPlayer.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * Time.deltaTime * flySpeed;
                    GTPlayer.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
            }
        }
    }
}
