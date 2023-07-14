using UnityEngine;


namespace GameCore
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        [SerializeField] private float _gizmosSphereRadius = 1.0f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _gizmosSphereRadius);
        }
    }
}