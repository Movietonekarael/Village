using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GameCore.Boot
{
    public class ButtonsAppear : MonoBehaviour
    {
        private Animator _animator;

        private readonly int _isActiveID = Animator.StringToHash("Active");
        private readonly int _isActivateID = Animator.StringToHash("Activate");

        [SerializeField] private bool _interactible;

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
        }
    }
}

