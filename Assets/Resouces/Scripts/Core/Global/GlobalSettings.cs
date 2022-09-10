using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameCore
{
    public class GlobalSettings : MonoBehaviour
    {
        [Header("StartUp Settings: ")]
        [SerializeField] private bool _cursorVisible = false;
        [SerializeField] private CursorLockMode _cursorLockMode = CursorLockMode.Locked;
        [SerializeField] private int _vSyncCount = 0;

        private void Awake()
        {
            Cursor.visible = _cursorVisible;
            Cursor.lockState = _cursorLockMode;
            QualitySettings.vSyncCount = _vSyncCount;
        }
    }
}
