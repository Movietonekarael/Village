using UnityEngine;

namespace GameCore
{
    namespace Inventory
    {
        public class PlayerHoldItem : MonoBehaviour, IPlayerHoldItem
        {
            [SerializeField] private Transform _holdPoint;
            [SerializeField] private MeshFilter _meshFilter;
            [SerializeField] private MeshRenderer _meshRenderer;

            private GameItem _item;


            public void SetItem(GameItem item) 
            {
                _item = item;
                ChangeItemMesh();
            }

            private void ChangeItemMesh()
            {

                if (_item is not null)
                {
                    var meshInfo = _item.Prefab.GetComponent<InventoryItemMeshInfo>();
                    if (meshInfo == null)
                    {
                        Debug.LogError("No InventoryItemMeshInfo on Storable prefab. Name: " + _item.Name);
                        return;
                    }

                    _meshFilter.sharedMesh = meshInfo.MeshFilter.sharedMesh;
                    _meshRenderer.sharedMaterial = meshInfo.MeshRenderer.sharedMaterial;

                    _holdPoint.localScale = _item.Prefab.transform.localScale;
                    _holdPoint.localScale *= .01f;
                }
                else
                {
                    _meshFilter.sharedMesh = null;
                    _meshRenderer.sharedMaterial = null;
                }
            }

        }
    }
}