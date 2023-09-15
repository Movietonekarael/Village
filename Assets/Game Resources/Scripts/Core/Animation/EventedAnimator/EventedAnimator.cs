using UnityEngine;

namespace GameCore
{
    namespace Animation
    {
        public sealed partial class EventedAnimator : MonoBehaviour, IEventedAnimator, IEventedAnimatorEvents
        {
            [SerializeField] private Animator _animator;

            private void Awake()
            {
                CheckForAnimator();
            }

            private void CheckForAnimator()
            {
                if (_animator == null)
                {
                    Debug.LogWarning("[Warning]: Animator wasn't attached to evented animator. Trying to find animator on the gameObject.");
                    _animator = GetComponent<Animator>();

                    CheckIfAnimatorNowWasAttached();
                }
            }

            private void CheckIfAnimatorNowWasAttached()
            {
                if (_animator != null)
                {
                    Debug.LogWarning("[Warning]: Animator was found. Keep going.");
                }
                else
                {
                    Debug.LogError("[Error]: Animator wasn't found. Please attach manually in editor.");
                }
            }
        }
    }
}