using GameCore.Inventory;
using UnityEngine;
using Zenject;

namespace GameCore
{
    namespace Installers
    {
        public sealed class IdentificationListInstaller : MonoInstaller
        {
            [SerializeField] private IdentificationList _identificationList;


            public override void InstallBindings()
            {
                BindIdentificationList(_identificationList);


                void BindIdentificationList(IdentificationList instance)
                {
                    Container.Bind<IdentificationList>()
                             .FromInstance(instance)
                             .AsSingle()
                             .NonLazy();
                }
            }
        }
    }
}