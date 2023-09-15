using Unity.Netcode;
using UnityEngine;

namespace GameCore
{
    namespace Network
    {
        public static class TransformExtension
        {
            public static void SetTransform(this Transform transform, ref NetworkTransformPR transformPR)
            {
                var localRotation = new Quaternion();
                QuaternionCompressor.DecompressQuaternion(ref localRotation, transformPR.Rotation);
                transform.SetLocalPositionAndRotation(transformPR.Position, localRotation);
            }
            /*
            public static void SetTransform(this Transform transform, ref NetworkTransformPRS transformPRS)
            {
                transform.SetLocalPositionAndRotation(transformPRS.Position, transformPRS.Rotation);
                transform.localScale = transformPRS.Scale;
            }*/
        }
    }
}

