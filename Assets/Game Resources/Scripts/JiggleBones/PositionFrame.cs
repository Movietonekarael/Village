using UnityEngine;

namespace JiggleBones
{
    public struct PositionFrame
    {
        public Vector3 position;
        public double time;
        public PositionFrame(Vector3 position, double time)
        {
            this.position = position;
            this.time = time;
        }
    }
}