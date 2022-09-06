using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace GameCore.LOD
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
#if MONOCACHE
    public class LoadOnLOD : MonoCache
#else
    public class LoadOnLOD : MonoBehaviour
#endif
    {
        private SkinnedMeshRenderer _renderer;

        [SerializeField] private Material _defaultMaterial;
        [SerializeField] private AssetReference _targetMaterial;

        private bool _isVisible = false;

        private AsyncOperationHandle<Material> handle;

        private void Awake()
        {
            _renderer = GetComponent<SkinnedMeshRenderer>();
            _renderer.sharedMaterial = _defaultMaterial;
        }

#if MONOCACHE
        protected override void FixedRun()
#else
        private void FixedUpdate()
#endif
        {
            if (_renderer.isVisible && !_isVisible)
            {
                _isVisible = true;
                StartLoad();
            }
            else if (!_renderer.isVisible && _isVisible)
            {
                _isVisible = false;

                _renderer.sharedMaterial = _defaultMaterial;
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }

            }
        }

        private void StartLoad()
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }

            handle = _targetMaterial.LoadAssetAsync<Material>();

            handle.Completed += (mat) =>
            {
                _renderer.sharedMaterial = mat.Result;
            };
        }


        private IEnumerator ApplyMaterial()
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }

            handle = _targetMaterial.LoadAssetAsync<Material>();
            yield return handle;
            /*if (handle.Status == AsyncOperationStatus.Succeeded)
            {*/
            Debug.Log("Load Succeed: " + name);
            try
            {
                //if (handle.Completed())
                _renderer.sharedMaterial = handle.Result;
            }
            catch (Exception e)
            {
                Debug.LogWarning("In object " + name + "\n" + e.Message);
                //Debug.Log(handle.Status.ToString());
            }
            //}
            //var material = handle.Result;


            //Addressables.Release(handle);

        }

    }

}
