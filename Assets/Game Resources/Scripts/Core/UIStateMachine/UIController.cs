using UnityEngine.InputSystem.XR;
using Zenject;


namespace GameCore
{
    namespace GUI
    {
        public abstract class UIController<T, P, I, K> : IUIController<T>,
                                                   ISpecificController,
                                                   ISubscribable,
                                                   IDeinitializable,
                                                   IActivatable
                                                   where T : IUIParameters
                                                   where P : class, ISpecificController
                                                   where I : K, IUIView<T, P>, IDeinitializable, IActivatable, new()
                                                   where K : ISpecificView
        {
            protected K _View;
            private IUIView<T, P> _view;
            private IDeinitializable _viewDeinitializator;
            private IActivatable _viewActivator;

            protected abstract void SubscribeForEvents();
            protected abstract void UnsubscribeForEvents();
            protected abstract void InitializeParameters(T parameters);
            protected abstract void OnActivate();
            protected abstract void OnDeactivate();

            public void Init(T parameters)
            {
                InitializeParameters(parameters);
                InitializeView(parameters);
                Subscribe();
            }

            private void InitializeView(T parameters)
            {
                InstantiateView();
                _view.Init(parameters, this as P);
            }

            private void InstantiateView()
            {
                var view = new I();
                DiContainerReference.Container.Inject(view);
                _View = view;
                _view = view;
                _viewDeinitializator = view;
                _viewActivator = view;
            }

            public void Subscribe()
            {
                SubscribeForEvents();
            }

            public void Unsubscribe()
            {
                UnsubscribeForEvents();
            }

            public void Deinitialize()
            {
                Unsubscribe();
                _viewDeinitializator?.Deinitialize();
            }

            public void Activate()
            {
                _viewActivator?.Activate();
                OnActivate();
            }

            public void Deactivate()
            {
                _viewActivator?.Deactivate();
                OnDeactivate();
            }
        }
    }
}