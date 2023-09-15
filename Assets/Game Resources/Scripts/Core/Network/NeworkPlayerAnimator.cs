using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEditor.Animations;
using UnityEngine;

namespace GameCore
{
    namespace Network
    {
        [RequireComponent(typeof(NetworkAnimator))]
        public sealed class NetworkPlayerAnimator : MonoBehaviour
        {
            private void Awake()
            {
                var animator = GetComponent<Animator>();
                animator.fireEvents = false;

                var animatorController = animator.runtimeAnimatorController as AnimatorController;

                for (var i = 0; i < animatorController.layers.Length; i++)
                {
                    var stateMachine = animatorController.layers[i].stateMachine;

                }
            }
        }
    }
}