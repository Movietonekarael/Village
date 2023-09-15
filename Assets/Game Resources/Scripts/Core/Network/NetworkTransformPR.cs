using System;
using Unity.Netcode;
using UnityEngine;

namespace GameCore
{
    namespace Network
    {
        public struct NetworkTransformPR : INetworkSerializable
        {
            public Vector3 Position;
            public uint Rotation;

            public NetworkTransformPR(Transform transform)
            {
                Position = transform.localPosition;
                var localRotation = transform.localRotation;
                Rotation = QuaternionCompressor.CompressQuaternion(ref localRotation);
            }

            public NetworkTransformPR(Vector3 position, Quaternion rotation)
            {
                Position = position;
                Rotation = QuaternionCompressor.CompressQuaternion(ref rotation);
            }

            public void ChangeValue(Transform transform)
            {
                Position = transform.localPosition;
                var localRotation = transform.localRotation;
                Rotation = QuaternionCompressor.CompressQuaternion(ref localRotation);
            }

            public void ChangeValue(Vector3 position, Quaternion rotation)
            {
                Position = position;
                Rotation = QuaternionCompressor.CompressQuaternion(ref rotation);
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref Position);
                serializer.SerializeValue(ref Rotation);
            }

            public override string ToString()
            {
                return $"Position: {Position} | Rotation: {Rotation}";
            }

            public override bool Equals(object obj)
            {
                if (obj is not NetworkTransformPR)
                    return false;

                var another = (NetworkTransformPR)obj;
                return Position == another.Position && Rotation == another.Rotation;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Position, Rotation);
            }

            public static bool operator ==(NetworkTransformPR a, NetworkTransformPR b)
            {
                return a.Equals(b);
            }

            public static bool operator !=(NetworkTransformPR a, NetworkTransformPR b)
            {
                return !a.Equals(b);
            }
        }
    }
}

