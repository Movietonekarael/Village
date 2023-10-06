using GameCore.GameControls;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameCore
{
    namespace GUI
    {
        namespace Windows
        {
            public abstract class OkButtonWindow : MonoBehaviour, IWindow
            {
                [SerializeField] private Button _okButton;
                [Inject] private readonly IEnterable _enter;

                protected abstract void OnAwake();
                protected abstract void ButtonPressed();

                private void Awake()
                {
                    ConnectToButtonEvent();
                    _enter.OnEnter += ButtonPressed;
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
                    _enter.OnEnter -= ButtonPressed;
                    Destroy(this.gameObject);
                }
            }
        }
    }
}