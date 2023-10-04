using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameCore
{
    namespace Inventory
    {
        public class PlayerHoldItem : MonoBehaviour, IPlayerHoldItem
        {
            [SerializeField] private Transform _holdPoint;
            [SerializeField] private MeshFilter _meshFilter;
            [SerializeField] private MeshRenderer _meshRenderer;

            private AsyncOperationHandle<GameObject>? _itemLoadHandle;


            public void SetItem(GameItem item) 
            {
                if (item is not null)
                {
                    var newItemLoadHandle = Addressables.LoadAssetAsync<GameObject>(item.PrefabReference);
                    newItemLoadHandle.WaitForCompletion();
                    var itemPrefab = newItemLoadHandle.Result;

                    if (!itemPrefab.TryGetComponent<InventoryItemMeshInfo>(out var meshInfo))
                    {
                        Debug.LogError("No InventoryItemMeshInfo on Storable prefab. Name: " + item.Name);
                        return;
                    }

                    _meshFilter.sharedMesh = meshInfo.MeshFilter.sharedMesh;
                    _meshRenderer.sharedMaterial = meshInfo.MeshRenderer.sharedMaterial;

                    _holdPoint.localScale = itemPrefab.transform.localScale;
                    _holdPoint.localScale *= .01f;

                    ReleaseOldMesh();

                    _itemLoadHandle = newItemLoadHandle;
                }
                else
                {
                    _meshFilter.sharedMesh = null;
                    _meshRenderer.sharedMaterial = null;

                    ReleaseOldMesh();
                }


                void ReleaseOldMesh()
                {
                    if (_itemLoadHandle is not null)
                    {
                        Addressables.Release(_itemLoadHandle.Value);
                        _itemLoadHandle = null;
                    }
                }
            }

            private void OnDestroy()
            {
                if (PlayerHoldItemWrapper.PlayerHoldItem == this as IPlayerHoldItem)
                    PlayerHoldItemWrapper.PlayerHoldItem = null;
            }
        }
    }
}