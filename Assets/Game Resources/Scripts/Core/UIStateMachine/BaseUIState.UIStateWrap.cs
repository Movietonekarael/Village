using UnityEngine;


namespace GameCore
{
    namespace GUI
    {
        [System.Serializable]
        public class UIStateWrap
        {
            [SerializeField]
            [RequireInterface(typeof(IUIState))]
            private UnityEngine.Object _valueBase;

            public bool[] Arguments;

            [HideInInspector] public IUIState Value { get => _valueBase as IUIState; }
        }
    }
}