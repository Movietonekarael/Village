using UnityEngine;
using UnityEngine.UI;


namespace GameCore
{
    namespace GUI
    {
        namespace Windows
        {
            public abstract class OkButtonWindow : MonoBehaviour, IWindow
            {
                [SerializeField] private Button _okButton;

                protected abstract void OnAwake();
                protected abstract void ButtonPressed();

                private void Awake()
                {
                    ConnectToButtonEvent();
                    OnAwake();
                }

                private void ConnectToButtonEvent()
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

                public void Close()
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}