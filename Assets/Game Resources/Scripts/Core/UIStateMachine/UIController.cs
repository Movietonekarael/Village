using UnityEngine.InputSystem.XR;
using Zenject;


namespace GameCore
{
    namespace GUI
    {
        public abstract class UIController<T, P, I, K> : IUIController<T>,
                                                   ISpecificController,
                                                   IDeinitializable,
                                                   IActivatable
                                                   where T : IUIParameters
                                                   where P : class, ISpecificController
                                                   where I : K, IUIView<T, P>, IDeinitializable, IActivatable, new()
                                                   where K : ISpecificView
        {
            [Inject] private readonly DiContainer _diContainer;
            
            protected K _View;
            private IUIView<T, P> _view;
            private IDeinitializable _viewDeinitializator;
            private IActivatable _viewActivator;

            protected abstract void SubscribeForPermanentEvents();
            protected abstract void UnsubscribeForPermanentEvents();
            protected abstract void SubscribeForTemporaryEvents();
            protected abstract void UnsubscribeForTemporaryEvents();
            protected abstract void InitializeParameters(T parameters);
            protected abstract void OnActivate();
            protected abstract void OnDeactivate();

            public void Init(T parameters)
            {
                InitializeParameters(parameters);
                InitializeView(parameters);
                SubscribeForPermanentEvents();
            }

            private void InitializeView(T parameters)
            {
                InstantiateView();
                _view.Init(parameters, this as P);
            }

            private void InstantiateView()
            {
                var view = new I();
                _diContainer.Inject(view);
                _View = view;
                _view = view;
                _viewDeinitializator = view;
                _viewActivator = view;
            }

            public void Deinitialize()
            {
                UnsubscribeForPermanentEvents();
                _viewDeinitializator?.Deinitialize();
            }

            public void Activate()
            {
                SubscribeForTemporaryEvents();
                _viewActivator?.Activate();
                OnActivate();
            }

            public void Deactivate()
            {
                UnsubscribeForTemporaryEvents();
                _viewActivator?.Deactivate();
                OnDeactivate();
            }
        }
    }
}