using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Testplate.Features;
using Testplate.Menu.UI;
using Testplate.Utils;
using UnityEngine;
using static TransferrableObject;

namespace Testplate.Menu
{
    public static class FeatureRegistry
    {
        public static List<MenuTab> Tabs = new List<MenuTab>();
        public static List<FeatureBase> AllFeatures = new List<FeatureBase>();

        private static bool superJump = false;
        private static bool speedBoost = false;
        private static bool fly = false;
        private static bool Tentacale = false;
        private static float flySpeed = 15f;

        private static bool testGun = false;

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

            var opTab = new MenuTab("OP");
            opTab.AddButton(new MenuButton("Test Gun", (Action)(() => testGun = true), (Action)(() => testGun = false), (Func<bool>)(() => testGun)));
            opTab.AddButton(new MenuButton("Tentacle Abuse (M)", (Action)(() => Tentacale = true), (Action)(() => Tentacale = false), (Func<bool>)(() => Tentacale)));
            Tabs.Add(opTab);
        }
        
        public static void Test(Player target)
        {
            Console.Writeline(target.ViewId);
        }


        public static void UpdateFeatures()
        {
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
                    GTPlayer.Instance.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                }
            }

            if (testGun)
            {
                RaycastHit hit;
                GunLib.UpdateGun(out hit);

                if (GunLib.IsShooting())
                {
                    VRRig targetRig = GunLib.GetLockedRig();
                    if (targetRig == null && hit.collider != null)
                    {
                        targetRig = hit.collider.GetComponentInParent<VRRig>();
                    }

                    if (targetRig != null && targetRig != GorillaTagger.Instance.offlineVRRig)
                    {
                        var netPlayer = targetRig.GetPlayer();
                        if (netPlayer != null)
                        {
                            var photonPlayer = PhotonNetwork.PlayerList.FirstOrDefault(p => p.UserId == netPlayer.UserId);
                            if (photonPlayer != null)
                            {
                                if (testGun) Test(photonPlayer);
                            }
                        }
                    }
                }
            }
        }
    }
}
