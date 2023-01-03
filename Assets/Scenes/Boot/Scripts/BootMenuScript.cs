using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore;


namespace GameCore.Boot
{
    public class BootMenuScript : MonoBehaviour
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

