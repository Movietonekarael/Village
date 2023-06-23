using System.Collections.Generic;
using UnityEngine;

namespace JiggleBones
{
    [System.Serializable]
    public class JiggleRig
    {
        [Tooltip("The root bone from which an individual JiggleRig will be constructed. The JiggleRig encompasses all children of the specified root.")]
        public Transform RootTransform;
        [Tooltip("The settings that the rig should update with, create them using the Create->JigglePhysics->Settings menu option.")]
        public JiggleSettings JiggleSettings;
        [Tooltip("The list of transforms to ignore during the jiggle. Each bone listed will also ignore all the children of the specified bone.")]
        public List<Transform> IgnoredTransforms;
        [HideInInspector] public List<JiggleBone> Bones;
        [HideInInspector] public List<Transform> BonesTransforms;

        public JiggleRig(Transform rootTransform, JiggleSettings jiggleSettings)
        {
            RootTransform = rootTransform;
            JiggleSettings = jiggleSettings;
            IgnoredTransforms = new();
            Bones = new();
            BonesTransforms = new();
        }
    }
}

