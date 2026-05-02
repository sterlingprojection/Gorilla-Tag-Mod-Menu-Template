using System.Linq;
using Photon.Pun;
using Testplate.Menu;
using Testplate.Utils;
using UnityEngine;

namespace Testplate.Features
{
    public class ExampleGun : FeatureBase
    {
        public override string Name => "Example Gun";

        public override void OnUpdate()
        {
            RaycastHit hit;
            GunLib.UpdateGun(out hit);

            if (!GunLib.IsShooting())
                return;

            VRRig targetRig = GunLib.GetLockedRig();

            if (targetRig == null && hit.collider != null)
                targetRig = hit.collider.GetComponentInParent<VRRig>();

            if (targetRig == null || targetRig == GorillaTagger.Instance.offlineVRRig)
                return;

            var netPlayer = targetRig.GetPlayer();

            if (netPlayer != null)
            {
                Debug.Log($"Example target selected: {netPlayer.NickName}");
            }
        }
    }
}
