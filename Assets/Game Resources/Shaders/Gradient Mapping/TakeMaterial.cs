using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Shaders
{
    [RequireComponent(typeof(MeshRenderer))]
#if MONOCACHE
    public class TakeMaterial : MonoCache
#else
    public class TakeMaterial : MonoBehaviour
#endif
    {
        [SerializeField] private GradientGenerate _gradientGenerate;
#if MONOCACHE
        protected override void FixedRun()
#else
        private void FixedUpdate()
#endif
        {
            GetComponent<MeshRenderer>().sharedMaterial = _gradientGenerate.rocksMaterial;
        }
    }
}

