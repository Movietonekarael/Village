using UnityEngine.InputSystem.XR;

namespace GameCore.GUI
{
    public abstract class UIView<T, P, I> : IUIView<T, P>,
                                            IActivatable<I>,
                                            IDeinitializable<I>
                                            where T : IUIParameters
                                            where P : ISpecificController
                                            where I : ISpecificView
    {
        protected T _Parameters;
        protected P _Controller;

        public abstract void Activate();
        public abstract void Deactivate();
        public abstract void Deinitialize();
        protected abstract void InstantiateViewElements();

        public void Init(T parameters, P controller)
        {
            InitializeParameters(parameters);
            InstantiateViewElements();
            _Controller = controller;
        }

        private void InitializeParameters(T parameters)
        {
            _Parameters = parameters;
        }
    }
}