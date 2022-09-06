using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore.GameControls;
using GameCore.Inventory;
using UnityEngine.UI;
using System;



namespace GameCore.Interactions
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private Transform _interactionPoint;
        [SerializeField] private float _interactionPointRadius = .5f;
        [SerializeField] private LayerMask _interactableMask;
        [SerializeField] private PlayerController _playerController;

        [SerializeField] private InteractionPromptUI _interactionPromptUI;

        public PlayerInventory inventory;

        private readonly Collider[] _colliders = new Collider[5];
        private int _numFound;

        private Collider _currentCollider = null;
        private bool _isShowingPrompt = false;

        private void Awake()
        {
            _playerController = PlayerController.instance;
            if (_playerController is null)
                Debug.LogWarning("There is not PlayerController in the scene to attach to Interactor script.");

            _interactionPromptUI.DisablePrompt();
        }


        private void Update()
        {
            _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

            if (_numFound > 0)
            {
                var interactable = _colliders[0].GetComponent<IInteractable>();

                if (interactable is null)
                {
                    return;
                }

                _isShowingPrompt = true;
                if (_colliders[0] != _currentCollider)
                {
                    _interactionPromptUI.SetPrompt(interactable.interactionMessage);
                }
                _currentCollider = _colliders[0];


                if (_playerController.IfInteractWasPerformed())
                {
                    if (!interactable.Interact(this))
                    {
                        Debug.Log("Inventory is full");
                    }
                }

            }
            else
            {
                if (_isShowingPrompt == true)
                {
                    _currentCollider = null;
                    _isShowingPrompt = false;
                    _interactionPromptUI.DisablePrompt();
                }
            }

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
        }



    }

}
