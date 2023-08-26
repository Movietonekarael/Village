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

            /// <summary>
            /// Subscribes controller object to events when initialized and do 
            /// not unsubscribe until destroyed.
            /// </summary>
            protected abstract void SubscribeForPermanentEvents();
            /// <summary>
            /// Unsubscribes controller object from events on destroy.
            /// </summary>
            protected abstract void UnsubscribeForPermanentEvents();
            /// <summary>
            /// Subscribes controller object to events when state entered and do
            /// not unsubscribe until state exit.
            /// </summary>
            protected abstract void SubscribeForTemporaryEvents();
            /// <summary>
            /// Unsubscribes controller object from events on state exit.
            /// </summary>
            protected abstract void UnsubscribeForTemporaryEvents();
            /// <summary>
            /// Uses for initializing view parameters at runtime.
            /// </summary>
            /// <param name="parameters">Parameters to modify. Often ScriptableObject.</param>
            protected abstract void InitializeParameters(T parameters);
            /// <summary>
            /// Executes on state enter.
            /// </summary>
            protected abstract void OnActivate();
            /// <summary>
            /// Executes on state exit.
            /// </summary>
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
                OnActivate();
                _viewActivator?.Activate();
            }

            public void Deactivate()
            {
                UnsubscribeForTemporaryEvents();
                OnDeactivate();
                _viewActivator?.Deactivate();
            }
        }
    }
}