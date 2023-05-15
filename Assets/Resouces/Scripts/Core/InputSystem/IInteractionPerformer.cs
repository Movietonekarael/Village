using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.GameControls
{
    public interface IInteractionPerformer
    {
        public bool IfInteractionWasPerformed();
    }
}