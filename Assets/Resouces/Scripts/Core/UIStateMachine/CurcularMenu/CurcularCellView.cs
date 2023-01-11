using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.GUI
{
    public class CurcularCell : MonoBehaviour
    {
        [SerializeField] private Animator _highlightAnimator;

        [SerializeField] private Image _itemImage;
        public Sprite ItemSprite 
        { 
            private get
            {
                return _itemImage.sprite;
            } 
            set
            {
                _itemImage.sprite = value;
            }
        }

        private readonly int _highlightAppearTriggerHash = Animator.StringToHash("Appear");
        private readonly int _highlightDisappearTriggerHash = Animator.StringToHash("Disappear");

        public void Appear()
        {
            _highlightAnimator.SetTrigger(_highlightAppearTriggerHash);
        }

        public void Disappear()
        {
            _highlightAnimator.SetTrigger(_highlightDisappearTriggerHash);
        }
    }
}

