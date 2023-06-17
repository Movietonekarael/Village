using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JiggleBones
{
    [System.Serializable]
    public struct JiggleSettingsBase
    {
        public float GravityMultiplier;
        public float Friction;
        public float AirFriction;
        public float Blend;
        public float AngleElasticity;
        public float ElasticitySoften;
        public float LengthElasticity;
    }


    [CreateAssetMenu(fileName = "Jiggles", menuName = "Jiggles/Settings", order = 1)]
    public class JiggleSettings : ScriptableObject
    {
        [Range(0f, 2f)]
        [Tooltip("How much gravity to apply to the simulation, it is a multiplier of the Physics.gravity setting.")]
        public float gravityMultiplier = 1f;

        [Range(0f, 1f)]
        [Tooltip("How much mechanical friction to apply, this is specifically how quickly oscillations come to rest.")]
        public float friction = 0.4f;

        [Range(0f, 1f)]
        [Tooltip("How much air friction to apply, this is how much jiggled objects should get dragged behind during movement. Or how *thick* the air is.")]
        public float airFriction = 0.1f;

        [Range(0f, 1f)]
        [Tooltip("How much of the simulation should be expressed. A value of 0 would make the jiggle have zero effect. A value of 1 gives the full movement as intended.")]
        public float blend = 1f;

        [Range(0f, 1f)]
        [Tooltip("How much angular force to apply in order to push the jiggled object back to rest.")]
        public float angleElasticity = 0.4f;

        [Range(0f, 1f)]
        [Tooltip("How much to allow free bone motion before engaging elasticity.")]
        public float elasticitySoften = 0f;

        [Range(0f, 1f)]
        [Tooltip("How much linear force to apply in order to keep the jiggled object at the correct length. Squash and stretch!")]
        public float lengthElasticity = 0.4f;

        public JiggleSettingsBase GetSettingsStruct()
        {
            var settings = new JiggleSettingsBase()
            {
                GravityMultiplier = gravityMultiplier,
                Friction = friction,
                AirFriction = airFriction,
                Blend = blend,
                AngleElasticity = angleElasticity,
                ElasticitySoften = elasticitySoften,
                LengthElasticity = lengthElasticity
            };

            return settings;
        }
    }

}