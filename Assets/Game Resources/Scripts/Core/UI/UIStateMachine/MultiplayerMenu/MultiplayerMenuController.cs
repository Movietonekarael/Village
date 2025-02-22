﻿using GameCore.Network;
using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using GameCore.SceneManagement;


namespace GameCore
{
    namespace GUI
    {
        public sealed class MultiplayerMenuController : UIController<MultiplayerMenuViewParameters,
                                                                     IMultiplayerMenuController,
                                                                     MultiplayerMenuView,
                                                                     IMultiplayerMenuView>,
                                                        IMultiplayerMenuController
        {
            public event Action OnBackToMainMenu;

            private AssetReference _multiPlayerSceneReference;


            protected override void InitializeParameters(MultiplayerMenuViewParameters parameters) { }
            protected override void OnActivate() { }
            protected override void OnDeactivate() { }
            protected override void SubscribeForPermanentEvents() { }
            protected override void SubscribeForTemporaryEvents() { }
            protected override void UnsubscribeForPermanentEvents() { }
            protected override void UnsubscribeForTemporaryEvents() { }


            public void BackToMainMenu()
            {
                OnBackToMainMenu?.Invoke();
            }

            public async void StartHostServer()
            {
                var startRelayServerResult = await TryStartRelayServer();
                var ifAllocationSuccessful = startRelayServerResult.Item1;

                if (ifAllocationSuccessful)
                {
                    await NetworkConnectionService.StartHost(startRelayServerResult.Item2.Value);
                    await AddressablesSceneManager.Singleton.LoadSceneServerAsync(_multiPlayerSceneReference, LoadSceneMode.Single);
                }
            }

            private async Task<(bool, RelayHostData?)> TryStartRelayServer()
            {
                try
                {
                    var hostData = await NetworkConnectionService.StartRelayServer();
                    return (true, hostData);
                }
                catch
                {
                    _View.CreateMessageWindow("Connection error");
                    return (false, null);
                }
            }

            public async void ConnectToServer(string joinCode)
            {
                var connectToRelayResult = await TryConnectToRelayServer(joinCode);
                var ifAllocationIsSuccessful = connectToRelayResult.Item1;

                if (ifAllocationIsSuccessful) 
                {
                    NetworkConnectionService.StartClient(connectToRelayResult.Item2.Value);
                }
            }

            private async Task<(bool, RelayJoinData?)> TryConnectToRelayServer(string joinCode)
            {
                try
                {
                    var joinData = await NetworkConnectionService.ConnectToRelayServer(joinCode);
                    return (true, joinData);
                }
                catch
                {
                    _View.CreateMessageWindow("Connection error");
                    return (false, null);
                }
            }

            public void SetMultiPlayerSceneReference(AssetReference sceneReference)
            {
                _multiPlayerSceneReference = sceneReference;
            }
        }
    }
}