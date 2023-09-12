using UnityEngine;


namespace GameCore
{
    public class GlobalSettings : MonoBehaviour
    {
        [Header("StartUp Settings: ")]
        [SerializeField] private bool _cursorVisible = false;
        [SerializeField] private CursorLockMode _cursorLockMode = CursorLockMode.Locked;
        [SerializeField] private int _targetFramerate = 0;

        private void Awake()
        {
            Cursor.visible = _cursorVisible;
            Cursor.lockState = _cursorLockMode;
            Application.targetFrameRate = _targetFramerate > 0 ? _targetFramerate : -1;
        }
    }
}
