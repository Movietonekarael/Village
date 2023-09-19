using GameCore.Network;
using TMPro;
using UnityEngine;

namespace GameCore
{ 
    namespace GUI
    {
        public sealed class ConnectionCodeText : MonoBehaviour
        {
            private void Awake()
            {
                var text = GetComponent<TextMeshProUGUI>();
                if (NetworkConnectionService.ConnectionCode != string.Empty)
                {
                    text.text = $"Connection code: {NetworkConnectionService.ConnectionCode}";
                }
                else
                {
                    text.text = string.Empty;
                }
            }
        }
    }
}