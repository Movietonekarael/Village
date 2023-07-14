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
            [Inject] private readonly InputHandler _interactionPerformerBase;
            private IInteractionPerformer _interactionPerformer => _interactionPerformerBase as IInteractionPerformer;

            [Header("Overlap sphere parameters")]
            [SerializeField] private Transform _interactionPoint;
            [SerializeField] private float _interactionPointRadius = .5f;
            [SerializeField] private LayerMask _interactableMask;

            [Header("IInventory component")]
            [SerializeField]
            [RequireInterface(typeof(IInventory))]
            private UnityEngine.Object _inventoryBase;
            public IInventory Inventory { get => _inventoryBase as IInventory; }

            [Header("Interaction sound effect")]
            [SerializeField] private AudioSource _interactionAudio;

            private const int MAX_COLLIDERS = 1;


            private void Update()
            {
                InteractWithInteractables();
            }

            private void InteractWithInteractables()
            {
                var collider = GetCollider();

                if (collider != null && CheckInteractible(collider, out IInteractable interactable))
                {
                    InteractIfInteractionPerformed(interactable);
                }
            }

            private Collider GetCollider()
            {
                var hitColliders = new Collider[MAX_COLLIDERS];
                Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, hitColliders, _interactableMask);

                if (hitColliders[0] != null)
                    return hitColliders[0];
                else
                    return null;
            }

            private bool CheckInteractible(Collider collider, out IInteractable interactable)
            {
                interactable = collider.GetComponent<IInteractable>();

                if (interactable != null)
                    return true;
                else
                    return false;
            }

            private void InteractIfInteractionPerformed(IInteractable interactable)
            {
                if (_interactionPerformer.IfInteractionWasPerformed())
                {
                    interactable.Interact(this);
                    _interactionAudio.Play();
                }
            }

            private void OnDrawGizmos()
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
            }
        }
    }
}