using UnityEngine;
using TMPro;
using System;


namespace GameCore
{
    namespace GUI
    {
        namespace Windows
        {
            public sealed class MessageWindowMono : OkButtonWindow, IMessageWindowMono
            {
                public event Action OnOkPressed;

                [SerializeField] private TextMeshProUGUI _text;


                protected override void OnAwake() { }


                protected override void ButtonPressed()
                {
                    OnOkPressed?.Invoke();
                }

                public void SetMessage(string message)
                {
                    _text.text = message;
                }
            }
        }
    }
}