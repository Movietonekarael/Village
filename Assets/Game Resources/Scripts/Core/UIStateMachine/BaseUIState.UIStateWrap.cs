using UnityEngine;

namespace GameCore.GUI
{
    public abstract partial class BaseUIState<T, I> where T : IUIParameters, new()
                                                    where I : ISpecificController
    {
        [System.Serializable]
        private class UIStateWrap
        {
            [SerializeField]
            [RequireInterface(typeof(IUIState))]
            private UnityEngine.Object _valueBase;

            [HideInInspector] public IUIState Value { get => _valueBase as IUIState; }
        }
    }
}

