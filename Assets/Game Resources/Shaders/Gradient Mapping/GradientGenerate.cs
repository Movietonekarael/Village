using UnityEngine;


namespace GameCore
{
    namespace Shaders
    {
        [ExecuteInEditMode]
#if MONOCACHE
    public class GradientGenerate : MonoCache
#else
        public class GradientGenerate : MonoBehaviour
#endif
        {
            public Material rocksMaterial;
            public bool realtimeGeneration;
            public Gradient lutGradient;
            public Vector2Int lutTextureSize;
            public Texture2D lutTexture;

#if MONOCACHE
        protected override void Run()
#else
            private void Update()
#endif
            {
                if (realtimeGeneration)
                {
                    GenerateLutTexture();
                }
            }

            private void GenerateLutTexture()
            {
                lutTexture = new Texture2D(lutTextureSize.x, lutTextureSize.y)
                {
                    wrapMode = TextureWrapMode.Clamp
                };

                for (var x = 0; x < lutTextureSize.x; x++)
                {
                    var color = lutGradient.Evaluate(x / (float)lutTextureSize.x);
                    for (var y = 0; y < lutTextureSize.y; y++)
                    {
                        lutTexture.SetPixel(x, y, color);
                    }
                }

                lutTexture.Apply();
                rocksMaterial.SetTexture("_GradientRamp", lutTexture);

            }
        }
    }
}