using UnityEngine;
using UnityEngine.UI;
using GameCore.GUI;
using Zenject;

namespace GameCore
{
    namespace Boot
    {
        [RequireComponent(typeof(UiSelecter))]
        public class ButtonAppear : MonoBehaviour
        {
            private Animator _animator;

            private readonly int _isActiveID = Animator.StringToHash("Active");
            private readonly int _isActivateID = Animator.StringToHash("Activate");

            private ISelectable _selectable;

            [Inject] private readonly UiSelectionService _uiSelectionService;
            [SerializeField] private bool _interactible;
            [SerializeField] private bool _setActive = false;
            [SerializeField] private UIStateMachine _stateMachine;

            private void Awake()
            {
                _animator = GetComponent<Animator>();
                _selectable = GetComponent<UiSelecter>();
            }

            public void AppearButton()
            {
                _animator.SetBool(_isActivateID, true);
            }

            public void SetNowActive()
            {
                _animator.SetBool(_isActiveID, true);

                var button = GetComponent<Button>();
                button.interactable = _interactible;
                if (_setActive)
                {
                    _uiSelectionService.CurrentSelected = _selectable;
                    _stateMachine.StartStateMachine();
                }
            }
        }
    }
}