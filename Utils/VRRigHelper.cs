using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Testplate.Utils
{
    internal static class VRRigHelper
    {
        private static PropertyInfo instanceProperty;
        private static FieldInfo activeRigsField;
        private static FieldInfo localRigField;

        static VRRigHelper()
        {
            instanceProperty = typeof(VRRigCache).GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            activeRigsField = typeof(VRRigCache).GetField("m_activeRigs", BindingFlags.NonPublic | BindingFlags.Static);
            localRigField = typeof(VRRigCache).GetField("localRig", BindingFlags.Public | BindingFlags.Instance);
        }

        public static VRRig[] GetAllRemoteRigs()
        {
            var instance = instanceProperty?.GetValue(null) as VRRigCache;
            if (instance == null) return new VRRig[0];

            var localRigContainer = localRigField?.GetValue(instance) as RigContainer;
            VRRig localRig = localRigContainer?.Rig;

            var activeRigs = activeRigsField?.GetValue(null) as List<VRRig>;
            if (activeRigs == null) return new VRRig[0];

            return activeRigs.Where(r => r != localRig).ToArray();
        }

        public static VRRig[] GetAllRigs()
        {
            var activeRigs = activeRigsField?.GetValue(null) as List<VRRig>;
            return activeRigs?.ToArray() ?? new VRRig[0];
        }
    }
}
