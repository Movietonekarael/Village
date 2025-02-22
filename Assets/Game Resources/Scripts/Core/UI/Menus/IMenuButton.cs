﻿using System;


namespace GameCore
{
    namespace GUI
    {
        namespace Menus
        {
            public interface IMenuButton
            {
                public event Action OnButtonActivated;
                public event Action<uint> OnButtonPressed;

                public void SubscribeForClickEvent();
                public void SetSelected();
                public void SetActive();
                public void SetIndex(uint index);
                public void EnableButtonIfAllowed();
                public void DisableButtonIfAllowed();
            }
        }
    }
}