using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;


namespace GameCore
{
    namespace Installers
    {
        public sealed class EventSystemInstaller : MonoInstaller
        {
            [SerializeField] private EventSystem _eventSystem;

            public override void InstallBindings()
            {
                BindEventSystem();
            }

            private void BindEventSystem()
            {
                Container.Bind<EventSystem>().FromInstance(_eventSystem).AsSingle().NonLazy();
            }
        }
    }
}