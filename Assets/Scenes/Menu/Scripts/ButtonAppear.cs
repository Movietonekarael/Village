using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GameCore.GUI;


namespace GameCore
{
    namespace Boot
    {
        public class ButtonAppear : MonoBehaviour
        {
            private Animator _animator;

            private readonly int _isActiveID = Animator.StringToHash("Active");
            private readonly int _isActivateID = Animator.StringToHash("Activate");

            [SerializeField] private bool _interactible;
            [SerializeField] private bool _setActive = false;
            [SerializeField] private EventSystem _eventSystem;
            [SerializeField] private UIStateMachine _stateMachine;

            private void Awake()
            {
                _animator = GetComponent<Animator>();
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
                    _eventSystem.SetSelectedGameObject(this.gameObject);
                    _stateMachine.StartStateMachine();
                }
            }
        }
    }
}