using UnityEngine;
using TMPro;
using System;
using GameCore.GameControls;
using Zenject;

namespace GameCore
{
    namespace GUI
    {
        namespace Windows
        {
            public sealed class ConnectionCodeWindowMono : OkButtonWindow, IConnectionCodeWindowMono
            {
                public event Action<string> OnOkPressed;

                [SerializeField] private TextMeshProUGUI _text;

                protected override void OnAwake() { }

                protected override void ButtonPressed()
                {
                    var code = _text.text;
                    code = code[..^1];
                    OnOkPressed?.Invoke(code);
                }
            }
        }  
    }
}