using UnityEngine;


namespace GameCore
{
    namespace GUI
    {
        namespace Menus
        {
            [RequireComponent(typeof(Animator))]
            public abstract class CustomUIAnimator : MonoBehaviour
            {
                protected Animator _Animator;

                private void Awake()
                {
                    CacheAnimator();
                    OnAwake();
                }

                protected abstract void OnAwake();

                private void CacheAnimator()
                {
                    _Animator = GetComponent<Animator>();
                }
            }
        }
    }
}