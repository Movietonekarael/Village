using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore;


namespace GameCore.Boot
{
#if MONOCACHE
    public class BootMenuScript : MonoCache
#else
    public class BootMenuScript : MonoBehaviour
#endif
    {
        [SerializeField] List<ButtonsAppear> _buttonsAppears;

        public void UpButtonsPanel()
        {
            foreach (var button in _buttonsAppears)
            {
                button.AppearButton();
            }
        }

    }




}

