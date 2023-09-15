using Unity.Netcode;
using UnityEngine;

namespace GameCore
{
    namespace Network
    {/*
        public struct NetworkTransformPRS : INetworkSerializable
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Scale;

            public NetworkTransformPRS(Transform transform)
            {
                Position = transform.localPosition;
                Rotation = transform.localRotation;
                Scale = transform.localScale;
            }

            public NetworkTransformPRS(Vector3 position, Quaternion rotation)
            {
                Position = position;
                Rotation = rotation;
                Scale = Vector3.one;
            }

            public NetworkTransformPRS(Vector3 position, Quaternion rotation, Vector3 scale)
            {
                Position = position;
                Rotation = rotation;
                Scale = scale;
            }

            public NetworkTransformPRS(NetworkTransformPR transformPR)
            {
                Position = transformPR.Position;
                Rotation = transformPR.Rotation;
                Scale = Vector3.one;
            }

            public NetworkTransformPRS(NetworkTransformPR transformPR, Vector3 scale)
            {
                Position = transformPR.Position;
                Rotation = transformPR.Rotation;
                Scale = scale;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref Position);
                serializer.SerializeValue(ref Rotation);
                serializer.SerializeValue(ref Scale);
            }
        }*/
    }
}

