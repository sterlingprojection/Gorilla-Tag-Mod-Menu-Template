using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Photon.Pun;
using GorillaNetworking;

namespace Testplate.Utils
{
    public static class VRRigExtensions
    {
        private static FieldInfo creatorField = typeof(VRRig).GetField("creator", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) 
                                              ?? typeof(VRRig).GetField("Creator", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool IsTagged(this VRRig rig)
        {
            if (rig == null) return false;
            
            if (GorillaGameManager.instance != null && GorillaGameManager.instance is GorillaTagManager tagManager)
            {
                var player = rig.GetPlayer();
                return player != null && tagManager.currentInfected.Contains(player);
            }
            return false;
        }

        public static NetPlayer GetPlayer(this VRRig rig)
        {
            if (rig == null) return null;

            if (creatorField != null)
            {
                var creator = creatorField.GetValue(rig) as NetPlayer;
                if (creator != null) return creator;
            }

            try 
            {
                var view = rig.GetPhotonView();
                if (view != null && view.Owner != null)
                {
                    return PhotonNetwork.PlayerList.FirstOrDefault(p => p.UserId == view.Owner.UserId);
                }
            }
            catch { }

            return null;
        }

        public static PhotonView GetPhotonView(this VRRig rig)
        {
            if (rig == null) return null;
            return rig.GetComponent<PhotonView>();
        }
    }
}
