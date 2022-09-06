using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity;
using System.Collections;


namespace MovietoneMath
{
    /// <summary>
    /// Math library for specific cases
    /// </summary>
    public static class MathM
    {
        /// <summary>
        /// Math with unity engine vectors
        /// </summary>
        public static class Vector
        {
            /// <summary>
            /// Returns angle from 0 to 360 depends on vector direction
            /// </summary>
            /// <param name="vec">Vectro2 value</param>
            /// <returns></returns>
            public static float GetAngleOfVector2(Vector2 vec)
            {
                float val = Vector2.Angle(vec, Vector2.up);
                if (vec.x < 0)
                    val *= -1;
                return val;
            }
            /// <summary>
            /// Returns Vector2 with x, y from 0 to 360 depends on angle
            /// </summary>
            /// <param name="angle">Angle value</param>
            /// <returns></returns>
            public static Vector2 GetVector2OfAngle(float angle)
            {
                angle *= -1;
                angle += 90;
                angle = angle / 180 * Mathf.PI;
                Vector2 vec = new(Mathf.Cos(angle), Mathf.Sin(angle));
                return vec;
            }
        }

    }
}
