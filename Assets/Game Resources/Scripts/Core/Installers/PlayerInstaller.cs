using GameCore.Inventory;
using UnityEngine;
using Zenject;


namespace GameCore
{
    namespace Installers
    {
        public class PlayerInstaller : MonoInstaller
        {
            [SerializeField] private GameObject _playerPrefab;
            [SerializeField] private PlayerSpawnPoint _spawnPoint;

            public override void InstallBindings()
            {
                Debug.Log("Creating player.");
                var instance = CreateInstance();
                BindPlayer(instance);
                BindInventory(instance);
                BindHoldItem(instance);
            }

            private GameObject CreateInstance()
            {
                return Container.InstantiatePrefab(_playerPrefab, _spawnPoint.transform.position, Quaternion.identity, null);
            }

            private void BindPlayer(GameObject playerInstance)
            {
                Container.Bind<GameObject>().WithId("Player").FromInstance(playerInstance).AsSingle().NonLazy();
            }

            private void BindInventory(GameObject playerInstance)
            {
                var playerInventory = playerInstance.GetComponent<PlayerInventory>();
                Container.Bind<PlayerInventory>().FromInstance(playerInventory).AsCached().NonLazy();
                Container.Bind<IInventory>().To<PlayerInventory>().FromInstance(playerInventory).AsCached().NonLazy();
            }

            private void BindHoldItem(GameObject playerInstance)
            {
                var playerHoldItem = playerInstance.GetComponent<PlayerHoldItem>();
                Container.Bind<PlayerHoldItem>().FromInstance(playerHoldItem).AsSingle().NonLazy();
            }
        }
    }
}