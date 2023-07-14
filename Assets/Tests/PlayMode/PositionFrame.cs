using UnityEngine;
using System;


namespace Testing
{
    public partial class JiggleBonesTesting
    {
        private partial struct BoneTrace
        {
            private struct PositionFrame : IEquatable<PositionFrame>
            {
                public Vector3 position;
                public double time;
                public PositionFrame(Vector3 position, double time)
                {
                    this.position = position;
                    this.time = time;
                }

                public PositionFrame(JiggleBones.PositionFrame frame)
                {
                    position = frame.position;
                    time = frame.time;
                }

                public PositionFrame(JigglePhysics.JiggleBone.PositionFrame frame)
                {
                    position = frame.position;
                    time = frame.time;
                }

                public readonly bool Equals(PositionFrame other)
                {
                    var equality = true;
                    equality = equality && other.position.Equals(position);
                    equality = equality && other.time.Equals(time);
                    return equality;
                }

                public override string ToString()
                {
                    return $"(Position: {position}), (Time: {time})";
                }
            }
        }
    }
}