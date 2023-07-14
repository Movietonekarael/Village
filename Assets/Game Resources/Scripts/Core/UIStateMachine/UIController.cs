using Zenject;


namespace GameCore
{
    namespace GUI
    {
        public abstract class UIController<T, P, I> : IUIController<T>,
                                                   ISpecificController,
                                                   ISubscribable,
                                                   IDeinitializable<P>,
                                                   IActivatable<P>
                                                   where T : IUIParameters
                                                   where P : class, ISpecificController
                                                   where I : ISpecificView
        {
            [Inject] protected I _SpecificView;
            [Inject] private IUIView<T, P> _view;
            [Inject] private IDeinitializable<I> _viewDeinitializator;
            [Inject] private IActivatable<I> _viewActivator;

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
                _view.Init(parameters, this as P);
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