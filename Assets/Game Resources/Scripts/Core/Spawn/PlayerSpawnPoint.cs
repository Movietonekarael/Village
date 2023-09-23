using UnityEngine;


namespace GameCore
{
    public sealed class PlayerSpawnPoint : MonoBehaviour
    {
        [SerializeField] private float _gizmosSphereRadius = 1.0f;

        private static PlayerSpawnPoint _instance;

        private void Awake()
        {
            _instance = this;
        }

        public static bool TryGetSpawnPoint(out Vector3 spawnPoint)
        {
            if (_instance != null)
            {
                spawnPoint = _instance.transform.position;
                return true;
            }
            else
            {
                spawnPoint = Vector3.zero;
                return false;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _gizmosSphereRadius);
        }
    }
}