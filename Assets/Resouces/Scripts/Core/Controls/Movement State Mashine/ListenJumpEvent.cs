using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore;
using GameCore.GameMovement;

#if MONOCACHE
public class ListenJumpEvent : MonoCache
#else
public class ListenJumpEvent : MonoBehaviour
#endif
{
    [SerializeField] private NPCMovement _npc;

    public void JumpEnded()
    {
        if (_npc != null)
            _npc.EndJump();
    }

    public void StartJumpingEnd()
    {
        if (_npc != null)
            _npc.StartJumpingEnd();
    }
}
