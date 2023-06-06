using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore;


namespace GameCore.Boot
{
    public class ButtonsAppearAnimator : MonoBehaviour
    {
        [SerializeField] List<ButtonAppear> _buttonsAppears;

        public void AppearAllButtons()
        {
            foreach (var button in _buttonsAppears)
            {
                button.AppearButton();
            }
        }

    }




}

