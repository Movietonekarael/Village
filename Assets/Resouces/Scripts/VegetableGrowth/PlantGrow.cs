using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example namespace for growing tomatoes test
/// </summary>
namespace GrowVegetablesExample
{
    /// <summary>
    /// Class to attach to scaleble GameObjects
    /// </summary>
    public class PlantGrow : MonoBehaviour
    {
        /// <summary>
        /// Coefficient of scale per tick
        /// </summary>
        public float scaleKoef = 1.05f;


        /// <summary>
        /// Unused container to implement in job
        /// </summary>
        public struct Data
        {
            private float scaleKoef;
            //private float count;

            public Data(float _scaleKoef)
            {
                scaleKoef = _scaleKoef;

                //count = .0f;
            }

            public void Update()
            {
                //objectTransform.localScale *= scaleKoef;
                //count += scaleKoef;
            }
        }


        /// Old unused update method with coroutine
        /*
        private int lastTime = 0;
        public float speed = 2f;
        public float duration = 3f;


        private void Update()
        {
            if (Time.fixedTime - lastTime >= 5)
            {
                //transform.localScale *= 1.05f;
                StartCoroutine(RepeatLerp(transform.localScale, transform.localScale * 1.05f, duration));
                lastTime += 5;
            }
        }
        */
        public IEnumerator RepeatLerp(Vector3 a, Vector3 b, float time, /*not necessary*/float speed)
        {
            float i = 0.0f;
            float rate = (1.0f / time) * speed;
            while (i < 1.0f)
            {
                i += Time.deltaTime * rate;
                transform.localScale = Vector3.Lerp(a, b, i);
                yield return null;
            }
        }
    }
}

