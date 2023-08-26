using UnityEngine;

namespace GameCore
{
    namespace GUI
    {
        public sealed class LabelAnimator : CustomUIAnimator
        {
            private readonly int _isAnimatedID = Animator.StringToHash("Animated");

            protected override void OnAwake() { }

            public void SetAnimated(bool isAnimated)
            {
                _Animator.SetBool(_isAnimatedID, isAnimated);
            }
        }
    }
}