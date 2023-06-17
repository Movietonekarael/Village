using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Inventory
{
    public class PlayerHoldItem : MonoBehaviour
    {
        [SerializeField] private Transform _holdPoint;
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;

        private GameItem _item;

        public GameItem Item
        {
            set
            {
                _item = value;
                ChangeItemMesh();
            }
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

                _meshFilter.sharedMesh = meshInfo.meshFilter.sharedMesh;


                _meshRenderer.sharedMaterial = meshInfo.meshRenderer.sharedMaterial;


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

