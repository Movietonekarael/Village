using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameCore.Shaders
{
#if MONOCACHE
    public class TerrainBlendingBaker : MonoCache
#else
    public class TerrainBlendingBaker : MonoBehaviour
#endif
    {
        public Shader depthShader;
        public RenderTexture depthTexture;
        private Camera _cam;
        [SerializeField] private bool _isDepth = true;

        private void UpdateBakingCamera()
        {
            if (_cam == null)
            {
                _cam = GetComponent<Camera>();
            }

            Shader.SetGlobalFloat("_TB_SCALE", _cam.orthographicSize * 2);

            Shader.SetGlobalFloat("_TB_OFFSET_X", _cam.transform.position.x - _cam.orthographicSize);
            Shader.SetGlobalFloat("_TB_OFFSET_Z", _cam.transform.position.z - _cam.orthographicSize);

            Shader.SetGlobalFloat("_TB_OFFSET_Y", _cam.transform.position.y - _cam.farClipPlane);

            Shader.SetGlobalFloat("_TB_FARCLIP", _cam.farClipPlane);
        }

        private void Start()
        {
            switch (_isDepth)
            {
                case true:
                    BakeTerrainDepth();
                    break;
                case false:
                    BakeTerrainNormals();
                    break;
            }



            //BakeTerrainDepth();
            //BakeTerrainNormals();
            //UpdateBakingCamera();
        }

#if MONOCACHE
        protected override void Run()
#else
        private void Update()
#endif
        {
            //UpdateBakingCamera();
        }


        [ContextMenu("Bake Depth Texture")]
        public void BakeTerrainDepth()
        {
            UpdateBakingCamera();

            if (depthShader != null && depthTexture != null)
            {
                _cam.SetReplacementShader(depthShader, "RenderType");

                _cam.targetTexture = depthTexture;

                Shader.SetGlobalTexture("_TB_DEPTH", depthTexture);
            }
            else
            {
                Debug.Log("You need to assign the depth shader and depth texture in the inspector");
            }
        }



        [ContextMenu("Bake Normal Texture")]
        public void BakeTerrainNormals()
        {
            UpdateBakingCamera();

            if (depthShader != null && depthTexture != null)
            {
                _cam.SetReplacementShader(depthShader, "RenderType");

                _cam.targetTexture = depthTexture;

                Shader.SetGlobalTexture("_TB_NORMALS", depthTexture);
            }
            else
            {
                Debug.Log("You need to assign the depth shader and depth texture in the inspector");
            }
        }
    }
}