using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameCore.GUI
{
    public sealed class OpenedPlayerInventoryArgs
    {
        public OpenedPlayerInventoryViewParameters OpenedPlayerInventoryViewParameters;
        public int ItemsNumber;
        public EventSystem EventSystem;
        public Camera UiCamera;
    }
}

