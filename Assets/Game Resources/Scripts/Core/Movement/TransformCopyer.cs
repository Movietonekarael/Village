using UnityEngine;

namespace GameCore
{
    namespace GameMovement
    {
        public sealed class TransformCopyer : MonoBehaviour
        {
            public Transform CopyToTransform;

            private void LateUpdate()
            {
                CopyTransform();
            }

            private void CopyTransform()
            {
                CopyToTransform.SetLocalPositionAndRotation(transform.localPosition, transform.localRotation);
            }
        }
    }
}
