using UnityEngine;


namespace GameCore
{
    namespace Inventory
    {
        public class InventoryItemMeshInfo : MonoBehaviour
        {
            [SerializeField] private MeshFilter _meshFilter;
            [SerializeField] private MeshRenderer _meshRenderer;

            public MeshFilter MeshFilter
            {
                get
                {
                    return _meshFilter;
                }
            }

            public MeshRenderer MeshRenderer
            {
                get
                {
                    return _meshRenderer;
                }
            }
        }
    }
}