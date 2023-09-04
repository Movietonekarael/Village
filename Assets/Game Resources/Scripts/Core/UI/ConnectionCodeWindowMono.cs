using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace GameCore
{
    namespace GUI
    {
        public sealed class ConnectionCodeWindowMono : MonoBehaviour
        {
            private ConnectionCodeWindow _owner;
            [SerializeField] private TextMeshProUGUI _text;
            [SerializeField] private Button _okButton;

            private void Awake()
            {
                if (_okButton == null)
                {
                    Debug.LogWarning("OK button is not attached.");
                    return;
                }
                else
                {
                    _okButton.onClick.AddListener(ButtonPressed);
                }
            }

            public void Init(ConnectionCodeWindow owner)
            {
                _owner = owner;
            }

            private void ButtonPressed()
            {
                _owner.
            }
        }
    }
}