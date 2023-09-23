using UnityEngine;
using GameCore.GameControls;
using GameCore.Inventory;
using Zenject;


namespace GameCore
{
    namespace Interactions
    {
        public class Interactor : MonoBehaviour
        {
            [Header("IInteraction performer")]
            [SerializeField]
            [RequireInterface(typeof(IInteractionPerformer))]
            private UnityEngine.Object _interactionPerformerBase;
            private IInteractionPerformer _interactionPerformer { get => _interactionPerformerBase as IInteractionPerformer; }

            [Header("Overlap sphere parameters")]
            public Transform InteractionPoint;
            [SerializeField] private float _interactionPointRadius = .5f;
            [SerializeField] private LayerMask _interactableMask;

            [Header("IInventory component")]
            [SerializeField]
            [RequireInterface(typeof(IInventory))]
            private UnityEngine.Object _inventoryBase;
            public IInventory Inventory { get => _inventoryBase as IInventory; }

            [Header("Interaction sound effect")]
            [HideInInspector] public AudioSource InteractionAudio;

            private const int MAX_COLLIDERS = 1;

            private IInteractable _interactableObject = null;


            private void Awake()
            {
                SubscribeForInteraction();
            }

            private void Update()
            {
                if (InteractionPoint != null)
                {
                    InteractWithInteractables();
                }
                
            }

            private void OnDestroy()
            {
                UnsubscribeForInteraction();
            }

            private void InteractWithInteractables()
            {
                var collider = GetCollider();
                GetInteractible(collider, out _interactableObject);
            }

            private Collider GetCollider()
            {
                var hitColliders = new Collider[MAX_COLLIDERS];
                Physics.OverlapSphereNonAlloc(InteractionPoint.position, _interactionPointRadius, hitColliders, _interactableMask);

                if (hitColliders[0] != null)
                    return hitColliders[0];
                else
                    return null;
            }

            private bool GetInteractible(Collider collider, out IInteractable interactable)
            {
                interactable = null;
                if (collider != null)
                    interactable = collider.GetComponent<IInteractable>();

                if (interactable != null)
                    return true;
                else
                    return false;
            }

            private void SubscribeForInteraction()
            {
                _interactionPerformer.OnInteractionPerformed += PerformInteraction;
            }

            private void UnsubscribeForInteraction()
            {
                _interactionPerformer.OnInteractionPerformed -= PerformInteraction;
            }

            private void PerformInteraction()
            {
                if (_interactableObject == null) return;

                _interactableObject.Interact(this);

                if (InteractionAudio != null)
                    InteractionAudio.Play();
            }

            private void OnDrawGizmos()
            {
                if (InteractionPoint != null ) 
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(InteractionPoint.position, _interactionPointRadius);
                }
            }
        }
    }
}